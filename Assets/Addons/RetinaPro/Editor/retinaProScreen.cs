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

[XmlRoot("screen")]
public class retinaProScreen
{
	private int _width;
	private int _height;
	private bool _useForBothLandscapePortrait;
	
	[XmlElement("width")]
    public int width
	{
    	get { return _width; }
        set { _width = value; }
    }	
	
	[XmlElement("height")]
    public int height
	{
    	get { return _height; }
        set { _height = value; }
    }	
	
	[XmlElement("useForBothLandscapePortrait")]
    public bool useForBothLandscapePortrait
	{
    	get { return _useForBothLandscapePortrait; }
        set { _useForBothLandscapePortrait = value; }
    }	

	public retinaProScreen()
	{
		_width = 0;
		_height = 0;
		_useForBothLandscapePortrait = false;
	}
	
	public bool isScreenValid()
	{
		if (_width == 0 || _height == 0)
			return false;
		
		return true;
	}
	
}
