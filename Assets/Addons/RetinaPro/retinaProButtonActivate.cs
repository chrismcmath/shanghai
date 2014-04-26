//-------------------------------------------------------------------------
// RetinaPro for NGUI
// © oeFun, Inc. 2012-2013
// http://oefun.com
// 
// NGUI and Tasharen are trademarks and copyright of Tasharen Entertainment
//-------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

// script that activates or deactivates an object and it's children when it receives OnClick()
// this is similar to NGUI's UIButtonActivate, however this script calls MakePixelPerfect
// on all the widgets, ensuring that they are displayed at the correct size. Additionally,
// this script refreshes the colliders to make sure they match the new scale

public class retinaProButtonActivate : MonoBehaviour {

	public GameObject target;
	public bool state = true;

	void OnClick ()
	{
		if (target != null)
		{
			NGUITools.SetActive(target, state); 

			// we're activating the target object
			if (state == true)
			{
				retinaProUtil.sharedInstance.refreshAllWidgetsPixelPerfect(target);
			}
		}
	}
}
