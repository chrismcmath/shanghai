using UnityEngine;
using System.Collections;

namespace Shanghai.Grid {
    public class GridCell {
        public Vector2 Key;
        public bool Valid = true;

        public GridCell(Vector2 key) {
            Key = key;
        }
    }
}
