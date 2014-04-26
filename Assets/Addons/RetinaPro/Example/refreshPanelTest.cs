using UnityEngine;
using System.Collections;

public class refreshPanelTest : MonoBehaviour {
	
	public UIPanel	panel;
	
	ScreenOrientation lastOrient;

	// Use this for initialization
	void Start () {
		
		lastOrient = ScreenOrientation.Unknown;

		retinaProUtil.sharedInstance.refreshAllWidgetsPixelPerfect(panel.gameObject);		// fix up the visible and collider size
	}
	
	// Update is called once per frame
	void Update () {
		
		refreshOrientation();
	}
	
	void refreshOrientation()
	{
		if (lastOrient != Screen.orientation)
		{
			Debug.Log("Orientation changed to: " + Screen.orientation);
			
			// set the active device based on this new screen resolution / orientation
			retinaProAtlasController.sharedInstance.refreshActiveDevice();
			
			if (Screen.orientation == ScreenOrientation.Landscape)
			{
				// handle landscape version here...
				// turn on / off panels etc.
				

				
			}
			else if (Screen.orientation == ScreenOrientation.Portrait)
			{
				// handle portrait version here...
				// turn on / off panels etc.
				

				
			}
			
			// refresh the panel
			retinaProUtil.sharedInstance.refreshAllWidgetsPixelPerfect(panel.gameObject);		// fix up the visible and collider size

			lastOrient = Screen.orientation;
		}
	}
	
}
