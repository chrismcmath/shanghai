//-------------------------------------------------------------------------
// RetinaPro for NGUI
// © oeFun, Inc. 2012-2013
// http://oefun.com
// 
// NGUI and Tasharen are trademarks and copyright of Tasharen Entertainment
//-------------------------------------------------------------------------

//#define RETINAPRO_DEBUGLOG

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


[System.Serializable]
public class retinaProAtlasWindow : EditorWindow {
	
	float 					progressPeriod;
	float 					progressPortion;
	string 					progressString;
	int						deviceIndex;
	int						fileIndex;
	retinaProAtlas			genAtlasItem;
	UIAtlas					genAtlas;
	List<UISpriteData>	 	oldSpriteData;			// used to save sprite data associated with an atlas while we are refreshing the atlas

	int			 			editorSelectedAtlasIdx;

	[MenuItem ("Window/RetinaPro/Atlases")]
    static void Init () 
	{
		retinaProAtlasWindow w = (retinaProAtlasWindow) EditorWindow.GetWindow (typeof (retinaProAtlasWindow));
		w.title = "Atlases";
    }
	
	void OnEnable()
	{
		hideFlags = HideFlags.HideAndDontSave;
		editorSelectedAtlasIdx = -1;
	}
	
	void OnDisable()
	{
	}
	
	void OnDestroy()
	{
	}

	void OnGUI () 
	{
		if (retinaProState.state == retinaProState.rpState.kWaiting)
		{
			bool setupComplete = retinaProConfig.isSetupComplete();
			
			if (setupComplete)
			{
				retinaProDataSerialize.sharedInstance.loadSettings();
				bool needOneValidDevice = retinaProConfig.isOneValidDevice();
				if (needOneValidDevice)
				{
					showAtlasUI();
				}
				else
				{
					EditorGUILayout.Space();
					GUILayout.Label("Atlases", EditorStyles.boldLabel);
					EditorGUILayout.Space();
	
					retinaProConfig.showValidDeviceNeededUI();
				}
			}
			else
			{
				retinaProConfig.showSetupUI();
			}
		}
		else
		{
			refreshProgressPeriod();

			EditorUtility.DisplayProgressBar(
                "Atlas Progress",
                progressString,
                progressPeriod);
		}
	}
	
	void OnInspectorUpdate()
	{
		bool refresh = false;
		
		switch(retinaProState.state)
		{
			default:
			case retinaProState.rpState.kWaiting:
				break;
			
			case retinaProState.rpState.kGen:
			{
				deviceIndex = 0;
				fileIndex = 0;
				progressPeriod = 0.0f;
				progressString = "Atlas " + genAtlasItem.atlasName;
			
#if RETINAPRO_DEBUGLOG
				Debug.Log(progressString);
#endif	
			
				if (genAtlasItem.isFont)
				{
					retinaProState.state = retinaProState.rpState.kFont;
				}
				else
				{
					// create atlas that will be used as the reference for the device specific atlas
					UIAtlas atlasRef = retinaProNGTools.createAtlas(genAtlasItem.atlasName, null, out oldSpriteData);
					oldSpriteData = null;
					if (atlasRef == null)
					{
						Debug.LogWarning("Could not create atlas reference for " + genAtlasItem.atlasName);
						retinaProState.state = retinaProState.rpState.kDone;
						break;
					}
				
					retinaProParent parent = atlasRef.gameObject.GetComponent<retinaProParent>();
					if (parent == null)
					{
						atlasRef.gameObject.AddComponent<retinaProParent>();
					}
				
					EditorUtility.SetDirty(atlasRef.gameObject);
					retinaProState.state = retinaProState.rpState.kAtlas;
				}

				break;
			}
			

		case retinaProState.rpState.kAtlas: {
			retinaProDevice deviceItem = retinaProDataSerialize.sharedInstance.deviceList[deviceIndex];
			progressString = "Atlas " + genAtlasItem.atlasName + " / " + deviceItem.name + " - Processing images";
			retinaProState.state = retinaProState.rpState.kAtlasProcess;
			break;
		}


		case retinaProState.rpState.kAtlasProcess: {
			retinaProDevice deviceItem = retinaProDataSerialize.sharedInstance.deviceList[deviceIndex];
			
#if RETINAPRO_DEBUGLOG
			Debug.Log("addNewAtlas; " + genAtlasItem.atlasName + ", for device: " + deviceItem.name + ", pixelSize = " + deviceItem.pixelSize);
#endif	
			// gather textures for this atlas / device
			DirectoryInfo dinfo = new DirectoryInfo(retinaProFileLock.baseDataPath + retinaProConfig.atlasTextureFolder + deviceItem.name + "/" + genAtlasItem.atlasName);
			if (dinfo == null || !dinfo.Exists) {
					Debug.LogWarning("Folder does not exist; " + genAtlasItem.atlasName + ", for device: " + deviceItem.name + ", pixelSize = " + deviceItem.pixelSize);
			} else {
				List<FileInfo> fis;
				retinaProConfig.getValidArtFiles(dinfo, out fis);

				if (fis != null && fis.Count > 0) {
	                    genAtlas = retinaProNGTools.createAtlas(genAtlasItem.atlasName, deviceItem.name, out oldSpriteData);
					if (genAtlas == null) {
						Debug.LogWarning("Could not create atlas for " + genAtlasItem.atlasName + ", device = " + deviceItem.name);
						retinaProState.state = retinaProState.rpState.kDone;
						break;
					}

					NGUISettings.atlas = genAtlas;
	                NGUISettings.atlasPadding = genAtlasItem.atlasPadding;
	                NGUISettings.atlasTrimming = false;
					NGUISettings.allow4096 = true;
					NGUISettings.fontTexture = null;
					NGUISettings.unityPacking = true;
                        
	                genAtlas.pixelSize = deviceItem.pixelSize;
	                EditorUtility.SetDirty(genAtlas.gameObject);

					// add all art files into the atlas (one-pass)
					List<Texture> textures = new List<Texture>();
			
					foreach(FileInfo fi in fis) {
						// check to see if this file has a corresponding .txt file (i.e. it's a font)
						bool isFont = false;
						{
							string fontName = Path.GetFileNameWithoutExtension(fi.Name);
							if (dinfo != null && dinfo.Exists)
							{
								FileInfo [] fontTextFile = dinfo.GetFiles(fontName + ".txt");
								if (fontTextFile != null && fontTextFile.Length == 1)
								{
									isFont = true;
								}
							}
						}

						if (isFont) {
							progressString = "Atlas " + genAtlasItem.atlasName + " / " + deviceItem.name + " - Fonts in a sprite atlas are not supported!";
							string fontName = Path.GetFileNameWithoutExtension(fi.Name);
							Debug.LogWarning("Fonts (" + fontName + ") in a sprite atlas is not supported. Use a font atlas instead, see the example scene.");
						} else {
							{
								string textureName = "Assets" + retinaProConfig.atlasTextureFolder + deviceItem.name + "/" + genAtlasItem.atlasName + "/" + fi.Name;
	
								// source texture
								Texture2D tex = AssetDatabase.LoadAssetAtPath(textureName, typeof(Texture2D)) as Texture2D;
								if (retinaProDataSerialize.sharedInstance.getUtilityRefreshSourceTextures()) {		// refresh importer settings on source texture?
									// update texture importer settings on source artwork
									// this ensures that we don't bring in assets into the atlas that are too small (for their given size)
									TextureImporter tImporter = AssetImporter.GetAtPath(textureName) as TextureImporter;
									if (tImporter != null) {
										tImporter.textureType = TextureImporterType.Advanced;
										tImporter.normalmap = false;
										tImporter.linearTexture = true;
										tImporter.alphaIsTransparency = true;
										tImporter.convertToNormalmap = false;
										tImporter.grayscaleToAlpha = false;
										tImporter.lightmap = false;
										tImporter.npotScale = TextureImporterNPOTScale.None;
										tImporter.filterMode = FilterMode.Point;
										tImporter.maxTextureSize = 4096;
										tImporter.mipmapEnabled = false;
										tImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
										AssetDatabase.ImportAsset(textureName, ImportAssetOptions.ForceUpdate);
									}
								}
									
								tex.filterMode = genAtlasItem.atlasFilterMode;
								tex.wrapMode = TextureWrapMode.Clamp;
								textures.Add(tex);

#if RETINAPRO_DEBUGLOG
								Debug.Log("- added tex: " + textureName);
#endif
							}
						}
					}

					List<UIAtlasMaker.SpriteEntry> sprites = UIAtlasMaker.CreateSprites(textures);
					UIAtlasMaker.ExtractSprites(genAtlas, sprites);
					UIAtlasMaker.UpdateAtlas(genAtlas, sprites);
					AssetDatabase.SaveAssets();

					// set texture filter mode
					{
						string newTex = "Assets" + retinaProConfig.atlasResourceFolder + deviceItem.name + "/" + genAtlasItem.atlasName + "~" + deviceItem.name + ".png";
						TextureImporter tImporter = AssetImporter.GetAtPath(newTex) as TextureImporter;
						
						if (tImporter != null) {
							tImporter.filterMode = genAtlasItem.atlasFilterMode;
							tImporter.textureFormat = genAtlasItem.atlasTextureFormat;
							AssetDatabase.ImportAsset(newTex, ImportAssetOptions.ForceUpdate);
						}
					}
			
					// restore the sprite data within the atlas
					// update the new atlas with the old sprite data
					retinaProNGTools.updateAtlasSpriteData(ref genAtlas, ref oldSpriteData);
#if RETINAPRO_DEBUGLOG
					retinaProNGTools.debugAtlasSpriteData(ref genAtlas);
#endif
				
					genAtlas.MarkAsChanged();
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();

				
					// continue with next device
					deviceIndex++;
					if (deviceIndex >= retinaProDataSerialize.sharedInstance.deviceList.Count) {
						retinaProState.state = retinaProState.rpState.kDone;
						break;
					} else {
						retinaProState.state = retinaProState.rpState.kAtlas;
					}

					progressPortion = 0.0f;
					break;
				}
			}
			break;
		}
			
			case retinaProState.rpState.kFont:
			{
				retinaProDevice deviceItem = retinaProDataSerialize.sharedInstance.deviceList[deviceIndex];

				DirectoryInfo dinfo = new DirectoryInfo(retinaProFileLock.baseDataPath + retinaProConfig.atlasTextureFolder + deviceItem.name + "/" + genAtlasItem.atlasName);
				if (dinfo == null || !dinfo.Exists)
				{
					Debug.LogWarning("Folder does not exist; " + genAtlasItem.atlasName + ", for device: " + deviceItem.name + ", pixelSize = " + deviceItem.pixelSize);
					retinaProState.state = retinaProState.rpState.kDone;
					break;
				}

				FileInfo [] fis = dinfo.GetFiles("*.png");
				FileInfo [] fisTxt = dinfo.GetFiles("*.txt");
			
				if (fis == null || fis.Length != 1 || fisTxt == null || fisTxt.Length != 1)
				{
					Debug.LogWarning("Font atlases should contain two files; thefont.png / thefont.txt");
					fileIndex = fis.Length;
					retinaProState.state = retinaProState.rpState.kDone;
					break;
				}

				FileInfo fi = fis[fileIndex];
			
				{
					string fontName = Path.GetFileNameWithoutExtension(fi.Name);
					progressString = "Atlas " + genAtlasItem.atlasName + " / " + deviceItem.name + " / " + fontName;
						
#if RETINAPRO_DEBUGLOG
					Debug.Log("addNewFont; " + fontName + ", for device: " + deviceItem.name + ", pixelSize = " + deviceItem.pixelSize);
#endif						
			
					UIFont font = retinaProNGTools.createFont(genAtlasItem.atlasName, fontName, deviceItem.name);
					//font.pixelSize = deviceItem.pixelSize;
					EditorUtility.SetDirty(font.gameObject);

					// create the reference font version
					UIFont fontRef = retinaProNGTools.createFont(genAtlasItem.atlasName, fontName, null);
					fontRef.replacement = font;
					//fontRef.pixelSize = deviceItem.pixelSize;
				
					retinaProParent parent = fontRef.gameObject.GetComponent<retinaProParent>();
					if (parent == null)
						fontRef.gameObject.AddComponent<retinaProParent>();
				
					EditorUtility.SetDirty(fontRef.gameObject);
				}
			
				// set texture filter mode
				{
					string newTex = "Assets" + retinaProConfig.atlasTextureFolder + deviceItem.name + "/" + genAtlasItem.atlasName + "/" + fis[0].Name;
					TextureImporter tImporter = AssetImporter.GetAtPath(newTex) as TextureImporter;
					
					if (tImporter != null)
					{
						tImporter.filterMode = genAtlasItem.atlasFilterMode;
						tImporter.textureFormat = genAtlasItem.atlasTextureFormat;
						AssetDatabase.ImportAsset(newTex, ImportAssetOptions.ForceUpdate);
					}
				}
			

				fileIndex++;
				if (fileIndex >= fis.Length)
				{
					fileIndex = 0;

					deviceIndex++;
					if (deviceIndex >= retinaProDataSerialize.sharedInstance.deviceList.Count)
					{
						fileIndex = fis.Length;
						retinaProState.state = retinaProState.rpState.kDone;
					}
				}
				progressPortion = (((float)(fileIndex+1)) / ((float)fis.Length));
				progressPortion = Mathf.Clamp01(progressPortion);
				break;
			}
			
			case retinaProState.rpState.kDone:
			{
				EditorUtility.ClearProgressBar();
				refresh = true;
				retinaProState.state = retinaProState.rpState.kWaiting;
				Repaint();
				break;
			}

		
		
		
		}
		
		if (refresh)
		{
#if RETINAPRO_DEBUGLOG
			Debug.Log("refresh called");
#endif
			int idx = retinaProDataSerialize.sharedInstance.getPreviewDeviceIdx();
			retinaProDevice di = retinaProDataSerialize.sharedInstance.deviceList[idx];
			if (di.isDeviceValid())
			{
				retinaProNGTools.refreshReferencesForDevice(di, retinaProDataSerialize.sharedInstance.getPreviewScreenIdx(), retinaProDataSerialize.sharedInstance.getPreviewGameViewIdx());
				retinaProNGTools.refresh();
			}
		}
		
		if (retinaProState.state != retinaProState.rpState.kWaiting)
		{
			Repaint();
		}
	}
	
	
	void showAtlasUI()
	{
		bool save = false;
		bool refresh = false;
		
		EditorGUI.BeginDisabledGroup(retinaProState.state != retinaProState.rpState.kWaiting);
		EditorGUILayout.Space();
		GUILayout.Label("Atlases", EditorStyles.boldLabel);
		EditorGUILayout.Space();
		EditorGUI.EndDisabledGroup();
		
		// begin scrolling section which contains all of our atlas list items
		float buttonHeight = 22.0f;
		float numLines = 2.0f;
		if (retinaProDataSerialize.sharedInstance.atlasList != null)
		{
			numLines = (float) retinaProDataSerialize.sharedInstance.atlasList.Count;
			if (numLines > 12)
				numLines = 12;
		}
		
		float scrollHeight = (numLines * buttonHeight) + (numLines * 3.0f);
		GUILayout.BeginVertical(GUILayout.Height(scrollHeight));
		
		Vector2 atlasScrollPos = Vector2.zero;
		atlasScrollPos.x = EditorPrefs.GetFloat("atlasScrollPosX", 0.0f);
		atlasScrollPos.y = EditorPrefs.GetFloat("atlasScrollPosY", 0.0f);

		atlasScrollPos = GUILayout.BeginScrollView(atlasScrollPos, false, false);
		
		EditorPrefs.SetFloat("atlasScrollPosX", atlasScrollPos.x);
		EditorPrefs.SetFloat("atlasScrollPosY", atlasScrollPos.y);

		EditorGUI.BeginDisabledGroup(retinaProState.state != retinaProState.rpState.kWaiting);
		
		
		
		// show each atlas and it's configuration
		if (retinaProDataSerialize.sharedInstance.atlasList != null)
		{
			for (int i=0; i<retinaProDataSerialize.sharedInstance.atlasList.Count; i++)
			{
				retinaProAtlas rpd = retinaProDataSerialize.sharedInstance.atlasList[i];
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("", GUILayout.Width(50f));

				if (rpd != null)
				{
					bool selected = false;
					if (editorSelectedAtlasIdx == i)
					{
						selected = true;
					}
					
					string str = rpd.atlasName;
					if (selected)
						str = "[X]  " + str;
					
					bool selectAtlas = GUILayout.Button(str, GUILayout.Width(250f), GUILayout.Height(buttonHeight));
					if (selectAtlas)
					{
						editorSelectedAtlasIdx = i;
					}
				}

				
				GUILayout.EndHorizontal();
			}
		}
		
		EditorGUI.EndDisabledGroup();

		GUILayout.EndScrollView();
		GUILayout.EndVertical();

		
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		// show the add atlas button
		EditorGUI.BeginDisabledGroup(retinaProState.state != retinaProState.rpState.kWaiting);
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.Width(50f));
			
			bool pressed = GUILayout.Button("Add Atlas", GUILayout.Width(200f));
			if (pressed)
			{
				retinaProAtlas newItem = new retinaProAtlas();
				retinaProDataSerialize.sharedInstance.atlasList.Add(newItem);
				
				editorSelectedAtlasIdx = retinaProDataSerialize.sharedInstance.atlasList.Count-1;
				
				save = true;
			}
			GUILayout.EndHorizontal();
		}

		EditorGUI.EndDisabledGroup();
		
		if (editorSelectedAtlasIdx != -1)
		{
			retinaProAtlas editorSelectedAtlas = retinaProDataSerialize.sharedInstance.atlasList[editorSelectedAtlasIdx];

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.Space();
			GUILayout.Label("Selected Atlas", EditorStyles.boldLabel);
			EditorGUILayout.Space();

			EditorGUI.BeginDisabledGroup(retinaProState.state != retinaProState.rpState.kWaiting);
			
			GUILayout.BeginHorizontal();

			GUILayout.Label("Name:", GUILayout.Width(50f));
			if (editorSelectedAtlas != null)
			{
				string n = GUILayout.TextField(editorSelectedAtlas.atlasName, GUILayout.MaxWidth(150f));
				if (n.CompareTo(editorSelectedAtlas.atlasName) != 0)
				{
					editorSelectedAtlas.atlasName = n;
					save = true;
				}
			}

			if (editorSelectedAtlas != null)
			{
				bool isFont = GUILayout.Toggle(editorSelectedAtlas.isFont, "Font", GUILayout.Width(50f));
				if (isFont != editorSelectedAtlas.isFont)
				{
					editorSelectedAtlas.isFont = isFont;
					save = true;
				}
			}

			bool removeDevice = GUILayout.Button("Remove Atlas", GUILayout.Width(100f));
			if (removeDevice)
			{
				for (int i=0; i<retinaProDataSerialize.sharedInstance.atlasList.Count; i++)
				{
					if (retinaProDataSerialize.sharedInstance.atlasList[i].atlasName.CompareTo(editorSelectedAtlas.atlasName) == 0)
					{
						retinaProDataSerialize.sharedInstance.atlasList.RemoveAt(i);
						break;
					}
				}
				
				editorSelectedAtlasIdx = -1;
				editorSelectedAtlas = null;
				save = true;
			}

			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Padding:", GUILayout.Width(50f));
			if (editorSelectedAtlas != null)
			{
				int p = EditorGUILayout.IntField(editorSelectedAtlas.atlasPadding, GUILayout.Width(100f));
				if (p != editorSelectedAtlas.atlasPadding)
				{
					editorSelectedAtlas.atlasPadding = p;
					save = true;
				}
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Filter:", GUILayout.Width(50f));
			if (editorSelectedAtlas != null)
			{
				FilterMode f = (FilterMode) EditorGUILayout.EnumPopup(editorSelectedAtlas.atlasFilterMode, GUILayout.Width(100f));
				if (f != editorSelectedAtlas.atlasFilterMode)
				{
					editorSelectedAtlas.atlasFilterMode = f;
					save = true;
				}
			}
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Format:", GUILayout.Width(50f));
			if (editorSelectedAtlas != null)
			{
				TextureImporterFormat f = (TextureImporterFormat) EditorGUILayout.EnumPopup(editorSelectedAtlas.atlasTextureFormat, GUILayout.Width(200f));
				if (f != editorSelectedAtlas.atlasTextureFormat)
				{
					editorSelectedAtlas.atlasTextureFormat = f;
					save = true;
				}
			}
			GUILayout.EndHorizontal();
			
			// display required folders
			bool foldersValid = showFoldersUI(ref editorSelectedAtlas);
			
			if (foldersValid)
			{
				// folders exist
				// validate the files within the folders
				bool filesValid = showFilesUI(ref editorSelectedAtlas);
				
				if (filesValid)
				{
					if (editorSelectedAtlas.atlasName != null && editorSelectedAtlas.atlasName.Length > 0)
					{
						// allow user to create / refresh the atlas
						GUILayout.BeginHorizontal();
						GUILayout.Label("", GUILayout.Width(50f));
						
						bool refreshAtlas = false;
						bool atlasExists = isAtlasExist(editorSelectedAtlas.atlasName);
						if (atlasExists)
						{
							refreshAtlas = GUILayout.Button("Refresh", GUILayout.Width(60f));
						}
						else
						{
							refreshAtlas = GUILayout.Button("Create", GUILayout.Width(60f));
						}
						
						if (refreshAtlas)
						{
							genAtlasItem = editorSelectedAtlas;
							
							EditorUtility.ClearProgressBar();
							progressPeriod = 0.0f;
							progressPortion = 0.0f;
							progressString = "Preparing...";
							retinaProState.state = retinaProState.rpState.kGen;
							deviceIndex = 0;
							fileIndex = 0;
							progressPeriod = 0.0f;
						}
						
						GUILayout.EndHorizontal();
					}
				}
				else
				{
				}
				
			}
			else
			{
				// provide option for fixing folders for this atlas
				GUILayout.BeginHorizontal();
				GUILayout.Label("", GUILayout.Width(50f));

				bool fixFolders = GUILayout.Button("Fix Folders", GUILayout.Width(100f));
				if (fixFolders)
				{
					fixAtlasFolders(ref editorSelectedAtlas);
				}
				
				GUILayout.EndHorizontal();
			}
			
			EditorGUI.EndDisabledGroup();
		}
		
		{
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.Space();
			GUILayout.Label("Utilities", EditorStyles.boldLabel);
			EditorGUILayout.Space();

			EditorGUI.BeginDisabledGroup(retinaProState.state != retinaProState.rpState.kWaiting);

			GUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.Width(50f));
			GUILayout.Label("This will fix all missing folders for all atlases.", GUILayout.Width(260f));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.Width(50f));

			bool fixAllFolders = GUILayout.Button("Fix ALL Folder Issues", GUILayout.Width(200f));
			if (fixAllFolders)
			{
				for (int i=0; i<retinaProDataSerialize.sharedInstance.atlasList.Count; i++)
				{
					retinaProAtlas rpd = retinaProDataSerialize.sharedInstance.atlasList[i];
				
					bool foldersValid = showFoldersUI(ref rpd);
					
					if (!foldersValid)
					{
						fixAtlasFolders(ref rpd);
					}
				}
				
			}
			
			GUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			GUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.Width(50f));
			GUILayout.Label("When an atlas is refreshed this setting changes", GUILayout.Width(260f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.Width(50f));
			GUILayout.Label("the texture import settings on all of the source", GUILayout.Width(260f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.Width(50f));
			GUILayout.Label("textures. It makes sure they are set to the", GUILayout.Width(260f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.Width(50f));
			GUILayout.Label("maximum size and not compressed.", GUILayout.Width(260f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.Width(50f));
			GUILayout.Label("This ensures that the created atlas textures are", GUILayout.Width(260f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.Width(50f));
			GUILayout.Label("the correct size and quality for being rendered", GUILayout.Width(260f));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.Width(50f));
			GUILayout.Label("pixel perfect.", GUILayout.Width(260f));
			GUILayout.EndHorizontal();

			EditorGUILayout.Space();

			GUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.Width(50f));
			bool refreshSourceTextures = GUILayout.Toggle(retinaProDataSerialize.sharedInstance.getUtilityRefreshSourceTextures(), "Refresh Source Textures (Recommended)", GUILayout.Width(260f));
			if (refreshSourceTextures != retinaProDataSerialize.sharedInstance.getUtilityRefreshSourceTextures())
			{
				retinaProDataSerialize.sharedInstance.setUtilityRefreshSourceTextures(refreshSourceTextures);
				save = true;
			}
			GUILayout.EndHorizontal();
			
			
			EditorGUI.EndDisabledGroup();
		}
		
		if (save)
		{
			retinaProDataSerialize.sharedInstance.saveSettings();
		}
		
		if (refresh)
		{
			int idx = retinaProDataSerialize.sharedInstance.getPreviewDeviceIdx();
			retinaProDevice di = retinaProDataSerialize.sharedInstance.deviceList[idx];
			if (di.isDeviceValid())
			{
				retinaProNGTools.refreshReferencesForDevice(di, retinaProDataSerialize.sharedInstance.getPreviewScreenIdx(), retinaProDataSerialize.sharedInstance.getPreviewGameViewIdx());
				retinaProNGTools.refresh();
			}
		}
		
	}
	
	bool showFoldersUI(ref retinaProAtlas atlasItem)
	{
		if (atlasItem == null)
			return false;
		
		bool valid = true;
		
		for(int i=0; i<retinaProDataSerialize.sharedInstance.deviceList.Count; i++)
		{
			retinaProDevice di = retinaProDataSerialize.sharedInstance.deviceList[i];
			
			if (!di.isDeviceValid())
				continue;
			
			string reqPath = retinaProConfig.atlasTextureFolder + di.name + "/" + atlasItem.atlasName;
			
			DirectoryInfo dinfo = new DirectoryInfo(retinaProFileLock.baseDataPath + reqPath);
			if (dinfo != null && dinfo.Exists)
				continue;
			
			valid = false;

			GUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.Width(50f));

			Color col = GUI.color;
			GUI.color = new Color(1f, 0.4f, 0.4f, 1f);
			GUILayout.Label("Add folder: " + reqPath);
			GUI.color = col;
			
			GUILayout.EndHorizontal();
		}
		
		return valid;
	}
	
	void fixAtlasFolders(ref retinaProAtlas atlasItem)
	{
		for(int i=0; i<retinaProDataSerialize.sharedInstance.deviceList.Count; i++)
		{
			retinaProDevice di = retinaProDataSerialize.sharedInstance.deviceList[i];
			
			if (!di.isDeviceValid())
				continue;
			
			string reqPath = retinaProConfig.atlasTextureFolder + di.name + "/" + atlasItem.atlasName;
			
			DirectoryInfo dinfo = new DirectoryInfo(retinaProFileLock.baseDataPath + reqPath);
			if (dinfo == null || !dinfo.Exists)
			{
				dinfo.Create();
			}
		}
	}
	
	bool showFilesUI(ref retinaProAtlas atlasItem)
	{
		if (atlasItem.atlasName == null || atlasItem.atlasName.Length == 0)
			return false;

		bool 				valid = true;
		List<FileInfo>		fisLast = null;
		FileInfo [] 		fontImageFilesLast = null;
		FileInfo [] 		fontTxtFilesLast = null;
		
		for(int i=0; i<retinaProDataSerialize.sharedInstance.deviceList.Count; i++)
		{
			retinaProDevice di = retinaProDataSerialize.sharedInstance.deviceList[i];
			
			if (!di.isDeviceValid())
				continue;
			
			string reqPath = retinaProConfig.atlasTextureFolder + di.name + "/" + atlasItem.atlasName;
			
			DirectoryInfo dinfo = new DirectoryInfo(retinaProFileLock.baseDataPath + reqPath);
			if (dinfo == null || !dinfo.Exists)
				continue;
			
			if (atlasItem.isFont)
			{
				FileInfo [] fontImageFiles = dinfo.GetFiles("*.png");
				FileInfo [] fontTxtFiles = dinfo.GetFiles("*.txt");
				
				if (fontImageFiles == null || fontImageFiles.Length == 0)
				{
					valid = false;
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("", GUILayout.Width(50f));
		
					Color col = GUI.color;
					GUI.color = new Color(1f, 0.4f, 0.4f, 1f);
					GUI.skin.button.wordWrap = true;
					GUILayout.Label("Add a single <fontname>.png: " + reqPath);
					GUI.skin.button.wordWrap = false;
					GUI.color = col;
					
					GUILayout.EndHorizontal();
				}
				
				if (fontTxtFiles == null || fontTxtFiles.Length == 0)
				{
					valid = false;
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("", GUILayout.Width(50f));
		
					Color col = GUI.color;
					GUI.color = new Color(1f, 0.4f, 0.4f, 1f);
					GUI.skin.button.wordWrap = true;
					GUILayout.Label("Add a single <fontname>.txt: " + reqPath);
					GUI.skin.button.wordWrap = false;
					GUI.color = col;
					
					GUILayout.EndHorizontal();
				}
				
				if (!valid)
					continue;
				
				if (fontImageFiles.Length > 1)
				{
					valid = false;
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("", GUILayout.Width(50f));
		
					Color col = GUI.color;
					GUI.color = new Color(1f, 0.4f, 0.4f, 1f);
					GUI.skin.button.wordWrap = true;
					GUILayout.Label("Only supports single <font>.png: " + reqPath);
					GUI.skin.button.wordWrap = false;
					GUI.color = col;
					
					GUILayout.EndHorizontal();
				}
				
				if (fontTxtFiles.Length > 1)
				{
					valid = false;
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("", GUILayout.Width(50f));
		
					Color col = GUI.color;
					GUI.color = new Color(1f, 0.4f, 0.4f, 1f);
					GUI.skin.button.wordWrap = true;
					GUILayout.Label("Only supports single <font>.txt: " + reqPath);
					GUI.skin.button.wordWrap = false;
					GUI.color = col;
					
					GUILayout.EndHorizontal();
				}
				
				if (!valid)
					continue;
				
				string fontImageName = Path.GetFileNameWithoutExtension(fontImageFiles[0].Name);
				string fontTxtName = Path.GetFileNameWithoutExtension(fontTxtFiles[0].Name);
				
				if (fontImageName.CompareTo(fontTxtName) != 0)
				{
					valid = false;
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("", GUILayout.Width(50f));
		
					Color col = GUI.color;
					GUI.color = new Color(1f, 0.4f, 0.4f, 1f);
					GUI.skin.button.wordWrap = true;
					GUILayout.Label(".png / .txt filenames need to match: " + reqPath);
					GUI.skin.button.wordWrap = false;
					GUI.color = col;
					
					GUILayout.EndHorizontal();
				}
				
				if (!valid)
					continue;
				
				
				if (fontImageFilesLast != null && fontTxtFilesLast != null)
				{
					if (fontImageFiles[0].Name.CompareTo(fontImageFilesLast[0].Name) != 0)
					{
						// font image names don't match across devices
						valid = false;
						
						GUILayout.BeginHorizontal();
						GUILayout.Label("", GUILayout.Width(50f));
			
						Color col = GUI.color;
						GUI.color = new Color(1f, 0.4f, 0.4f, 1f);
						GUILayout.Label("<font>.png files mismatch: " + reqPath);
						GUI.color = col;
						
						GUILayout.EndHorizontal();
						
					}
		
					if (fontTxtFiles[0].Name.CompareTo(fontTxtFilesLast[0].Name) != 0)
					{
						// font txt names don't match across devices
						valid = false;
						
						GUILayout.BeginHorizontal();
						GUILayout.Label("", GUILayout.Width(50f));
			
						Color col = GUI.color;
						GUI.color = new Color(1f, 0.4f, 0.4f, 1f);
						GUILayout.Label("<font>.txt files mismatch: " + reqPath);
						GUI.color = col;
						
						GUILayout.EndHorizontal();
						
					}

					if (!valid)
						continue;
				}

				fontImageFilesLast = (FileInfo []) fontImageFiles.Clone();
				fontTxtFilesLast = (FileInfo []) fontTxtFiles.Clone();
			}
			else
			{
				List<FileInfo> fis;
				retinaProConfig.getValidArtFiles(dinfo, out fis);
				if (fis == null || fis.Count == 0)
				{
					valid = false;
					
					GUILayout.BeginHorizontal();
					GUILayout.Label("", GUILayout.Width(50f));
		
					Color col = GUI.color;
					GUI.color = new Color(1f, 0.4f, 0.4f, 1f);
					GUI.skin.button.wordWrap = true;
					GUILayout.Label("Add art assets to: " + reqPath);
					GUI.skin.button.wordWrap = false;
					GUI.color = col;
					
					GUILayout.EndHorizontal();
					
					continue;
				}
				
				if (fisLast != null)
				{
					// are they the same length?
					if (fis.Count != fisLast.Count)
					{
						valid = false;
						
						GUILayout.BeginHorizontal();
						GUILayout.Label("", GUILayout.Width(50f));
			
						Color col = GUI.color;
						GUI.color = new Color(1f, 0.4f, 0.4f, 1f);
						GUILayout.Label("Art asset mismatch: " + reqPath);
						GUI.color = col;
						
						GUILayout.EndHorizontal();
						
						continue;
					}
					
					// compare this file list to previous file list
					// if they don't match, reject it
					for (int f=0; f<fis.Count; f++)
					{
						if (fis[f].Name.CompareTo(fisLast[f].Name) != 0)
						{
							valid = false;
							
							GUILayout.BeginHorizontal();
							GUILayout.Label("", GUILayout.Width(50f));
				
							Color col = GUI.color;
							GUI.color = new Color(1f, 0.4f, 0.4f, 1f);
							GUILayout.Label("Art asset mismatch: " + reqPath);
							GUI.color = col;
							
							GUILayout.EndHorizontal();
							
							continue;
						}
					}
					
				}

				if (fis != null)
					fisLast = new List<FileInfo>(fis);				
				
				if (fis != null)
				{
					fis.Clear();
					fis = null;
				}
			}
			
			
		}
		
		if (fisLast != null)
		{
			fisLast.Clear();
			fisLast = null;
		}
		
		
		fontImageFilesLast = null;
		fontTxtFilesLast = null;
		
		
		return valid;
	}
	
	bool isAtlasExist(string atlasName)
	{
		DirectoryInfo dinfo = new DirectoryInfo(retinaProFileLock.baseDataPath + retinaProConfig.atlasResourceFolder);
		if (dinfo == null || !dinfo.Exists)
		  return false;
		  
		FileInfo [] fis = dinfo.GetFiles("*.prefab");
		
		foreach(FileInfo fi in fis)
		{
			string filename = Path.GetFileNameWithoutExtension(fi.Name);
			if (filename.CompareTo(atlasName) == 0)
			{
				return true;
			}
		}
		
		return false;
	}
	
	void refreshProgressPeriod()
	{
		progressPeriod = 0.0f;
		
		if (retinaProDataSerialize.sharedInstance.deviceList.Count > 0)
		{
			progressPeriod += ((float)deviceIndex) / ((float)retinaProDataSerialize.sharedInstance.deviceList.Count);
			progressPeriod += (progressPortion * (1.0f / ((float)retinaProDataSerialize.sharedInstance.deviceList.Count)));
		}

		progressPeriod = Mathf.Clamp01(progressPeriod);
	}
	
}
