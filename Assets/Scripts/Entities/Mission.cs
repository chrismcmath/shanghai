using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shanghai.Entities {
    public class Mission : Entity {
        public IntVect2 CellKey;
        public string ClientID;
        public string TargetID;
        public int Bounty = 0;

        public Mission(IntVect2 cellKey, string clientID, string targetID, int bounty) {
            CellKey = cellKey;
            ClientID = clientID;
            TargetID = targetID;
            Bounty = bounty;
        }
    }
}
