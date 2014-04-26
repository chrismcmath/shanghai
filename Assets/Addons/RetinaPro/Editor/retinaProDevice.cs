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
using System.Xml.Serialization;

[XmlRoot("device")]
public class retinaProDevice
{
//	const int kBinarySerialVersion = 3;

	private string 					_deviceName;
	private float 					_pixelSize;
	private bool 					_rootAuto;
	private int						_rootHeight;
	private int 					_rootWidth;
	private bool					_rootUseBothPortLand;
	private List<retinaProScreen> 	_screens;
	
	private int 					_screenWidth;
	private int 					_screenHeight;
	private bool 					_useForBothLandscapePortrait;
	
	[XmlElement("name")]
    public string name
	{
    	get { return _deviceName; }
        set { _deviceName = value; }
    }	
	
	[XmlElement("pixelSize")]
    public float pixelSize
	{
    	get { return _pixelSize; }
        set { _pixelSize = value; }
    }	

	[XmlElement("rootAuto")]
    public bool rootAuto
	{
    	get { return _rootAuto; }
        set { _rootAuto = value; }
    }	

	[XmlElement("rootWidth")]
    public int rootWidth
	{
    	get { return _rootWidth; }
        set { _rootWidth = value; }
    }	

	[XmlElement("rootHeight")]
    public int rootHeight
	{
    	get { return _rootHeight; }
        set { _rootHeight = value; }
    }	

	[XmlElement("rootUseBothPortLand")]
    public bool rootUseBothPortLand
	{
    	get { return _rootUseBothPortLand; }
        set { _rootUseBothPortLand = value; }
    }	

	[XmlElement("screens")]
    public List<retinaProScreen> screens
	{
    	get { return _screens; }
        set { _screens = value; }
    }	

	[XmlElement("screenWidth")]
    public int screenWidth
	{
    	get { return _screenWidth; }
        set { _screenWidth = value; }
    }	
	
	[XmlElement("screenHeight")]
    public int screenHeight
	{
    	get { return _screenHeight; }
        set { _screenHeight = value; }
    }	
	
	[XmlElement("useForBothLandscapePortrait")]
    public bool useForBothLandscapePortrait
	{
    	get { return _useForBothLandscapePortrait; }
        set { _useForBothLandscapePortrait = value; }
    }	

	public retinaProDevice()
	{
		_deviceName = "";
		_pixelSize = 1.0f;
		_rootWidth = 0;
		_rootHeight = 0;
		_rootAuto = true;
		_rootUseBothPortLand = false;
		_screens = new List<retinaProScreen>(10);

		// no longer used - will be removed in a later version
		_screenWidth = 0;
		_screenHeight = 0;
		_useForBothLandscapePortrait = false;
	}
	
/*	public void load(ref BinaryReader readBinary)
	{
		int version = readBinary.ReadInt32();
		for(int i=version; i>=1; i--)
		{
			loadVersion(i, ref readBinary);
		}
	}
	
	void loadVersion(int version, ref BinaryReader readBinary)
	{
		switch(version)
		{
			default:
				break;
	
			case 3:
				_screenWidth = readBinary.ReadInt32();
				break;

			case 2:
				_screenHeight = readBinary.ReadInt32();
				break;
			
			case 1:
				_deviceName = readBinary.ReadString();
				_pixelSize = readBinary.ReadSingle();
				break;
		}
	}*/
	
	public bool isDeviceValid()
	{
		if (_deviceName == null || _deviceName.Length == 0)
			return false;
		
		if (_screens.Count == 0)
			return false;
		
		foreach(retinaProScreen rps in _screens)
		{
			if (rps.width == 0 || rps.height == 0)
				return false;
		}
		
		return true;
	}
	
}
