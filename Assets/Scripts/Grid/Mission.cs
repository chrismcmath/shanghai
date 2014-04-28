using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shanghai.Grid {
    public class Mission {
        public IntVect2 CellKey;
        public string ClientID;
        public string TargetID;
        public float TTL;

        public bool IsActive = false;

        public Mission(IntVect2 cellKey, string clientID, string targetID, float ttl) {
            CellKey = cellKey;
            ClientID = clientID;
            TargetID = targetID;
            TTL = ttl;
        }

        public bool IsTTD(float delta) {
            TTL -= delta;
            return TTL <= 0.0f;
        }
    }
}
