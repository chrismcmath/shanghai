//-------------------------------------------------------------------------
// RetinaPro for NGUI
// © oeFun, Inc. 2012-2013
// http://oefun.com
// 
// NGUI and Tasharen are trademarks and copyright of Tasharen Entertainment
//-------------------------------------------------------------------------

#define RETINAPRO_DEBUGLOG

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class retinaProRuntimeScreen
{
	public int 		width;
	public int 		height;
	public bool 	useForBothLandscapePortrait;
};

public class retinaProRuntimeDevice 
{
	public string 							name;
	public float 							pixelSize;
	public bool 							rootAuto;
	public int								rootHeight;
	public int 								rootWidth;
	public bool 							rootUseBothPortLand;
	public List<retinaProRuntimeScreen>		screens;
	
	public retinaProRuntimeDevice()
	{
		screens = new List<retinaProRuntimeScreen> ();
	}

};

public class retinaProRuntimeAtlas 
{
	public string name;
	public bool isFont;
};

public class retinaProDataRuntime
{
	public static string atlasFolder = "rp_Atlases/";
	public bool loaded;
	
	public List<retinaProRuntimeDevice> deviceList;
	public List<retinaProRuntimeAtlas> atlasList;
	

	public retinaProDataRuntime()
	{
		deviceList = new List<retinaProRuntimeDevice> ();
		atlasList = new List<retinaProRuntimeAtlas> ();
		
		loaded = loadSettings();
	}
	
	~retinaProDataRuntime()
	{
	}
	

	public bool loadSettings()
	{
		lock(retinaProFileLock.fileLock)
		{
			TextAsset data = Resources.Load("RetinaProData", typeof(TextAsset)) as TextAsset;
			if (data == null)
				return false;
			
			if (data.bytes == null)
				return false;
			
			if (data.bytes.Length <= 0)
				return false;
			
			// parse our data
			XMLParser par = new XMLParser();
			XMLNode nodes = par.Parse(data.text);
			
			if (nodes == null)
			{
				Debug.LogWarning("Could not parse retinaProData.txt");
				return false;
			}
			
			bool validDevice = loadDevices(ref nodes);
			if (!validDevice)
			{
				Debug.LogWarning("Could not parse devices");
				return false;
			}
			
			bool validAtlas = loadAtlases(ref nodes);
			if (!validAtlas)
			{
				Debug.LogWarning("Could not parse atlases");
				return false;
			}
		}
		
		return true;
	}
	
	bool loadDevices(ref XMLNode nodes)
	{
		XMLNodeList nodeList = nodes.GetNodeList("retinaProData>0>ArrayOfRetinaProDevice>0>retinaProDevice");
		if (nodeList == null)
			return false;
		
		foreach (XMLNode node in nodeList)
        {
			retinaProRuntimeDevice rd = new retinaProRuntimeDevice();

			foreach(DictionaryEntry pair in node)
			{
				if (pair.Key.ToString().CompareTo("name") == 0)
				{
					XMLNodeList vs = (XMLNodeList) pair.Value;
		 			foreach (XMLNode v in vs)
		            {
						foreach(DictionaryEntry dp in v)
						{
							if (dp.Key.ToString().CompareTo("_text") == 0)
							{
								rd.name = dp.Value.ToString();
							}
						}
					}
				}
				else if (pair.Key.ToString().CompareTo("pixelSize") == 0)
				{
					XMLNodeList vs = (XMLNodeList) pair.Value;
		 			foreach (XMLNode v in vs)
		            {
						foreach(DictionaryEntry dp in v)
						{
							if (dp.Key.ToString().CompareTo("_text") == 0)
							{
								string str = dp.Value.ToString();
								Single.TryParse(str, out rd.pixelSize);
							}
						}
					}
				}
				else if (pair.Key.ToString().CompareTo("screens") == 0)
				{
					
					XMLNodeList vs = (XMLNodeList) pair.Value;
		 			foreach (XMLNode v in vs)
		            {
						retinaProRuntimeScreen rs = new retinaProRuntimeScreen();
	
						foreach(DictionaryEntry dp in v)
						{
							if (dp.Key.ToString().CompareTo("width") == 0)
							{
								XMLNodeList vs2 = (XMLNodeList) dp.Value;
					 			foreach (XMLNode v2 in vs2)
					            {
									foreach(DictionaryEntry dp2 in v2)
									{
										if (dp2.Key.ToString().CompareTo("_text") == 0)
										{
											string str = dp2.Value.ToString();
											Int32.TryParse(str, out rs.width);
										}
									}
								}
								
							}
							else if (dp.Key.ToString().CompareTo("height") == 0)
							{
								XMLNodeList vs2 = (XMLNodeList) dp.Value;
					 			foreach (XMLNode v2 in vs2)
					            {
									foreach(DictionaryEntry dp2 in v2)
									{
										if (dp2.Key.ToString().CompareTo("_text") == 0)
										{
											string str = dp2.Value.ToString();
											Int32.TryParse(str, out rs.height);
										}
									}
								}
								
							}
							else if (dp.Key.ToString().CompareTo("useForBothLandscapePortrait") == 0)
							{
								XMLNodeList vs2 = (XMLNodeList) dp.Value;
					 			foreach (XMLNode v2 in vs2)
					            {
									foreach(DictionaryEntry dp2 in v2)
									{
										if (dp2.Key.ToString().CompareTo("_text") == 0)
										{
											string str = dp2.Value.ToString();
											Boolean.TryParse(str, out rs.useForBothLandscapePortrait);
										}
									}
								}
								
							}
						}

						rd.screens.Add(rs);
					}
					
				}
				else if (pair.Key.ToString().CompareTo("rootWidth") == 0)
				{
					XMLNodeList vs = (XMLNodeList) pair.Value;
		 			foreach (XMLNode v in vs)
		            {
						foreach(DictionaryEntry dp in v)
						{
							if (dp.Key.ToString().CompareTo("_text") == 0)
							{
								string str = dp.Value.ToString();

								Int32.TryParse(str, out rd.rootWidth);
							}
						}
					}
				}
				else if (pair.Key.ToString().CompareTo("rootHeight") == 0)
				{
					XMLNodeList vs = (XMLNodeList) pair.Value;
		 			foreach (XMLNode v in vs)
		            {
						foreach(DictionaryEntry dp in v)
						{
							if (dp.Key.ToString().CompareTo("_text") == 0)
							{
								string str = dp.Value.ToString();

								Int32.TryParse(str, out rd.rootHeight);
							}
						}
					}
				}
				else if (pair.Key.ToString().CompareTo("rootAuto") == 0)
				{
					XMLNodeList vs = (XMLNodeList) pair.Value;
		 			foreach (XMLNode v in vs)
		            {
						foreach(DictionaryEntry dp in v)
						{
							if (dp.Key.ToString().CompareTo("_text") == 0)
							{
								string str = dp.Value.ToString();

								bool.TryParse(str, out rd.rootAuto);
							}
						}
					}
				}
				else if (pair.Key.ToString().CompareTo("rootUseBothPortLand") == 0)
				{
					XMLNodeList vs = (XMLNodeList) pair.Value;
		 			foreach (XMLNode v in vs)
		            {
						foreach(DictionaryEntry dp in v)
						{
							if (dp.Key.ToString().CompareTo("_text") == 0)
							{
								string str = dp.Value.ToString();

								bool.TryParse(str, out rd.rootUseBothPortLand);
							}
						}
					}
				}
				
				
			}

			// add one to the device list
			deviceList.Add(rd);
#if RETINAPRO_DEBUGLOG
			
			{
				string debug = "";
				
				debug += "RetinaPro Device = " + rd.name + ", ";
				debug += "pixelSize = " + rd.pixelSize + ", ";
				if (rd.rootAuto)
				{
					debug += "root = Auto, ";
				}
				else
				{
					debug += "root ("+rd.rootWidth + " x " + rd.rootHeight + "), Port & Land = " + rd.rootUseBothPortLand + ", ";
				}
				
				for (int si=0; si<rd.screens.Count; si++)
				{
					debug += "screen[" + si + "] = { " + rd.screens[si].width + " X " + rd.screens[si].height + ", Port & Land = " + rd.screens[si].useForBothLandscapePortrait + " }";
					if (si != rd.screens.Count-1)
						debug += ", ";
				}
				
				Debug.Log(debug);
			}
#endif
        }
		
		return true;
	}

	bool loadAtlases(ref XMLNode nodes)
	{
		XMLNodeList nodeList = nodes.GetNodeList("retinaProData>0>ArrayOfRetinaProAtlas>0>retinaProAtlas");
		if (nodeList == null)
			return false;
		
		foreach (XMLNode node in nodeList)
        {
			retinaProRuntimeAtlas ra = new retinaProRuntimeAtlas();

			foreach(DictionaryEntry pair in node)
			{
				if (pair.Key.ToString().CompareTo("name") == 0)
				{
					XMLNodeList vs = (XMLNodeList) pair.Value;
		 			foreach (XMLNode v in vs)
		            {
						foreach(DictionaryEntry dp in v)
						{
							if (dp.Key.ToString().CompareTo("_text") == 0)
							{
								ra.name = dp.Value.ToString();
							}
						}
					}
				}
				else if (pair.Key.ToString().CompareTo("isFont") == 0)
				{
					XMLNodeList vs = (XMLNodeList) pair.Value;
		 			foreach (XMLNode v in vs)
		            {
						foreach(DictionaryEntry dp in v)
						{
							if (dp.Key.ToString().CompareTo("_text") == 0)
							{
								string str = dp.Value.ToString();

								Boolean.TryParse(str, out ra.isFont);
							}
						}
					}
				}
				
			}

			// add one to the atlas list
			atlasList.Add(ra);
#if RETINAPRO_DEBUGLOG
			Debug.Log("RetinaPro Atlas = " + ra.name + ", " + "isFont = " + ra.isFont);
#endif
        }
		
		return true;
	}

	
}


