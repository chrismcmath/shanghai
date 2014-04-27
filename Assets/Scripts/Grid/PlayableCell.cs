using UnityEngine;
using System.Collections;

namespace Shanghai.Grid {
    public class PlayableCell : GridCell {
        public enum PipeType {NONE=0, HORI, VERT, NE, NW, SE, SW, LEFT, RIGHT, TOP, BOTTOM};

        private PipeType _Pipe = PipeType.NONE;
        public PipeType Pipe {
            get { return _Pipe; }
            set {
                _Pipe = value;
                Debug.Log("set Pipe to value " + value);
            }
        }
        public string ClientID = "";
        public string TargetID = "";

        public PlayableCell(IntVect2 key) : base(key) {
        }
    }
}
