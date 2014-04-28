using UnityEngine;
using System.Collections;

using Shanghai.Grid;

namespace Shanghai.Controllers {
    public class TelegramController : MonoBehaviour {
        public static readonly string EVENT_TELEGRAM_END_DRAG = "EVENT_TELEGRAM_END_DRAG";
        public static readonly string EVENT_TELEGRAM_DROPPED = "EVENT_TELEGRAM_DROPPED";

        public UILabel TargetLabel;
        public UILabel BountyLabel;
        public UISprite BackgroundSprite;

        private bool _IsDragging = false;
        public bool IsDragging {
            get { return _IsDragging; }
        }

        private SourceController _OverSource = null;

        private Source _Source;
        public Source Source {
            set { 
                _Source = value;
                TargetLabel.text = string.Format("{0}", ShanghaiUtils.BeautifyString(_Source.TargetID));
                BountyLabel.text = string.Format("{0}", _Source.Bounty);
                BackgroundSprite.color = _Config.MinistryColours[_Source.TargetID];
            }
        }

        private ShanghaiConfig _Config;

        public void Awake() {
            _Config = ShanghaiConfig.Instance;
            Messenger<SourceController>.AddListener(SourceController.EVENT_TELEGRAM_OVER, OnTelegramOver);
        }

        public void Destroy() {
            Messenger<SourceController>.RemoveListener(SourceController.EVENT_TELEGRAM_OVER, OnTelegramOver);
        }

        public void OnDragStart() {
            _IsDragging = true;
        }

        public void OnPress(bool press) {
            if (_IsDragging && !press) {
                if (_OverSource != null) {
                    Messenger<int, Source>.Broadcast(EVENT_TELEGRAM_DROPPED, _OverSource.Key, _Source);
                }
                Destroy(gameObject);
            }
        }

        private void OnTelegramOver(SourceController srcCont) {
            _OverSource = srcCont;
        }
    }
}
