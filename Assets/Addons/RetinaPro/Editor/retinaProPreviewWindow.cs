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
public class retinaProPreviewWindow : EditorWindow {
	
	
	[MenuItem ("Window/RetinaPro/Preview")]
    static void Init () 
	{
		retinaProPreviewWindow w = (retinaProPreviewWindow) EditorWindow.GetWindow (typeof (retinaProPreviewWindow));
		w.title = "Preview";
    }
	
	void OnEnable()
	{
		hideFlags = HideFlags.HideAndDontSave;
	}
	
	void OnDisable()
	{
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
			showEditorPreviewUI();
		}
		else
		{
			retinaProConfig.showSetupUI();
		}
	}
	
	void showEditorPreviewUI()
	{
		bool save = false;
		bool refresh = false;
		
		EditorGUI.BeginDisabledGroup(retinaProState.state != retinaProState.rpState.kWaiting);
		EditorGUILayout.Space();
		GUILayout.Label("Editor Preview", EditorStyles.boldLabel);
		EditorGUILayout.Space();
		
		string [] availableDevices = new string[retinaProDataSerialize.sharedInstance.deviceList.Count];
		for (int i=0; i<retinaProDataSerialize.sharedInstance.deviceList.Count; i++)
		{
			if (retinaProDataSerialize.sharedInstance.deviceList[i].isDeviceValid())
			{
				availableDevices[i] = retinaProDataSerialize.sharedInstance.deviceList[i].name;
			}
			else
			{
				availableDevices[i] = "";
			}
		}
		
		bool showRefreshButton = true;
		
		if (availableDevices.Length == 0)
		{
			retinaProConfig.showValidDeviceNeededUI();
			showRefreshButton = false;
		}
		else if (retinaProDataSerialize.sharedInstance.atlasList.Count == 0)
		{
			GUILayout.Label("Add at least one atlas");
			showRefreshButton = false;
		}
		else if (retinaProDataSerialize.sharedInstance.getPreviewDeviceIdx() >= availableDevices.Length)
		{
			int idx = retinaProDataSerialize.sharedInstance.getPreviewDeviceIdx();
			idx %= availableDevices.Length;
			retinaProDataSerialize.sharedInstance.setPreviewDeviceIdx(idx);

			// refresh the atlas references
			refresh = true;
			save = true;
		}
		else
		{
			int currentDeviceIdx = retinaProDataSerialize.sharedInstance.getPreviewDeviceIdx();
			int newDeviceIdx = EditorGUILayout.Popup(currentDeviceIdx, availableDevices, GUILayout.Width(150f));
			if (currentDeviceIdx != newDeviceIdx)
			{
				retinaProDataSerialize.sharedInstance.setPreviewDeviceIdx(newDeviceIdx);
				
				// refresh the atlas references
				refresh = true;
				save = true;
			}
			
			retinaProDevice di = retinaProDataSerialize.sharedInstance.deviceList[retinaProDataSerialize.sharedInstance.getPreviewDeviceIdx()];
			if (di.isDeviceValid())
			{
				if (di.screens != null && di.screens.Count > 0)
				{
					string [] screens = new string [di.screens.Count];
					for(int rsi=0; rsi<di.screens.Count; rsi++)
					{
						retinaProScreen rps = di.screens[rsi];
						
						screens[rsi] = "" + rps.width + " x " + rps.height;
					}
					
					int currentScreenViewIdx = retinaProDataSerialize.sharedInstance.getPreviewScreenIdx();
					int newScreenViewIdx = EditorGUILayout.Popup(currentScreenViewIdx, screens, GUILayout.Width(150f));
					if (newScreenViewIdx != currentScreenViewIdx)
					{
						retinaProDataSerialize.sharedInstance.setPreviewScreenIdx(newScreenViewIdx);

						// refresh the atlas references
						refresh = true;
						save = true;
					}
					
					{
						retinaProScreen rps = di.screens[retinaProDataSerialize.sharedInstance.getPreviewScreenIdx()];
						if (rps.useForBothLandscapePortrait)
						{
							string [] gameViews = { "Portrait", "Landscape" };
							
							int currentGameViewIdx = retinaProDataSerialize.sharedInstance.getPreviewGameViewIdx();
							int newGameViewIdx = EditorGUILayout.Popup(currentGameViewIdx, gameViews, GUILayout.Width(150f));
							
							if (currentGameViewIdx != newGameViewIdx)
							{
								retinaProDataSerialize.sharedInstance.setPreviewGameViewIdx(newGameViewIdx);
							
								// refresh the atlas references
								refresh = true;
								save = true;
							}
						}
					}
					
					
					
				}
				
			}
		}
		
		if (showRefreshButton)
		{
			bool pressed = GUILayout.Button("Refresh Atlas References", GUILayout.Width(150f));
			if (pressed)
			{
				refresh = true;
			}
		}
		
		EditorGUI.EndDisabledGroup();
		
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
	
	
}
