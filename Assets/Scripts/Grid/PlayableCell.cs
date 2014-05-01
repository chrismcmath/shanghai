using UnityEngine;
using System.Collections;

namespace Shanghai.Grid {
    public class PlayableCell : GridCell {
        public static readonly string EVENT_CELL_UPDATED = "EVENT_CELL_UPDATED";

        public enum PipeType {NONE=0, HORI, VERT, NE, NW, SE, SW, LEFT, RIGHT, TOP, BOTTOM};

        private PipeType _Pipe = PipeType.NONE;
        public PipeType Pipe {
            get { return _Pipe; }
            set {
                _Pipe = value;
            }
        }
        public string ClientID = "";
        public string TargetID = "";

        public int Bounty = 0;

        public float Progress = 0.0f;
        public float TotalProgress = 0.0f;

        public PlayableCell(IntVect2 key) : base(key) {
        }

        public void Reset() {
            ClientID = "";
            TargetID = "";
            Bounty = 0;
            Progress = 0.0f;
            TotalProgress = 0.0f;
            Pipe = PipeType.NONE;
        }

        public bool IsFree() {
            return Pipe == PipeType.NONE;
        }

        public bool HasMission() {
            return ClientID != "" || TargetID != "";
        }
    }
}
