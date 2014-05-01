using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Shanghai.Entities;
using Shanghai.Grid;

namespace Shanghai {
    public class EventGenerator {
        public static readonly string EVENT_MISSION_CREATED = "EVENT_MISSION_CREATED";
        public static readonly string EVENT_SOURCE_CREATED = "EVENT_SOURCE_CREATED";

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
            string targetID = _Model.Targets.ElementAt(Random.Range(0, _Model.Targets.Count)).Value.Key;
            string clientID = _Model.Clients.ElementAt(Random.Range(0, _Model.Clients.Count)).Value.Key;
            float TTL =_Config.MissionWaitTimeMedium + (Random.Range(0, _Config.MissionWaitTimeDeviance*2)) - _Config.MissionWaitTimeDeviance; 

            Mission mission = new Mission(cellKey, clientID, targetID, TTL);
            Messenger<Mission>.Broadcast(EVENT_MISSION_CREATED, mission);
            return true;
        }

        public bool GenerateSource() {
            float bountyDeviance = Mathf.Pow(Random.Range(0f,1f), _Config.BountyDeviancePower) * (float) (_Config.BountyMax - _Config.BountyMin);
            float bounty = bountyDeviance + _Config.BountyMin;
        
            Target target = _Model.Targets.ElementAt(Random.Range(0, _Model.Targets.Count)).Value;
            //bounty *= target.Health / _Config.MaxHealth;

            Source source = new Source((int) bounty, target.Key);
            Messenger<Source>.Broadcast(EVENT_SOURCE_CREATED, source);
            return true;
        }
    }
}

