using UnityEngine;
using System.Collections;

namespace Shanghai.Grid {
    public class SourceCell {
        public static readonly string EVENT_SOURCE_CELL_UPDATED = "EVENT_SOURCE_CELL_UPDATED";

        public Source Source = null;
        public int Key;
        public SourceCell(int key) {
            Key = key;
        }
    }
}
