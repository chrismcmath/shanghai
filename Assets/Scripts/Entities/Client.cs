using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shanghai.Entities {
    public class Client : Entity {
        public static readonly string EVENT_CLIENT_UPDATED = "EVENT_CLIENT_UPDATED";

        public float Reputation = 100f;
        private ShanghaiConfig _Config;

        public Client() {}
        public Client(string key) : base (key) {
        }

        public void DockReputation() {
            if (_Config == null) {
                _Config = ShanghaiConfig.Instance;
            }
            Reputation += _Config.ReputationDecOnMissionFailed;
            ValidateReputation();
        }

        public void IncReputation() {
            if (_Config == null) {
                _Config = ShanghaiConfig.Instance;
            }
            Reputation += _Config.ReputationIncOnMissionComplete;
            ValidateReputation();
        }

        public void DrainReputation(float delta) {
            if (_Config == null) {
                _Config = ShanghaiConfig.Instance;
            }
            Reputation -= _Config.ReputationDecPerSecond * delta;
            ValidateReputation();
        }

        public void ValidateReputation() {
            Reputation = (Reputation < _Config.MinReputation) ? _Config.MinReputation : Reputation;
            Reputation = (Reputation > _Config.MaxReputation) ? _Config.MaxReputation : Reputation;
            Messenger<Client>.Broadcast(EVENT_CLIENT_UPDATED, this);
        }
    }
}
