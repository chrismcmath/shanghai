using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shanghai.Entities {
    public class Source : Entity {
        public int Bounty = 0;
        public string TargetID = "";
        public Source() {}
        public Source(string id, int bounty, string targetID) : base (id) {
            Bounty = bounty;
            TargetID = targetID;
        }
    }
}
