using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shanghai.Entities {
    public class Mission : Entity {
        public IntVect2 CellKey;
        public string ClientID;
        public string TargetID;

        public Mission(IntVect2 cellKey, string clientID, string targetID) {
            CellKey = cellKey;
            ClientID = clientID;
            TargetID = targetID;
        }
    }
}
