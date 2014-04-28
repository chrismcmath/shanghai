using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Shanghai.Grid;

namespace Shanghai {
    public class ShanghaiUtils {
        public static bool IsEndPoint(PlayableCell cell) {
            return cell.ClientID != "";
        }

        public static bool KeysMatch(IntVect2 k1, IntVect2 k2) {
            return k1.x == k2.x && k1.y == k2.y;
        }

        public static string BeautifyString(string str) {
            switch (str) {
                case "education":
                    return "Ministry of Education";
                    break;
                case "environment":
                    return "Ministry of Environmental Affairs";
                    break;
                case "health":
                    return "Ministry of Health";
                    break;
                case "justice":
                    return "Ministry of Justice";
                    break;
                case "trade":
                    return "Ministry of Trade";
                    break;
                default:
                    return "none found";
                    break;
            }
        }

        public static void RemoveAllChildren(Transform transform) {
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in transform) children.Add(child.gameObject);
            children.ForEach(child => Object.Destroy(child));
        }
    }
}
