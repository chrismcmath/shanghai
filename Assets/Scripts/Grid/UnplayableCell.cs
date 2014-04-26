using UnityEngine;
using System.Collections;

namespace Shanghai.Grid {
    public class UnplayableCell : GridCell {
        public UnplayableCell(Vector2 key) : base(key) {
            Valid = false;
        }
    }
}
