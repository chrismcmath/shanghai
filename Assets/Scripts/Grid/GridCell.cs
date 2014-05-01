using UnityEngine;
using System.Collections;

namespace Shanghai.Grid {
    public class GridCell {

        public IntVect2 Key;
        public bool Selected = false;

        public GridCell(IntVect2 key) {
            Key = key;
        }
    }
}
