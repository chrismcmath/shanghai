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

public class retinaProNGTools : ScriptableObject {
	
	public static void refresh()
	{
		EditorApplication.delayCall += pixPerfect;
	}

	static void pixPerfect()
	{
		EditorApplication.delayCall += pixPerfect2;
	}
	
	static void pixPerfect2()
	{
		NGUITools.Broadcast("MakePixelPerfect");
	}
	
	public static void setRootManualHeight(int manualHeight)
	{
		UIRoot [] roots = Resources.FindObjectsOfTypeAll(typeof(UIRoot)) as UIRoot[];
		if (roots == null || roots.Length == 0)
		{
			Debug.LogWarning("Could not find the UIRoot. Please set the UIRoot manual height property to :" + manualHeight);
		}
		else
		{
			foreach(UIRoot root in roots)
			{
				retinaProIgnoreUIRoot ignore = root.gameObject.GetComponent<retinaProIgnoreUIRoot>();
				if (ignore != null && ignore.ignoreUIRoot)
					continue;

				root.scalingStyle = UIRoot.Scaling.FixedSize;
				root.manualHeight = manualHeight;
				root.minimumHeight = manualHeight;
				root.maximumHeight = manualHeight;
			}
		}
	}
	
	// gameViewOrientation, 0 = portrait, 1 = landscape (only used if the device is used for both landscape / portrait)
	// this is a work around because Screen.width / Screen.height returns bogus values
	// screenIndex is an index into the device screen list
	
	public static void refreshReferencesForDevice(retinaProDevice deviceItem, int screenIndex, int gameViewOrientation)
	{
		if (deviceItem == null)
			return;
		
		if (!deviceItem.isDeviceValid())
			return;
		
		if (screenIndex < 0 || screenIndex >= deviceItem.screens.Count)
			return;
		
		int manualHeight = 0;
		
		float width;
		float height;
		bool useBothPortLand;
		
		if (deviceItem.rootAuto)
		{
			retinaProScreen rps = deviceItem.screens[screenIndex];
			
			width = (float) rps.width;
			height = (float) rps.height;
			useBothPortLand = rps.useForBothLandscapePortrait;
		}
		else
		{
			width = (float) deviceItem.rootWidth;
			height = (float) deviceItem.rootHeight;
			useBothPortLand = deviceItem.rootUseBothPortLand;
		}
			
		if (useBothPortLand)
		{
			if (width >= height)		// landscape
			{
				if (gameViewOrientation == 1)
				{
					// screen game view is also landscape
					manualHeight = (int) (height * deviceItem.pixelSize);
				}
				else
				{
					// screen game view is opposite (portrait)
					manualHeight = (int) (width * deviceItem.pixelSize);
				}
			}
			else      													// portrait
			{
				if (gameViewOrientation == 1)
				{
					// screen game view is opposite (landscape)
					manualHeight = (int) (width * deviceItem.pixelSize);
				}
				else
				{
					// screen game view is also portrait
					manualHeight = (int) (height * deviceItem.pixelSize);
				}
			}
		}
		else
		{
			manualHeight = (int) (height * deviceItem.pixelSize);
		}

		retinaProNGTools.setRootManualHeight(manualHeight);
		
		{
			DirectoryInfo dinfo = new DirectoryInfo(retinaProFileLock.baseDataPath + retinaProConfig.atlasResourceFolder);
			if (dinfo != null && dinfo.Exists)
			{
                FileInfo [] fis = dinfo.GetFiles("*.prefab");
    
                foreach(FileInfo fi in fis)
                {
					GameObject prefab = (GameObject) AssetDatabase.LoadAssetAtPath("Assets" + retinaProConfig.atlasResourceFolder + fi.Name, typeof(GameObject));
					if (prefab == null)
						continue;
					
					UIAtlas atlas = prefab.GetComponent<UIAtlas>();
					if (atlas != null)
					{
	                    string newAR = "Assets" + retinaProConfig.atlasResourceFolder + deviceItem.name + "/" + Path.GetFileNameWithoutExtension(fi.Name) + "~" + deviceItem.name + ".prefab";

						UIAtlas ar = (UIAtlas) AssetDatabase.LoadAssetAtPath(newAR, typeof(UIAtlas));
	                    atlas.replacement = ar;
					}

					UIFont font = prefab.GetComponent<UIFont>();
					if (font != null)
					{
	                    string newFR = "Assets" + retinaProConfig.atlasResourceFolder + deviceItem.name + "/" + Path.GetFileNameWithoutExtension(fi.Name) + "~" + deviceItem.name + ".prefab";
	                    UIFont fr = (UIFont) AssetDatabase.LoadAssetAtPath(newFR, typeof(UIFont));
	                    font.replacement = fr;
					}
    
                }
            }
		}
		
	}
	
	public static void getAtlasSpriteData(ref UIAtlas atlas, ref List<UISpriteData> sprites)
	{
		foreach(UISpriteData sprite in atlas.spriteList)
		{
			UISpriteData newSpriteData = new UISpriteData();
			newSpriteData.CopyFrom(sprite);
			sprites.Add(newSpriteData);
		}
	}
	
	public static void updateAtlasSpriteData(ref UIAtlas atlas, ref List<UISpriteData> sprites)
	{
		foreach(UISpriteData sprite in sprites)
		{
			// does this old sprite data exist in the new atlas?
			foreach(UISpriteData newSprite in atlas.spriteList)
			{
				if (newSprite.name.CompareTo(sprite.name) == 0)
				{
					// found a match
					// update new sprite with old border data
					newSprite.borderLeft = sprite.borderLeft;
					newSprite.borderRight = sprite.borderRight;
					newSprite.borderTop = sprite.borderTop;
					newSprite.borderBottom = sprite.borderBottom;
				
					newSprite.paddingLeft = sprite.paddingLeft;
					newSprite.paddingRight = sprite.paddingRight;
					newSprite.paddingTop = sprite.paddingTop;
					newSprite.paddingBottom = sprite.paddingBottom;
				}
			}
		}
	}
	
	public static void debugAtlasSpriteData(ref UIAtlas atlas)
	{
		Debug.Log("sprite count in atlas (" + atlas.name + ") = " + atlas.spriteList.Count);
		foreach(UISpriteData sprite in atlas.spriteList)
		{
			Debug.Log("name = " + sprite.name);
		}
	}
	
	public static UIAtlas createAtlas(string atlasName, string device, out List<UISpriteData> oldSpriteData)
	{
		oldSpriteData = new List<UISpriteData>();

#if RETINAPRO_DEBUGLOG
		string debugString = "createAtlas; atlasName = " + atlasName;
		
		if (device != null && device.Length > 0)
		{
			debugString += ", device = " + device;
		}

		Debug.Log(debugString);
#endif

		string newAR = "";
		string newARMat = "";
		string folder = "";
		
		if (device == null || device.Length == 0)
		{
			newAR = "Assets" + retinaProConfig.atlasResourceFolder + atlasName + ".prefab";
			newARMat = "Assets" + retinaProConfig.atlasResourceFolder + atlasName + ".mat";
			folder = retinaProFileLock.baseDataPath + retinaProConfig.atlasResourceFolder;
		}
		else
		{
			newAR = "Assets" + retinaProConfig.atlasResourceFolder + device + "/" + atlasName + "~" + device + ".prefab";
			newARMat = "Assets" + retinaProConfig.atlasResourceFolder + device + "/" + atlasName + "~" + device + ".mat";
			folder = retinaProFileLock.baseDataPath + retinaProConfig.atlasResourceFolder + device;
		}
		
		DirectoryInfo dinfo = new DirectoryInfo(folder);
		if (!dinfo.Exists)
		{
#if RETINAPRO_DEBUGLOG
			Debug.Log("create folder: " + folder);
#endif
			dinfo.Create();
		}
		
		
#if RETINAPRO_DEBUGLOG
		Debug.Log("creating atlas assets: " + newAR);
#endif
		
		Material mat = AssetDatabase.LoadAssetAtPath(newARMat, typeof(Material)) as Material;
		if (mat == null)
		{
			Shader shader = Shader.Find("Unlit/Transparent Colored");
			if (shader != null)
			{
				mat = new Material(shader);

				AssetDatabase.CreateAsset(mat, newARMat);
				AssetDatabase.SaveAssets();

				mat = AssetDatabase.LoadAssetAtPath(newARMat, typeof(Material)) as Material;
			}
		}
		
		if (mat == null)
		{
			Debug.LogWarning("Error; Could not create material for atlas = " + atlasName);
			return null;
		}
		
		bool saveSpriteData = true;
		Object atlasPrefab = AssetDatabase.LoadAssetAtPath (newAR, typeof(GameObject));
		if (atlasPrefab == null)
		{
			atlasPrefab = PrefabUtility.CreateEmptyPrefab(newAR);
			saveSpriteData = false;	// no sprite data to save on a new atlas
		}
		
		if (atlasPrefab == null)
		{
			Debug.LogWarning("Error; Could not create atlas Prefab for atlas = " + atlasName);
			return null;
		}
		
		GameObject go = new GameObject(atlasName);
		go.AddComponent<UIAtlas>().spriteMaterial = mat;
		
		// save off any sprite data associated with this atlas
		if (saveSpriteData)
		{
			GameObject goOldAtlas = (GameObject) Instantiate(atlasPrefab);
			UIAtlas oldAtlas = goOldAtlas.GetComponent<UIAtlas>();
			retinaProNGTools.getAtlasSpriteData(ref oldAtlas, ref oldSpriteData);
			oldAtlas = null;
			DestroyImmediate(goOldAtlas);
		}
		
		PrefabUtility.ReplacePrefab(go, atlasPrefab, ReplacePrefabOptions.ReplaceNameBased);
		
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		// get the atlas
		GameObject go2 = AssetDatabase.LoadAssetAtPath(newAR, typeof(GameObject)) as GameObject;
		
		DestroyImmediate(go);
		EditorUtility.UnloadUnusedAssets();

		UIAtlas ret = go2.GetComponent<UIAtlas>();
		
#if RETINAPRO_DEBUGLOG
		Debug.Log("Finished creating atlas = " + ret);
#endif
		return ret;
	}
	
	public static UIFont createFont(string atlasName, string fontName, string device)
	{
#if RETINAPRO_DEBUGLOG
		string debugString = "createFont; fontName = " + atlasName;
		
		if (device != null && device.Length > 0)
		{
			debugString += ", device = " + device;
		}

		Debug.Log(debugString);
#endif

		string newFR = "";
		string newFRMat = "";
		string folder = "";
		
		if (device == null || device.Length == 0)
		{
			newFR = "Assets" + retinaProConfig.atlasResourceFolder + atlasName + ".prefab";
			newFRMat = "Assets" + retinaProConfig.atlasResourceFolder + atlasName + ".mat";
			folder = retinaProFileLock.baseDataPath + retinaProConfig.atlasResourceFolder;
		}
		else
		{
			newFR = "Assets" + retinaProConfig.atlasResourceFolder + device + "/" + atlasName + "~" + device + ".prefab";
			newFRMat = "Assets" + retinaProConfig.atlasResourceFolder + device + "/" + atlasName + "~" + device + ".mat";
			folder = retinaProFileLock.baseDataPath + retinaProConfig.atlasResourceFolder + device;
		}
		
		DirectoryInfo dinfo = new DirectoryInfo(folder);
		if (!dinfo.Exists)
		{
#if RETINAPRO_DEBUGLOG
			Debug.Log("create folder: " + folder);
#endif
			dinfo.Create();
		}

		Material mat = AssetDatabase.LoadAssetAtPath(newFRMat, typeof(Material)) as Material;
		if (mat == null)
		{
			Shader shader = Shader.Find("Unlit/Transparent Colored");
			if (shader != null)
			{
				mat = new Material(shader);

				AssetDatabase.CreateAsset(mat, newFRMat);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

				mat = AssetDatabase.LoadAssetAtPath(newFRMat, typeof(Material)) as Material;
			}
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		Object fontPrefab = AssetDatabase.LoadAssetAtPath (newFR, typeof(GameObject));
		if (fontPrefab == null)
			fontPrefab = PrefabUtility.CreateEmptyPrefab(newFR);

		GameObject go = new GameObject(atlasName);
		
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		UIFont font = go.AddComponent<UIFont>();
		font.atlas = null;
		font.spriteName = null;
			
		if (device != null && device.Length > 0)
		{
			string fontTextAssetName = "Assets" + retinaProConfig.atlasTextureFolder + device + "/" + atlasName + "/" + fontName + ".txt";
			string fontTextureName = "Assets" + retinaProConfig.atlasTextureFolder + device + "/" + atlasName + "/" + fontName + ".png";

			TextAsset fontData = AssetDatabase.LoadAssetAtPath(fontTextAssetName, typeof(TextAsset)) as TextAsset;
			Texture2D fontTexture = AssetDatabase.LoadAssetAtPath(fontTextureName, typeof(Texture2D)) as Texture2D;
			
			font.material = mat;
			
			BMFontReader.Load(font.bmFont, NGUITools.GetHierarchy(font.gameObject), fontData.bytes);
			
			mat.mainTexture = fontTexture;
		}
		
		PrefabUtility.ReplacePrefab(go, fontPrefab, ReplacePrefabOptions.ReplaceNameBased);
		
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		// get the font
		GameObject go2 = AssetDatabase.LoadAssetAtPath(newFR, typeof(GameObject)) as GameObject;
		
		DestroyImmediate(go);
		EditorUtility.UnloadUnusedAssets();

		UIFont ret = go2.GetComponent<UIFont>();

#if RETINAPRO_DEBUGLOG
		Debug.Log("Finished creating font = " + ret);
#endif
		return ret;
	}
	
	
}
