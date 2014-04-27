using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shanghai.Grid {
    public class Source {
        public int Bounty = 0;
        public string TargetID = "";

        public Source() {}
        public Source(int bounty, string targetID) {
            Bounty = bounty;
            TargetID = targetID;
        }
    }
}
