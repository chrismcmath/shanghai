using UnityEngine;
using System.Collections;

namespace Shanghai.Grid {
    public class PlayableCell : GridCell {
        public enum PipeType {NONE=0, HORI, VERT, NE, NW, SE, SW, LEFT, RIGHT, TOP, BOTTOM};

        public PipeType Pipe = PipeType.NONE;

        public PlayableCell(IntVect2 key) : base(key) {
        }
    }
}
