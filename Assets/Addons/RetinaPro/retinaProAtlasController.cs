//-------------------------------------------------------------------------
// RetinaPro for NGUI
// © oeFun, Inc. 2012-2013
// http://oefun.com
// 
// NGUI and Tasharen are trademarks and copyright of Tasharen Entertainment
//-------------------------------------------------------------------------

//#define RETINAPRO_ATLASCONTROLLER_DEBUGLOG

using UnityEngine;
using System.Collections;

public class retinaProAtlasController : MonoBehaviour {
	
	public static retinaProAtlasController sharedInstance;
	public bool autoRefreshOnStartup = true;
	
	retinaProDataRuntime		rt;
	int							activeDeviceIdx;
	int 						activeScreenIdx;

	void Awake()
	{
		sharedInstance = this;
		
		rt = new retinaProDataRuntime();
		if (!rt.loaded)
		{
			Debug.LogWarning("Could not load retinaPro data.");
			return;
		}
		
		refreshActiveDevice();
		fixRoot();
	}
	
	// Use this for initialization
	void Start () {
		
		if (autoRefreshOnStartup)
			fixAll();
	}
	
	public void refreshAll()
	{
		refreshActiveDevice();
		fixAll();
	}
	
	void fixAll()
	{
		if (activeDeviceIdx == -1)
			return;
		
		if (activeScreenIdx == -1)
			return;

		fixRoot();
		fixAtlases();
		fixFonts();
		
		Resources.UnloadUnusedAssets();
		NGUITools.Broadcast("MakePixelPerfect");
		StartCoroutine(fixPixelPerfect());
	}
	
	public bool isRuntimeDeviceLoaded()
	{
		return rt.loaded;
	}
	
	void checkAspect(ref retinaProRuntimeScreen rs, float aspect, float deviceAspect, ref int i, ref int s, ref float bestAspectDiff, ref int bestIdx, ref int bestScreenIdx, ref int highestWidth)
	{
		float d = Mathf.Abs(aspect - deviceAspect);
		
		if (bestIdx == -1)
		{
			bestAspectDiff = d;
			bestIdx = i;
			bestScreenIdx = s;
			highestWidth = rs.width;
		}
		else
		{
			if (d < bestAspectDiff && rs.width >= highestWidth)
			{
				bestAspectDiff = d;
				bestIdx = i;
				bestScreenIdx = s;
				highestWidth = rs.width;
			}
		}
	}
	
	public void refreshActiveDevice()
	{
		activeDeviceIdx = -1;
		activeScreenIdx = -1;
		
		float aspect = ((float)Screen.width) / ((float)Screen.height);

#if RETINAPRO_ATLASCONTROLLER_DEBUGLOG
		Debug.Log("Screen Size (" + Screen.width + ", " + Screen.height + ")");
		Debug.Log("Screen aspect: " + aspect);
#endif

		// what resolution are we running at
		// compare against our devices
		
		for (int i=0; i<rt.deviceList.Count; i++)
		{
			retinaProRuntimeDevice dd = rt.deviceList[i];

			for (int s=0; s<dd.screens.Count; s++)
			{
				retinaProRuntimeScreen rs = dd.screens[s];
				
				if (rs.useForBothLandscapePortrait)
				{
					if (
							(rs.width == Screen.width && rs.height == Screen.height) ||
							(rs.width == Screen.height && rs.height == Screen.width)
						)
					{
						activeDeviceIdx = i;
						activeScreenIdx = s;
						break;
					}
				}
				else
				{
					if (rs.width == Screen.width && rs.height == Screen.height)
					{
						activeDeviceIdx = i;
						activeScreenIdx = s;
						break;
					}
				}
			}
		}
		
		if (activeDeviceIdx == -1)
		{
			// we did not find any devices that match the current screen resolution
			Debug.LogWarning("Did not find any devices that match the current resolution");
			
			int bestIdx = -1;
			int bestScreenIdx = -1;
			float bestAspectDiff = 0.0f;
			int highestWidth = 0;

			// check the aspect ratio and find the closest and highest resolution that matches that aspect ratio
			for (int i=0; i<rt.deviceList.Count; i++)
			{
				retinaProRuntimeDevice dd = rt.deviceList[i];
				
				for (int s=0; s<dd.screens.Count; s++)
				{
					retinaProRuntimeScreen rs = dd.screens[s];
					
					float deviceAspect = ((float)rs.width) / ((float)rs.height);
					checkAspect(ref rs, aspect, deviceAspect, ref i, ref s, ref bestAspectDiff, ref bestIdx, ref bestScreenIdx, ref highestWidth);
					
					if (rs.useForBothLandscapePortrait)
					{
						deviceAspect = ((float)rs.height) / ((float)rs.width);
						checkAspect(ref rs, aspect, deviceAspect, ref i, ref s, ref bestAspectDiff, ref bestIdx, ref bestScreenIdx, ref highestWidth);
					}
				}
			}
			
			activeDeviceIdx = bestIdx;
			activeScreenIdx = bestScreenIdx;
		}

		if (activeDeviceIdx != -1)
		{
			Debug.Log("Using device: " + rt.deviceList[activeDeviceIdx].name + ", pixel size = " + rt.deviceList[activeDeviceIdx].pixelSize);

			if (activeScreenIdx != -1)
				Debug.Log("Using screen: " +
					rt.deviceList[activeDeviceIdx].screens[activeScreenIdx].width + " X " +
					rt.deviceList[activeDeviceIdx].screens[activeScreenIdx].height + ", Port & Land = " +
					rt.deviceList[activeDeviceIdx].screens[activeScreenIdx].useForBothLandscapePortrait);
		}
	}
	
	public void fixRoot()
	{
		// get the height of the screen, adjusted for pixel size
		int width;
		int height;
		bool usePortLand;
		
		if (rt.deviceList[activeDeviceIdx].rootAuto)
		{
			width = rt.deviceList[activeDeviceIdx].screens[activeScreenIdx].width;
			height = rt.deviceList[activeDeviceIdx].screens[activeScreenIdx].height;
			usePortLand = rt.deviceList[activeDeviceIdx].screens[activeScreenIdx].useForBothLandscapePortrait;
		}
		else
		{
			width = rt.deviceList[activeDeviceIdx].rootWidth;
			height = rt.deviceList[activeDeviceIdx].rootHeight;
			usePortLand = rt.deviceList[activeDeviceIdx].rootUseBothPortLand;
		}
		
		int manualHeight = (int) (height * rt.deviceList[activeDeviceIdx].pixelSize);
		
		if (usePortLand)
		{
			float aspect = ((float)Screen.width) / ((float)Screen.height);
			float deviceAspect = ((float)rt.deviceList[activeDeviceIdx].screens[activeScreenIdx].width) / ((float)rt.deviceList[activeDeviceIdx].screens[activeScreenIdx].height);

			// if the screen is orientated differently from the device settings, calculate the UIRoot height based on the screen width
			float d = Mathf.Abs(aspect - deviceAspect);

			if (d > 0.01f)
			{
				manualHeight = (int) ( ((float)width) * rt.deviceList[activeDeviceIdx].pixelSize);
			}
		}
		
#if RETINAPRO_ATLASCONTROLLER_DEBUGLOG
		Debug.Log("Setting manual height to: " + manualHeight);
#endif		
		
		UIRoot [] roots = Resources.FindObjectsOfTypeAll(typeof(UIRoot)) as UIRoot[];
		if (roots == null || roots.Length == 0)
		{
			Debug.LogWarning("Could not find the UIRoot. Please add a UIRoot to your scene. The manual height will be set to :" + manualHeight);
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
	
	public void fixAtlases()
	{
		// get the currently loaded assets
		UIAtlas [] loadedAtlas = Resources.FindObjectsOfTypeAll(typeof(UIAtlas)) as UIAtlas[];

#if RETINAPRO_ATLASCONTROLLER_DEBUGLOG
		Debug.Log("number of loadedAtlas = " + loadedAtlas.Length);
#endif
		
		// fix up atlas references
        for(int i=0; i<loadedAtlas.Length; i++)
        {	
			UIAtlas atlasRef = loadedAtlas[i];
			fixAtlas(ref atlasRef);
		}
	}
	
	public void fixFonts()
	{
		// get the currently loaded assets
		UIFont [] loadedFont = Resources.FindObjectsOfTypeAll(typeof(UIFont)) as UIFont[];

		// fix up font references
        for(int i=0; i<loadedFont.Length; i++)
        {			
			UIFont fontRef = loadedFont[i];
			fixFont(ref fontRef);
		}
	}

	public void fixAtlas(ref UIAtlas atlasRef)
	{
#if RETINAPRO_ATLASCONTROLLER_DEBUGLOG
		Debug.Log("atlasRef name = " + atlasRef.name);
#endif
		// determine if this is the parent reference atlas or it's a device specific atlas
		retinaProParent parent = atlasRef.gameObject.GetComponent<retinaProParent>();
		if (parent == null)
			return;

		// get name of prefab we want to have this parent atlas reference
		retinaProRuntimeDevice d = rt.deviceList[activeDeviceIdx];
		string atlasName = atlasRef.name + "~" + d.name;
			
#if RETINAPRO_ATLASCONTROLLER_DEBUGLOG
		Debug.Log("Required Atlas = " + atlasName);
#endif
			
		// determine whether we should load an atlas / replace existing atlas reference
		bool replace = false;
		if (atlasRef.replacement == null)			
		{
			// this atlas isn't referencing anything, fix it
			replace = true;
		}
		else
		{
			// this atlas is referencing something, is that the one we want?
			if (atlasRef.replacement.name.CompareTo(atlasName) != 0)
			{
				// this atlas isn't referencing the atlas we want, fix it
				replace = true;
			}
		}
		
		// load the required atlas and update the parents atlas reference
		if (replace)
		{
#if RETINAPRO_ATLASCONTROLLER_DEBUGLOG
			Debug.Log("- loading");
#endif				
			string prefabName = retinaProDataRuntime.atlasFolder + d.name + "/" + atlasName;

			UIAtlas atlas = Resources.Load(prefabName, typeof(UIAtlas)) as UIAtlas;
			if (atlas != null)
			{
				atlasRef.replacement = atlas;
#if RETINAPRO_ATLASCONTROLLER_DEBUGLOG
				Debug.Log("- loaded");
#endif					
			}
			else
			{
				Debug.LogWarning("Could not load the atlas");
			}
		}
		else
		{
#if RETINAPRO_ATLASCONTROLLER_DEBUGLOG
			Debug.Log("- already loaded");
#endif					
		}
	}
	
	public void fixFont(ref UIFont fontRef)
	{
		// determine if this is the parent reference font or it's a device specific font
		retinaProParent parent = fontRef.gameObject.GetComponent<retinaProParent>();
		if (parent == null)
			return;

		// get name of prefab we want to have this parent font reference
		retinaProRuntimeDevice d = rt.deviceList[activeDeviceIdx];
		string fontName = fontRef.name + "~" + d.name;
			
#if RETINAPRO_ATLASCONTROLLER_DEBUGLOG
		Debug.Log("Required Font = " + fontName);
#endif
			
		// determine whether we should load a font / replace existing font reference
		bool replace = false;
		if (fontRef.replacement == null)			
		{
			// this font isn't referencing anything, fix it
			replace = true;
		}
		else
		{
			// this font is referencing something, is that the one we want?
			if (fontRef.replacement.name.CompareTo(fontName) != 0)
			{
				// this font isn't referencing the atlas we want, fix it
				replace = true;
			}
		}
		
		// load the required font and update the parents font reference
		if (replace)
		{
#if RETINAPRO_ATLASCONTROLLER_DEBUGLOG
			Debug.Log("- loading");
#endif				
			string prefabName = retinaProDataRuntime.atlasFolder + d.name + "/" + fontName;

			UIFont font = Resources.Load(prefabName, typeof(UIFont)) as UIFont;
			if (font != null)
			{
				fontRef.replacement = font;
#if RETINAPRO_ATLASCONTROLLER_DEBUGLOG
				Debug.Log("- loaded");
#endif					
			}
			else
			{
				Debug.LogWarning("Could not load the font");
			}
		}
		else
		{
#if RETINAPRO_ATLASCONTROLLER_DEBUGLOG
			Debug.Log("- already loaded");
#endif					
		}
	}

	public void fixAtlasOrFont(GameObject go)
	{
		UIAtlas atlas = go.GetComponent<UIAtlas>();
		if (atlas)
			fixAtlas(ref atlas);

		UIFont font = go.GetComponent<UIFont>();
		if (font) {
			fixFont(ref font);
		}
	}

	IEnumerator fixPixelPerfect()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		NGUITools.Broadcast("MakePixelPerfect");

		yield return null;
	}
}
