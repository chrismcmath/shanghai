//-------------------------------------------------------------------------
// RetinaPro for NGUI
// © oeFun, Inc. 2012-2013
// http://oefun.com
// 
// NGUI and Tasharen are trademarks and copyright of Tasharen Entertainment
//-------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

public class retinaProDataSerialize
{
    static 			retinaProDataSerialize 		_sharedInstance = null;
	static readonly object padlock = new object();
	
	public static retinaProDataSerialize sharedInstance
    {
		get
		{
			lock(padlock)
			{
				if (_sharedInstance == null)
				{
					new retinaProDataSerialize();
				}
				
				return _sharedInstance;
			}
		}

    }


	public List<retinaProDevice> deviceList;
	public List<retinaProAtlas> atlasList;
	
	public retinaProDataSerialize()
	{
		_sharedInstance = this;

		deviceList = new List<retinaProDevice> ();
		atlasList = new List<retinaProAtlas> ();
		
		loadSettings();
	}
	
	~retinaProDataSerialize()
	{
		saveSettings();
		_sharedInstance = null;
	}
	

	public void saveSettings()
	{
		lock(retinaProFileLock.fileLock)
		{
			fixDataFolder();
	
			saveSettingsWithoutLock();
		}
	}
	
	void saveSettingsWithoutLock()
	{
		string folder = "Assets/" + retinaProConfig.retinaProDatFolder + "/" + retinaProConfig.retinaProDatFile;
		FileStream writeStream = new FileStream(folder, FileMode.Create, FileAccess.Write, FileShare.None);
		if (writeStream == null)
		{
			Debug.LogWarning("Could not save settings");
			return;
		}
		
		XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
		ns.Add("", "");
		
		XmlWriterSettings xmlsettings = new XmlWriterSettings();
		xmlsettings.NewLineHandling = NewLineHandling.Entitize;
		xmlsettings.Encoding = System.Text.Encoding.Unicode;
		xmlsettings.Indent = true;
		xmlsettings.IndentChars = "\t";
		
		XmlWriter writeText = XmlWriter.Create(writeStream, xmlsettings);
		if (writeText == null)
		{
			writeStream.Close();
			Debug.LogWarning("Could not create text writer for save settings");
			return;
		}
		
		writeText.WriteStartDocument();
		writeText.WriteStartElement("retinaProData");
		
		XmlSerializer deviceSerialize = new XmlSerializer(typeof(List<retinaProDevice>));
		deviceSerialize.Serialize(writeText, deviceList, ns);
		
		XmlSerializer atlasSerialize = new XmlSerializer(typeof(List<retinaProAtlas>));
		atlasSerialize.Serialize(writeText, atlasList, ns);
		
		// finish and close out file
		writeText.WriteEndElement();
		writeText.WriteEndDocument();		

		writeText.Flush();
		writeText.Close();
		writeStream.Close();
	}
	
	public void loadSettings()
	{
		lock(retinaProFileLock.fileLock)
		{
			fixDataFolder();
	
			// does the new xml version of the data exist?
			string folder = "Assets/" + retinaProConfig.retinaProDatFolder + "/" + retinaProConfig.retinaProDatFile;
			bool check = File.Exists(folder);

			if (check)
			{
				// load the new xml version
				FileStream readStream = new FileStream(folder, FileMode.Open, FileAccess.Read, FileShare.None);
				if (readStream == null)
				{
					Debug.LogWarning("Could not load settings");
					return;
				}
				
				XmlReaderSettings xmlsettings = new XmlReaderSettings();
				
				
				XmlReader readText = XmlReader.Create(readStream, xmlsettings);
				if (readText == null)
				{
					readStream.Close();
					Debug.LogWarning("Could not create text reader for load settings");
					return;
				}

				readText.ReadStartElement("retinaProData");
				
				// read in devices
				XmlSerializer deviceSerialize = new XmlSerializer(typeof(List<retinaProDevice>));
				deviceList = (List<retinaProDevice>) deviceSerialize.Deserialize(readText);
				
				// check for older xml format with a single screen
				updateScreens();
		
				// read in atlases
				XmlSerializer atlasSerialize = new XmlSerializer(typeof(List<retinaProAtlas>));	
				atlasList = (List<retinaProAtlas>) atlasSerialize.Deserialize(readText);
		
		
				// finish and close out file
				readText.ReadEndElement();
		
				readText.Close();
				readStream.Close();
			}
			

		}
	
	}
	
	void updateScreens()
	{
		// convert the old screen data in retinaProDevice into the new retinaProScreen object
		foreach (retinaProDevice rd in deviceList)
		{
			if (rd.isDeviceValid() && rd.screens.Count == 0)
			{
				retinaProScreen rps = new retinaProScreen();
				rps.width = rd.screenWidth;
				rps.height = rd.screenHeight;
				rps.useForBothLandscapePortrait = rd.useForBothLandscapePortrait;
				
				rd.screens.Add(rps);
			}
		}
	}
	
	void fixDataFolder()
	{
		string folder = retinaProFileLock.baseDataPath + retinaProConfig.retinaProDatFolder;
		
		DirectoryInfo dinfo = new DirectoryInfo(folder);
		if (dinfo == null || !dinfo.Exists)
		{
			dinfo.Create();
		}
	}
	
	
	const string edPrefPreviewDeviceIdx = "retinaPro_previewDeviceIdx";
	
	public int getPreviewDeviceIdx()
	{
		int idx = EditorPrefs.GetInt(edPrefPreviewDeviceIdx) % deviceList.Count;
		EditorPrefs.SetInt(edPrefPreviewDeviceIdx, idx);
		
		return idx;
	}
	
	public int setPreviewDeviceIdx(int idx)
	{
		idx = idx % deviceList.Count;
		EditorPrefs.SetInt(edPrefPreviewDeviceIdx, idx);
		
		return idx;
	}
		
	const string edPrefPreviewScreenIdx = "retinaPro_previewScreenIdx";
	
	public int getPreviewScreenIdx()
	{
		int di = getPreviewDeviceIdx();
		int sc = deviceList[di].screens.Count;
		
		int idx = EditorPrefs.GetInt(edPrefPreviewScreenIdx) % sc;
		EditorPrefs.SetInt(edPrefPreviewScreenIdx, idx);
		
		return idx;
	}
	
	public int setPreviewScreenIdx(int idx)
	{
		int di = getPreviewDeviceIdx();
		int sc = deviceList[di].screens.Count;

		idx = idx % sc;
		EditorPrefs.SetInt(edPrefPreviewScreenIdx, idx);
		
		return idx;
	}

	const string edPrefGameViewIdx = "retinaPro_previewGameViewIdx";

	public int getPreviewGameViewIdx()
	{
		int idx = EditorPrefs.GetInt(edPrefGameViewIdx) % 2;
		EditorPrefs.SetInt(edPrefGameViewIdx, idx);
		
		return idx;
	}

	public int setPreviewGameViewIdx(int idx)
	{
		idx = idx % 2;
		EditorPrefs.SetInt(edPrefGameViewIdx, idx);
		
		return idx;
	}

	const string edPrefUtilityRefreshSourceTextures = "retinaPro_utilityRefreshSourceTextures";
		
	public bool getUtilityRefreshSourceTextures()
		{
		return EditorPrefs.GetBool(edPrefUtilityRefreshSourceTextures);
		}

	public void setUtilityRefreshSourceTextures(bool refresh)
		{
		EditorPrefs.SetBool(edPrefUtilityRefreshSourceTextures, refresh);
		}

	
}


