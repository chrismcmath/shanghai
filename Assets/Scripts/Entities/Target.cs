using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shanghai.Entities {
    public class Target : Entity {
        public static readonly string EVENT_TARGET_UPDATED = "EVENT_TARGET_UPDATED";

        public float Health = 100f;
        private ShanghaiConfig _Config;

        public Target() {}
        public Target(string key) : base (key) {
            _Config = ShanghaiConfig.Instance;
        }

        public void DockHealth(int delta) {
            if (_Config == null) {
                _Config = ShanghaiConfig.Instance;
            }

            Health -= delta;
            ValidateHealth();
        }

        public void ReplenishHealth(float delta) {
            if (_Config == null) {
                _Config = ShanghaiConfig.Instance;
            }
            Health += _Config.HealthIncPerSecond * delta;
            ValidateHealth();
        }

        public void ValidateHealth() {
            Health = (Health < _Config.MinHealth) ? _Config.MinHealth : Health;
            Health = (Health > _Config.MaxHealth) ? _Config.MaxHealth : Health;
            Messenger<Target>.Broadcast(EVENT_TARGET_UPDATED, this);
        }
    }
}
