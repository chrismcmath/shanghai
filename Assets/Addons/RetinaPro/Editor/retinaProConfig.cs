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
using System.IO;
using System.Collections.Generic;

public static class retinaProConfig
{
	public const string retinaProDatFolder = "/RetinaPro/Resources";
	public const string retinaProDatFile = "RetinaProData.txt";
	public const string atlasFolder = "rp_Atlases/";
	public const string atlasResourceFolder = "/Resources/rp_atlases/";
	public const string atlasTextureFolder = "/Textures/rp_atlases/";
	
	public static bool isAtlasTextureFolderPresent()
	{
		DirectoryInfo dinfo = new DirectoryInfo(retinaProFileLock.baseDataPath + retinaProConfig.atlasTextureFolder);
		return (dinfo != null && dinfo.Exists);
	}
	
	public static bool isAtlasResourceFolderPresent()
	{
		DirectoryInfo dinfo = new DirectoryInfo(retinaProFileLock.baseDataPath + retinaProConfig.atlasResourceFolder);
		return (dinfo != null && dinfo.Exists);
	}
	
/*	public static bool isOldBinaryDataPresent()
	{
		string folder = "Assets/" + retinaProConfig.retinaProDatFolder + "/RetinaProBinData.txt";
		return File.Exists(folder);
	}*/

	public static bool isSetupComplete()
	{
		return (retinaProConfig.isAtlasResourceFolderPresent() && retinaProConfig.isAtlasTextureFolderPresent()/* && (!isOldBinaryDataPresent())*/ );
	}
	
	

	public static void showSetupUI()
	{
		EditorGUILayout.Space();
		GUILayout.Label("Setup", EditorStyles.boldLabel);
		
/*		if (isOldBinaryDataPresent())
		{
			GUILayout.Label("Detected old binary data format for RetinaPro.");
			GUILayout.Label("Update to new format before continuing.");

			EditorGUILayout.Space();
			
			bool updatePressed = GUILayout.Button("Update format", GUILayout.Width(150f));

			if (updatePressed)
			{
				retinaProDataSerialize.sharedInstance.updateBinaryToXML();
			}
			return;
		}
		else*/
		{
			GUILayout.Label("RetinaPro requires a specific project folder structure");
		}
		EditorGUILayout.Space();
		
		string atlasResLabel = "Assets" + retinaProConfig.atlasResourceFolder;
		string atlasTexLabel = "Assets" + retinaProConfig.atlasTextureFolder;
		
		if (!retinaProConfig.isAtlasResourceFolderPresent())
			atlasResLabel += " [missing]";

		if (!retinaProConfig.isAtlasTextureFolderPresent())
			atlasTexLabel += " [missing]";

		GUILayout.Label(atlasResLabel);
		GUILayout.Label(atlasTexLabel);

		EditorGUILayout.Space();
		bool pressed = GUILayout.Button("Create folders", GUILayout.Width(150f));

		if (pressed)
		{
			// create folders if they are missing
			if (!retinaProConfig.isAtlasResourceFolderPresent())
			{
				DirectoryInfo dinfo = new DirectoryInfo(retinaProFileLock.baseDataPath + retinaProConfig.atlasResourceFolder);
				dinfo.Create();
			}
			
			if (!retinaProConfig.isAtlasTextureFolderPresent())
			{
				DirectoryInfo dinfo = new DirectoryInfo(retinaProFileLock.baseDataPath + retinaProConfig.atlasTextureFolder);
				dinfo.Create();
			}
			
		}
	}
	
	public static bool isOneValidDevice()
	{
		for (int i=0; i<retinaProDataSerialize.sharedInstance.deviceList.Count; i++)
		{
			if (retinaProDataSerialize.sharedInstance.deviceList[i].isDeviceValid())
			{
				return true;
			}
		}
		
		return false;
	}
	
	public static void showValidDeviceNeededUI()
	{
		GUILayout.Label("Add at least one valid device");
		GUILayout.Label("- valid name");
		GUILayout.Label("- non-zero size");
	}
	
	public static void getValidArtFiles(DirectoryInfo dinfo, out List<FileInfo> files)
	{
		files = new List<FileInfo>();
		
		FileInfo [] fis;
		
		fis = dinfo.GetFiles("*.png");
		foreach(FileInfo fi in fis)
		{
			files.Add(fi);
		}
		
		fis = dinfo.GetFiles("*.psd");
		foreach(FileInfo fi in fis)
		{
			files.Add(fi);
		}

		fis = dinfo.GetFiles("*.tif");
		foreach(FileInfo fi in fis)
		{
			files.Add(fi);
		}
		
		fis = dinfo.GetFiles("*.jpg");
		foreach(FileInfo fi in fis)
		{
			files.Add(fi);
		}
		
		fis = dinfo.GetFiles("*.bmp");
		foreach(FileInfo fi in fis)
		{
			files.Add(fi);
		}

		fis = dinfo.GetFiles("*.tga");
		foreach(FileInfo fi in fis)
		{
			files.Add(fi);
		}

		fis = dinfo.GetFiles("*.gif");
		foreach(FileInfo fi in fis)
		{
			files.Add(fi);
		}
		
		fis = null;
	}
}
