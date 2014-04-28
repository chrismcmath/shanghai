using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Shanghai.Entities;

namespace Shanghai.Controllers {
    public class TargetController : MonoBehaviour {
        private string _Key;
        public string Key {
            get { return _Key; }
            set {
                _Key = value;
                BackgroundSprite.color = ShanghaiConfig.Instance.MinistryColours[_Key];
            }
        }

        public UISprite BackgroundSprite;
        public UILabel HealthLabel;

        public void Awake() {
        }

        public void OnDestroy() {
        }

        public void UpdateTarget(Target target) {
            HealthLabel.text = string.Format("{0}", Mathf.Round(target.Health));
        }
    }
}
