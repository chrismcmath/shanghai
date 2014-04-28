using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Shanghai.Entities;

namespace Shanghai.Controllers {
    public class ClientController : MonoBehaviour {
        public static readonly string CLIENT_PREFIX = "embassy";
        public static readonly string FLAG_PREFIX = "flag";

        private string _Key;
        public string Key {
            get { return _Key; }
            set {
                _Key = value;
                Debug.Log("new key: " + _Key);
                BackgroundSprite.spriteName = string.Format("{0}_{1}", CLIENT_PREFIX, _Key);
                FlagSprite.spriteName = string.Format("{0}_{1}", FLAG_PREFIX, _Key);
            }
        }

        public UISprite BackgroundSprite;
        public UISprite FlagSprite;
        public Transform Flag;

        public float FlagTop = 1.0f;
        public float FlagBottom = 0.0f;

        private ShanghaiConfig _Config;

        public void Awake() {
            _Config = ShanghaiConfig.Instance;
        }

        public void OnDestroy() {
        }

        public void UpdateClient(Client client) {

            float repFrac = (client.Reputation - _Config.MinReputation) / (_Config.MaxReputation - _Config.MinReputation);
            Flag.localPosition = new Vector3(Flag.localPosition.x, FlagBottom + (FlagTop - FlagBottom) * repFrac, Flag.localPosition.z);
        }
    }
}
