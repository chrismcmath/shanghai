//-------------------------------------------------------------------------
// RetinaPro for NGUI
// © oeFun, Inc. 2012-2013
// http://oefun.com
// 
// NGUI and Tasharen are trademarks and copyright of Tasharen Entertainment
//-------------------------------------------------------------------------

using UnityEngine;
using System;
using System.Collections;

public class retinaProUtil : MonoBehaviour {
	
	public static 				retinaProUtil 		sharedInstance = null;
	
	void Awake()
	{
		sharedInstance = this;
	}

	public void refreshAllWidgetsPixelPerfect(GameObject go)
	{
		if (!retinaProAtlasController.sharedInstance.isRuntimeDeviceLoaded())
		{
			Debug.LogWarning("retinaProAtlasController did not load properly.");
			return;
		}
		
		// fix up the UIRoot (needed if the screen resolution changed / orientation)
		retinaProAtlasController.sharedInstance.fixRoot();
		
		// make sure each widget that uses an atlas or font atlas is pointing to the correct device specific atlas
		{
			UIWidget [] widgets = go.GetComponentsInChildren<UIWidget>();
			if (widgets != null)
			{
				foreach(UIWidget w in widgets)
				{
					GameObject goAtlas = null;
					System.Type classType = w.GetType();
					
					if (classType == typeof(UISprite)) {
						goAtlas = ((UISprite)w).atlas.gameObject;
					}
					else if (classType == typeof(UILabel)) {
						goAtlas = ((UILabel)w).bitmapFont.gameObject;
					}
					
					if (goAtlas != null)
						retinaProAtlasController.sharedInstance.fixAtlasOrFont(goAtlas);
				}
			}
		}

		refreshVisible(go);
		StartCoroutine(refreshColliders(go));
	}
	
	IEnumerator refreshColliders(GameObject go)
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		fixColliders(go);

		yield return null;
	}

	public void refreshVisible(GameObject go)
	{
		if (go == null)
			return;
		
		UIPanel panel = go.GetComponent<UIPanel>();
		if (panel != null)
		{
			NGUITools.MarkParentAsChanged(go);
			go.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);		
		}

		// gather all of the widgets in this panel
		{
			UIWidget widget = go.GetComponent<UIWidget>();
			if (widget != null)
			{
				widget.MakePixelPerfect();
				widget.MarkAsChanged();
			}
		}
		
		UIWidget [] widgets = go.GetComponentsInChildren<UIWidget>();
		if (widgets != null)
		{
			foreach(UIWidget w in widgets)
			{
				// fix the transform scale so this object appears "pixel perfect"
				w.MakePixelPerfect();
				w.MarkAsChanged();
			}
		}
		
		if (panel != null)
		{
			panel.Refresh();
			go.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);		
			NGUITools.MarkParentAsChanged(go);
		}
	}
	
	void fixColliders(GameObject go)
	{
		if (go == null)
			return;
		
		UIPanel panel = go.GetComponent<UIPanel>();

		// update all box colliders
		{
			BoxCollider bc = go.GetComponent<BoxCollider>();
			if (bc != null)
			{
				NGUITools.AddWidgetCollider(bc.gameObject);
			}
		}

		BoxCollider [] colls = go.GetComponentsInChildren<BoxCollider>();
		
		if (colls != null)
		{
			foreach(BoxCollider bc in colls)
			{
				// refresh the box collider to match the visible size
				NGUITools.AddWidgetCollider(bc.gameObject);
			}
		}

		if (panel != null)
		{
			go.BroadcastMessage("CreatePanel", SendMessageOptions.DontRequireReceiver);		
			panel.Refresh();
		}
	}
	
 	[Obsolete("Use retinaProUtil.sharedInstance.refreshAllWidgetsPixelPerfect(myGameObject) instead")]
	public static void refreshWidgetPixelPerfect(GameObject go)
	{
		if (go == null)
			return;
		
		// gather all of the widgets in this panel
		UIWidget [] widgets = go.GetComponentsInChildren<UIWidget>();
		
		if (widgets != null)
		{
			foreach(UIWidget w in widgets)
			{
				// fix the transform scale so this object appears "pixel perfect"
				w.MakePixelPerfect();
			}
		}
		
		// gather all of the box colliders
		BoxCollider [] colls = go.GetComponentsInChildren<BoxCollider>();
		
		if (colls != null)
		{
			foreach(BoxCollider bc in colls)
			{
				// refresh the box collider to match the visible size
				NGUITools.AddWidgetCollider(bc.gameObject);
			}
		}
	}
}
