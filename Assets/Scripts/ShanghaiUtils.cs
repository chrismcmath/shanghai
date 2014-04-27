using UnityEngine;
using System.Collections;

using Shanghai.Grid;

namespace Shanghai {
    public class ShanghaiUtils {
        public static bool IsEndPoint(PlayableCell cell) {
            return cell.ClientID != "";
        }

        public static bool KeysMatch(IntVect2 k1, IntVect2 k2) {
            return k1.x == k2.x && k1.y == k2.y;
        }
    }
}
