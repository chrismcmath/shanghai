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


[XmlRoot("atlas")]
public class retinaProAtlas
{
	private string _atlasName;
	private FilterMode _atlasFilterMode;
	private TextureImporterFormat _atlasTextureFormat;
	private int _atlasPadding;
	private bool _isFont;
	
	
	[XmlElement("name")]
    public string atlasName
	{
    	get { return _atlasName; }
        set { _atlasName = value; }
    }	
	
	[XmlElement("filterMode")]
    public FilterMode atlasFilterMode
	{
    	get { return _atlasFilterMode; }
        set { _atlasFilterMode = value; }
    }	

	[XmlElement("textureFormat")]
    public TextureImporterFormat atlasTextureFormat
	{
    	get { return _atlasTextureFormat; }
        set { _atlasTextureFormat = value; }
    }	
	
	[XmlElement("padding")]
    public int atlasPadding
	{
    	get { return _atlasPadding; }
        set { _atlasPadding = value; }
    }	
	
	[XmlElement("isFont")]
    public bool isFont
	{
    	get { return _isFont; }
        set { _isFont = value; }
    }	
	
	
	public retinaProAtlas()
	{
		_atlasName = "";
		_atlasFilterMode = FilterMode.Point;
		_atlasTextureFormat = TextureImporterFormat.AutomaticTruecolor;
		_atlasPadding = 2;
		_isFont = false;
	}

	public void load(ref BinaryReader readBinary)
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
			
			case 4:
				_atlasTextureFormat = (TextureImporterFormat) readBinary.ReadInt32();
				break;
	
			case 3:
				_isFont = readBinary.ReadBoolean();
				break;
	
			case 2:
				_atlasPadding = readBinary.ReadInt32();
				break;
	
			case 1:
				_atlasName = readBinary.ReadString();
				_atlasFilterMode = (FilterMode) readBinary.ReadInt32();
				break;
		}
	}
	
}
