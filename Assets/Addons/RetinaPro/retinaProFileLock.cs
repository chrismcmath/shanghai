using UnityEngine;
using System.Collections;

public class retinaProFileLock {

	public static readonly object fileLock = new object();
	public static string baseDataPath = Application.dataPath;
}
