using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Shanghai.Entities;
using Shanghai.Grid;

namespace Shanghai {
    public class EventGenerator {
        public static readonly string EVENT_MISSION_CREATED = "EVENT_MISSION_CREATED";

        private GameModel _Model;
        private ShanghaiConfig _Config;

        public EventGenerator() {
            _Config = ShanghaiConfig.Instance;
            _Model = GameModel.Instance;
        }

        public bool GenerateMission() {
            IntVect2 cellKey = new IntVect2(0,0);
            if (!_Model.Grid.GetRandomCell(ref cellKey)) {
                return false;
            }
            string targetID = _Model.Targets.ElementAt(Random.Range(0, _Model.Targets.Count)).Value.ID;
            string clientID = _Model.Clients.ElementAt(Random.Range(0, _Model.Clients.Count)).Value.ID;

            Mission mission = new Mission(cellKey, clientID, targetID);
            Messenger<Mission>.Broadcast(EVENT_MISSION_CREATED, mission);
            return true;
        }

        public bool GenerateSource() {
            int bounty = _Config.BountyMedium + (Random.Range(0, _Config.BountyDeviance*2)) - _Config.BountyDeviance;
            return true;
        }
    }
}
