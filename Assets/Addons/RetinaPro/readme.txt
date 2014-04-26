------------------------------------------------------------------------
RetinaPro for NGUI
© oeFun, Inc. 2012-2013
http://oefun.com

NGUI and Tasharen are trademarks and copyright of Tasharen Entertainment
------------------------------------------------------------------------

Version 1.5
Support Email:  retinapro@oefun.com

------------------------------------------------------------------------
 Changes
------------------------------------------------------------------------

Version 1.5       **IMPORTANT CHANGES**
- requires NGUI 3.0.7f3 or newer. The faster atlas refreshing is dependent
on access to methods in UIAtlasMaker that are now public.
- added faster atlas refreshing. RetinaPro now adds all of the textures
to an atlas in one pass.
- added new feature to refresh the source textures when an atlas is
created or refreshed. This ensures the source textures are the correct
format and size when making the atlas.

Version 1.4       **IMPORTANT CHANGES**
- requires Unity 4.x (no longer supports 3.5.7)
- retinaProUtil needs to be added to your scene on a game object.
Alternatively you can drop the new RetinaProPrefab into your scene (note:
this prefab also includes retinaProAtlasController - please remove that
from your scene if it's already there.)
- improved retinaProUtil refresh method. Note: this method is no longer
static. Should be called like :-
 retinaProUtil.sharedInstance.refreshAllWidgetsPixelPerfect(myGameObject);
- refreshAllWidgetsPixelPerfect now fixes up atlas and font references for
any widgets in the hierarchy
- added bool check for retinaProAtlasController to enable / disable
the auto-refresh on startup. This is useful if you want to manage all
your updates via the new refreshAllWidgetsPixelPerfect method. When this is
off RetinaPro will refresh the active device and fix the UIRoot but will
not refresh any atlases or fonts. When this is on it behaves as it
previously did.
- fixed issue with sprite padding setting. Refreshing an atlas no longer
resets these values.

Version 1.3.2
- fixed bug that prevented new atlases from being created.

Version 1.3.1
- fixed issue with sprite border setting used for sliced sprites. Refreshing
atlases no longer resets these values.
- fixed button label on device window 'Add' now reads 'Add Screen'.

Version 1.3
- added support for multiple screen sizes per device (different aspect
ratios etc.). Allows you to reuse an atlas for different screen
resolutions.
- redesigned atlases window. Fixes issue where large numbers of atlases
(> 15) would be very slow to edit / change.
- added support to preview window for multiple screens and aspect
- fixed issues with aspect ratio / device selection when handling
auto-rotation
- fixed issues with device selection when running in editor and the game
view is smaller than the actual desired screen resolution
- updated readme.txt file to match new features

Version 1.2.2
- added support for portrait / landscape orientations (see special note).
- added retinaProIgnoreUIRoot script. Drop this script on a
gameobject that has a UIRoot script attached. Doing this will cause
RetinaPro to ignore this UIRoot and thus not modify the manual height
setting.

Version 1.2.1
- updated readme.txt to include helpful Hints and Common issues section
- added retinaProUtil script, adds easy method to refresh UIWidget and 
BoxCollider sizes.
- added example script showing UIPanel visibility and refresh

Version 1.2
- Switched retina pro data file from binary to xml (makes it easier when
working with teams / merging).
- Upon pressing the 'Update button (see any RetinaPro window), any
previous binary RetinaProBinData.txt file will be updated to the xml
version.*
- Fixed get_dataPath error

* Your previous binary file will be renamed (as a backup). If for some
reason the xml conversion fails please email the backed up file
(old_RetinaProBinData.txt) to the support email address above. Thanks!


Version 1.1.1
- Initial release


------------------------------------------------------------------------
 Updating from older version
------------------------------------------------------------------------

1. Open a new blank scene.
2. Close any open RetinaPro windows (devices, preview or atlases)
3. Make a backup of RetinaPro/Resources/RetinaProData.txt
4. Delete everything inside the RetinaPro folder except for the the
   RetinaPro/Resources folder
5. Install package from Unity Asset Store. Be sure to deselect the
   RetinaProData.txt file so you don't overwrite your modified version.


------------------------------------------------------------------------
 What is it all about?
------------------------------------------------------------------------

If you are currently managing iPad, iPhone and retina art assets this 
tool will save you hours of tedious work and allow you to concentrate 
on your app. Easy to setup and even easier to keep your atlases 
up-to-date.


------------------------------------------------------------------------
 Requirements
------------------------------------------------------------------------

1. NGUI 2.3.2 *
2. Unity 3.5.7 or later / Unity 4.0.0f7 or later

*Earlier versions of NGUI are supported - NGUI 2.2.6c, 2.2.7c or 2.3.1.
These versions need to be patched to fix a bug related to font atlases.
Thankfully, this just requires some minor modifications to a few files
in the NGUI Scripts folder. See the end of this document for more
details.

See this forum thread here for additional details:

http://www.tasharen.com/forum/index.php?topic=2656.msg13253#msg13253


------------------------------------------------------------------------
 How To Use
------------------------------------------------------------------------


---Initial configuration:


1. From Unity -> Window menu -> Retina Pro -> Devices.

2. Setup the required folder structure. (Push the button to create the 
folder structure.)

3. Add each device you support, like iPad, iPad@2x, iPhone, iPhone@2x 
   etc. This is done once per project. Add at least one screen resolution
   by tapping the 'Add' button.
   
4. From Unity -> Window menu -> Retina Pro -> Atlases.

5. Create as many atlases as you need by setting the name and texture 
   details. (Push a button to create the folders you need.)

   Note: Check the 'font' toggle if this atlas is a font atlas (not to 
         be confused with a font that is part of another existing atlas).

6. Copy your art files into the required folders*.

7. Push the 'Create' atlas button. This generates an atlas for each
 created device and sets up the atlas and font references.



---Updating artwork:


1. From Unity -> Window menu -> Retina Pro -> Atlases.

2. Update any art files etc.

3. Select the atlas you want to modify.

4. Push the 'Refresh' atlas button. This updates the atlas with your
settings.



---Previewing a device in the editor:

1. Make sure you have the appropriate build target selected (eg. iOS)

2. Select the game view

3. Select the resolution from the game view drop down that matches your
   device resolution settings.
   
4. From Unity -> Window menu -> Retina Pro -> Preview.

5. Choose the appropriate device, screen resolution and orientation.
   This will refresh the atlas and font references and make them point
   to the correct device specific atlas.



---Using an atlas:


1. Use the NGUI widget tool and select any atlas under the 
   Resources/rp_atlases folder. Do not use an atlas that is post-fixed
   with ~<device name>. These are device specific atlases and should not
   be used in your widgets.



* Under Textures/rp_atlases/ you will find a folder per device. In each device
folder, you will find a folder for each atlas. Place the art files you
want in that folder. 


* Make sure that the art file under one device/atlas has the exact same
filename under a different device/atlas. The retina pro tool will verify
this and report errors where they exist. For more information please
see the example scene.


Note: the editor tool will report various issues on the UI itself. Additionally
in certain cases the script will output various warning logs.

Note: using the *same* names for art assets in each atlas is a 
requirement of NGUI. This makes it easy to swap out a different atlas
(and still hook up to the correct art).

Please contact support if you run into any issues. Thanks!


------------------------------------------------------------------------
 The example scene
------------------------------------------------------------------------

---Installation

1. Switch your build target to iOS.

2. Open the scene and either build an iOS build or run in the editor.

3. Try switching the game screen resolution and select the matching
   device in the Unity -> Window -> Retina Pro -> Preview.


The example scene has been tested on iPhone, iPhone Retina, iPhone 5,
iPad and iPad Retina. 

It shows some sprites, label and image button. These assets have been
provided in four different sizes under the Textures folder.

The five different colored boxes are sprites saved using different
file formats.

At runtime the script retinaProAtlasController will automatically select the
correct atlas for the device screen resolution.


------------------------------------------------------------------------
 RetinaProData.txt file / Special Note
------------------------------------------------------------------------

RetinaPro stores it's configuration for all the devices and atlases in
a file called RetinaProData.txt. This file is saved under the 
Asset/retinaPro/Resources folder.

The retinaProAtlasController.cs script shows how to use this data in
your own project. This script will work for most cases and can be used
as is or modified.

Note: this file is stored as xml to make it easier to manually edit
and update when working with teams / version conflicts.



------------------------------------------------------------------------
 Landscape & Portrait orientations / Special Note
------------------------------------------------------------------------

For apps that support auto-rotation:
See RetinaPro/Example/refreshPanelTest.cs / working example.

When the screen re-orientates itself, the screen width & height change.
It's important that when this occurs RetinaPro is notified of the 
change.

To do this check Input.deviceOrientation and when the orientation changes,
call this method:

   retinaProAtlasController.sharedInstance.refreshAll();

That will cause RetinaPro to load the correct atlas for the new screen
size and unload the previous atlas.

Note: be sure that this is only called once when the orientation changes
and is not repeatedly called.


------------------------------------------------------------------------
 Device Settings [NEW]
------------------------------------------------------------------------

Device settings have been expanded to allow support for multiple screen
resolutions per device. Each of these screen sizes can support either
portrait and landscape or just the specified size.

Additionally there is now specific control over the manual height setting
of the UIRoot. Normally you can leave this as 'Auto'. However, in certain
cases you may wish to specify a specific value.

'Auto' will set the UIRoot manual height based on the height of the
selected device. Note: if 'Port & Land' is checked then it will use the
width instead if the device is orientated differently than the specified
orientation.

If 'Auto' is not set then simply specify the width x height that you want
to use. Same rule applies for the 'Port & Land' check box.


------------------------------------------------------------------------
 Hints and Common Issues / Pixel perfect
------------------------------------------------------------------------

- Textures that are not power-of-2 dimensions get converted by Unity's 
importer to be nearest-power-of-2. This causes issues when you're trying
to display something "pixel-perfect". Solution here is to always make 
sure textures are power-of-2 or make them use the next size up.

- At runtime RetinaPro tries to match a 'RetinaPro device' screen size 
to the current screen resolution. If it finds a match it selects that set
of atlases. If it fails it tries to match based on the closest aspect ratio.
It reports failure to match a device type as a warning in the log.
(Note: 640x960 is not the same as 960x640).

- At runtime, after a delay of a few updates, RetinaPro will broadcast
"MakePixelPerfect". This causes all active widgets to update their
 visible size appropriately. However, there are issues with BoxCollider
size and elements that may be current in-active. To solve these types
of issues see below (retinaProUtil). (Note: future versions may remove
this functionality in favor of more robust solution below.)

- In NGUI 2.5.0 the sliced sprite is now a type of UISprite. It
unfortunately uses the transform's scale to represent the size of your
sliced sprite. The problem is there isn't any way to correct for 
pixel-perfect in this case. Workaround is to stick with UISprite / simple.

- The recommended approach when working with UIPanel is to use 

   retinaProUtil.refreshWidgetPixelPerfect(myPanel.gameObject);

This utility will refresh all the visible widgets by calling 
'MakePixelPerfect'. It will then refresh any BoxColliders attached to the
game objects such that they will now match the new visible size.

- If you currently use NGUI's script UIButtonActivate, please switch to
using retinaProButtonActivate instead. In addition to activating or
deactivating it also calls 'MakePixelPerfect' and refreshes the 
BoxCollider size to match. This is a convenient script if you wish to
activate / deactivate UIPanel's with OnClick().


------------------------------------------------------------------------
 Known Issues
------------------------------------------------------------------------

Currently not supported is a font inside a sprite atlas. For a
workaround, please put the font in it's own atlas.

To do that, create an atlas and check the 'Is Font' toggle. 
Place a single font (.txt and .png font files) in that folder. 
The settings window includes helpful setup information. The example
scene shows how to do this.

Supported file formats for images (png, tif, jpg, bmp, tga, gif, psd). 
Note: some of these formats do not support alpha.


------------------------------------------------------------------------
NGUI v2.3.1 - Font "Pixel Size" fix

1. This enables support for font atlases using different pixelSize
2. This patch is required for v2.3.1 to compile RetinaPro

*** Note: the line numbers listed below are only correct if you follow the edits in sequence.

Tested on v2.3.1
------------------------------------------------------------------------

*** in UIFont.cs / line 39 insert:

	// Size in pixels for the sake of MakePixelPerfect functions.
	[HideInInspector][SerializeField] float mPixelSize = 1f;


*** in UIFont.cs / line 72 insert:

	/// <summary>
	/// Pixel size is a multiplier applied to widgets dimensions when performing MakePixelPerfect() pixel correction.
	/// Most obvious use would be on retina screen displays. The resolution doubles, but with UIRoot staying the same
	/// for layout purposes, you can still get extra sharpness by switching to an HD atlas that has pixel size set to 0.5.
	/// </summary>

	public float pixelSize
	{
  		get
  		{
   			return (mReplacement != null) ? mReplacement.pixelSize : mPixelSize;
		}
		set
  		{
   			if (mReplacement != null)
   			{
    			mReplacement.pixelSize = value;
   			}
   			else
   			{
    			float val = Mathf.Clamp(value, 0.25f, 4f);

    			if (mPixelSize != val)
    			{
     				mPixelSize = val;
     				MarkAsDirty();
    			}
   			}
  		}
	}


*** in UILabel.cs / line 488 replace with:

	float pixelSize = (font.atlas != null) ? font.atlas.pixelSize : font.pixelSize;


*** in UILabel.cs / line 523 replace with:

	float pixelSize = (font.atlas != null) ? font.atlas.pixelSize : font.pixelSize;


*** in UIFontInspector.cs / line 243 insert:

	float pixelSize = EditorGUILayout.FloatField("Pixel Size", mFont.pixelSize, GUILayout.Width(120f));

    if (pixelSize != mFont.pixelSize)
    {
    	NGUIEditorTools.RegisterUndo("Font Change", mFont);
    	mFont.pixelSize = pixelSize;
    }


------------------------------------------------------------------------
End of fix
------------------------------------------------------------------------


------------------------------------------------------------------------
NGUI v2.2.7c - Font "Pixel Size" fix

1. This enables support for font atlases using different pixelSize
2. This patch is required for v2.2.7c to compile RetinaPro

*** Note: the line numbers listed below are only correct if you follow the edits in sequence.

Tested on v2.2.7c
------------------------------------------------------------------------

*** in UIFont.cs / line 39 insert:

	// Size in pixels for the sake of MakePixelPerfect functions.
	[HideInInspector][SerializeField] float mPixelSize = 1f;


*** in UIFont.cs / line 72 insert:

	/// <summary>
	/// Pixel size is a multiplier applied to widgets dimensions when performing MakePixelPerfect() pixel correction.
	/// Most obvious use would be on retina screen displays. The resolution doubles, but with UIRoot staying the same
	/// for layout purposes, you can still get extra sharpness by switching to an HD atlas that has pixel size set to 0.5.
	/// </summary>

	public float pixelSize
	{
  		get
  		{
   			return (mReplacement != null) ? mReplacement.pixelSize : mPixelSize;
		}
		set
  		{
   			if (mReplacement != null)
   			{
    			mReplacement.pixelSize = value;
   			}
   			else
   			{
    			float val = Mathf.Clamp(value, 0.25f, 4f);

    			if (mPixelSize != val)
    			{
     				mPixelSize = val;
     				MarkAsDirty();
    			}
   			}
  		}
	}


*** in UILabel.cs / line 484 replace with:

	float pixelSize = (font.atlas != null) ? font.atlas.pixelSize : font.pixelSize;


*** in UILabel.cs / line 519 replace with:

	float pixelSize = (font.atlas != null) ? font.atlas.pixelSize : font.pixelSize;


*** in UIFontInspector.cs / line 241 insert:

	float pixelSize = EditorGUILayout.FloatField("Pixel Size", mFont.pixelSize, GUILayout.Width(120f));

    if (pixelSize != mFont.pixelSize)
    {
    	NGUIEditorTools.RegisterUndo("Font Change", mFont);
    	mFont.pixelSize = pixelSize;
    }


------------------------------------------------------------------------
End of fix
------------------------------------------------------------------------


------------------------------------------------------------------------
NGUI v2.2.6c - Font "Pixel Size" fix

1. This enables support for font atlases using different pixelSize
2. This patch is required for v2.2.6c to compile RetinaPro

*** Note: the line numbers listed below are only correct if you follow the edits in sequence.

Tested on v2.2.6c
------------------------------------------------------------------------

*** in UIFont.cs / line 39 insert:

	// Size in pixels for the sake of MakePixelPerfect functions.
	[HideInInspector][SerializeField] float mPixelSize = 1f;


*** in UIFont.cs / line 72 insert:

	/// <summary>
	/// Pixel size is a multiplier applied to widgets dimensions when performing MakePixelPerfect() pixel correction.
	/// Most obvious use would be on retina screen displays. The resolution doubles, but with UIRoot staying the same
	/// for layout purposes, you can still get extra sharpness by switching to an HD atlas that has pixel size set to 0.5.
	/// </summary>

	public float pixelSize
	{
  		get
  		{
   			return (mReplacement != null) ? mReplacement.pixelSize : mPixelSize;
		}
		set
  		{
   			if (mReplacement != null)
   			{
    			mReplacement.pixelSize = value;
   			}
   			else
   			{
    			float val = Mathf.Clamp(value, 0.25f, 4f);

    			if (mPixelSize != val)
    			{
     				mPixelSize = val;
     				MarkAsDirty();
    			}
   			}
  		}
	}


*** in UILabel.cs / line 478 replace with:

	float pixelSize = (font.atlas != null) ? font.atlas.pixelSize : font.pixelSize;


*** in UILabel.cs / line 513 replace with:

	float pixelSize = (font.atlas != null) ? font.atlas.pixelSize : font.pixelSize;


*** in UIFontInspector.cs / line 243 insert:

	float pixelSize = EditorGUILayout.FloatField("Pixel Size", mFont.pixelSize, GUILayout.Width(120f));

    if (pixelSize != mFont.pixelSize)
    {
    	NGUIEditorTools.RegisterUndo("Font Change", mFont);
    	mFont.pixelSize = pixelSize;
    }


------------------------------------------------------------------------
End of fix
------------------------------------------------------------------------
