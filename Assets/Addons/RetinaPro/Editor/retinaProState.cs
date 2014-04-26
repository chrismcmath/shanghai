using UnityEngine;
using System.Collections;

public class retinaProState
{
	public enum rpState
	{
		kWaiting,
		kGen,
		kAtlas,
		kAtlasProcess,
		kFont,
		kDone,
		
	}
	
	public static rpState			state;
}
