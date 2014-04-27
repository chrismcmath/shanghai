using UnityEngine;

/*
http://wiki.unity3d.com/index.php/Singleton
*/
public class Singleton<T> where T : class, new() {
    // <summary>
    // Singleton implementation, readonly and static ensure thread safeness.
    // </summary>
    public static readonly T Instance = new T();
}

public class MonoSingleton<T> : UnityEngine.MonoBehaviour where T : UnityEngine.MonoBehaviour {
    protected static T _Instance;

    /**
      Returns the instance of this singleton.
      */

    public static T GetInstance(bool logError) {
        if (_Instance == null) {
            _Instance = GetSingleton<T>(logError);
        }
        return _Instance;
    }

    public static T Instance {
        get {
            return GetInstance(true);
        }
    }

    public static bool HasInstance() {
        return GetInstance(false) != null;
    }

    public static T GetSingleton<T>() where T : Object {
        return GetSingleton<T>(true);
    }

        public static T GetSingleton<T>(bool logError) where T : Object {
            T[] objects = FindObjectsOfType<T>();
            if (objects.Length == 1) {
                return objects[0];
            } else {
                if (logError) {
                    string msg = "";
                    foreach (T obj in objects) {
                        if (!string.IsNullOrEmpty(msg)) {
                            msg += ", ";
                        }
                        msg += obj.name;
                    }
                }
            }
            return null;
        }

        public static T FindObjectOfType<T>() where T : Object {
            return Object.FindObjectOfType(typeof(T)) as T;
        }
}
