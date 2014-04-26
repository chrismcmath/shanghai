//-------------------------------------------------------------------------
// RetinaPro for NGUI
// © oeFun, Inc. 2012-2013
// http://oefun.com
// 
// NGUI and Tasharen are trademarks and copyright of Tasharen Entertainment
//-------------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


[System.Serializable]
public class retinaProDeviceWindow : EditorWindow {
	
	[MenuItem ("Window/RetinaPro/Devices")]
    static void Init () 
	{
		retinaProDeviceWindow w = (retinaProDeviceWindow) EditorWindow.GetWindow (typeof (retinaProDeviceWindow));
		w.title = "Devices";
    }
	
	void OnEnable()
	{
		hideFlags = HideFlags.HideAndDontSave;
	}
	
	void OnDisable()
	{
		AssetDatabase.Refresh();
	}
	
	void OnDestroy()
	{
	}
	
	void OnGUI () 
	{
		bool setupComplete = retinaProConfig.isSetupComplete();
		
		if (setupComplete)
		{
			retinaProDataSerialize.sharedInstance.loadSettings();
			showDeviceUI();
		}
		else
		{
			retinaProConfig.showSetupUI();
		}
	}
	
	void showDeviceUI()
	{
		bool save = false;
		
		EditorGUI.BeginDisabledGroup(retinaProState.state != retinaProState.rpState.kWaiting);
		EditorGUILayout.Space();
		GUILayout.Label("Devices", EditorStyles.boldLabel);
		EditorGUILayout.Space();
		EditorGUI.EndDisabledGroup();

		// begin scrolling section which contains all of our device list items
		GUILayout.BeginVertical();
		
		Vector2 deviceScrollPos = Vector2.zero;
		deviceScrollPos.x = EditorPrefs.GetFloat("deviceScrollPosX", 0.0f);
		deviceScrollPos.y = EditorPrefs.GetFloat("deviceScrollPosY", 0.0f);

		deviceScrollPos = GUILayout.BeginScrollView(deviceScrollPos, false, false);

		EditorPrefs.SetFloat("deviceScrollPosX", deviceScrollPos.x);
		EditorPrefs.SetFloat("deviceScrollPosY", deviceScrollPos.y);

		EditorGUI.BeginDisabledGroup(retinaProState.state != retinaProState.rpState.kWaiting);

		// show each device and it's configuration
		if (retinaProDataSerialize.sharedInstance.deviceList != null)
		{
			for (int i=0; i<retinaProDataSerialize.sharedInstance.deviceList.Count; i++)
			{
				retinaProDevice rpd = retinaProDataSerialize.sharedInstance.deviceList[i];
				
				GUILayout.BeginHorizontal();

				GUILayout.Label("Name:", GUILayout.Width(60f));
				if (rpd != null)
				{
					string n = GUILayout.TextField(rpd.name, GUILayout.MaxWidth(150f));
					if (n.CompareTo(rpd.name) != 0)
					{
						rpd.name = n;
						save = true;
					}
				}

//				EditorGUILayout.Space();

				bool removeDevice = GUILayout.Button("Remove", GUILayout.Width(60f));
				if (removeDevice)
				{
					retinaProDataSerialize.sharedInstance.deviceList.RemoveAt(i);
					save = true;
					break;
				}
				
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Pixel Size:", GUILayout.Width(60f));
				if (rpd != null)
				{
					float p = EditorGUILayout.FloatField(rpd.pixelSize, GUILayout.Width(60f));
					if (p != rpd.pixelSize)
					{
						rpd.pixelSize = p;
						save = true;
					}
				}
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("UIRoot:", GUILayout.Width(60f));
				if (rpd != null)
				{
					if (!rpd.rootAuto)
					{
						{
							int rw = EditorGUILayout.IntField(rpd.rootWidth, GUILayout.Width(60f));
							if (rw != rpd.rootWidth)
							{
								rpd.rootWidth = rw;
								save = true;
							}
						}

						GUILayout.Label("X", GUILayout.Width(15f));

						{
							int rh = EditorGUILayout.IntField(rpd.rootHeight, GUILayout.Width(60f));
							if (rh != rpd.rootHeight)
							{
								rpd.rootHeight = rh;
								save = true;
							}
						}

						{
							bool useForBothLandscapePortrait = GUILayout.Toggle(rpd.rootUseBothPortLand, "Port & Land", GUILayout.Width(80f));
							if (useForBothLandscapePortrait != rpd.rootUseBothPortLand)
							{
								rpd.rootUseBothPortLand = useForBothLandscapePortrait;
								save = true;
							}
						}
					}
					
					bool autoRoot = false;
					if (rpd.rootAuto)
					{
						autoRoot = GUILayout.Toggle(rpd.rootAuto, "Auto (sets UIRoot manual height based on screen size)", GUILayout.Width(330f));
					}
					else
					{
						autoRoot = GUILayout.Toggle(rpd.rootAuto, "Auto", GUILayout.Width(50f));
					}
					

					if (autoRoot != rpd.rootAuto)
					{
						rpd.rootAuto = autoRoot;
						save = true;
					}

				}
				GUILayout.EndHorizontal();

				if (rpd.screens != null)
				{
					for (int rsi = 0; rsi<rpd.screens.Count; rsi++)
					{
						retinaProScreen rps = rpd.screens[rsi];

						if (rps != null)
						{
							GUILayout.BeginHorizontal();
							GUILayout.Label("Screen:", GUILayout.Width(60f));

							{
								int w = EditorGUILayout.IntField(rps.width, GUILayout.Width(60f));
								if (w != rps.width)
								{
									rps.width = w;
									save = true;
								}
							}

							GUILayout.Label("X", GUILayout.Width(15f));

							{
								int h = EditorGUILayout.IntField(rps.height, GUILayout.Width(60f));
								if (h != rps.height)
								{
									rps.height = h;
									save = true;
								}
							}
							
							{
								bool useForBothLandscapePortrait = GUILayout.Toggle(rps.useForBothLandscapePortrait, "Port & Land", GUILayout.Width(80f));
								if (useForBothLandscapePortrait != rps.useForBothLandscapePortrait)
								{
									rps.useForBothLandscapePortrait = useForBothLandscapePortrait;
									save = true;
								}
							}
							
							{
								bool pressed = GUILayout.Button("X", GUILayout.Width(20f));
								if (pressed)
								{
									rpd.screens.RemoveAt(rsi);
									save = true;
									break;
								}
							}
							
							GUILayout.EndHorizontal();
						}
		
						
					}
				}
				
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label("", GUILayout.Width(60f));
					
					bool pressed = GUILayout.Button("Add Screen", GUILayout.Width(85f));
					if (pressed)
					{
						retinaProScreen newScreen = new retinaProScreen();
						rpd.screens.Add(newScreen);
						save = true;
					}
					GUILayout.EndHorizontal();
				}
				
				
				
				

				EditorGUILayout.Space();
				EditorGUILayout.Space();
			}
		}
		
		
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.Width(60f));
			
			bool pressed = GUILayout.Button("Add Device", GUILayout.Width(100f));
			if (pressed)
			{
				retinaProDevice newItem = new retinaProDevice();
				retinaProDataSerialize.sharedInstance.deviceList.Add(newItem);
				save = true;
			}
			GUILayout.EndHorizontal();
		}

		EditorGUI.EndDisabledGroup();

		GUILayout.EndScrollView();
		GUILayout.EndVertical();

		if (save)
		{
			retinaProDataSerialize.sharedInstance.saveSettings();
		}
	}
	
}
