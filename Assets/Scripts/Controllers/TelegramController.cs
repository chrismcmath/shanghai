using UnityEngine;
using System.Collections;

using Shanghai.Grid;

namespace Shanghai.Controllers {
    public class TelegramController : MonoBehaviour {
        public static readonly string EVENT_TELEGRAM_END_DRAG = "EVENT_TELEGRAM_END_DRAG";
        public static readonly string EVENT_TELEGRAM_DROPPED = "EVENT_TELEGRAM_DROPPED";
        public const float DESTROY_WAIT_TIME = 1.0f;

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
            animation.Play("tele_in");
            Messenger<SourceController>.AddListener(SourceController.EVENT_TELEGRAM_OVER, OnTelegramOver);
        }

        public void DestroyTelegram() {
            animation.Play("tele_out");
            StartCoroutine(DestroyGO());
        }

        private IEnumerator DestroyGO() {
            yield return new WaitForSeconds(DESTROY_WAIT_TIME);
            Destroy(gameObject);
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
                    Destroy(gameObject);
                    Messenger<int, Source>.Broadcast(EVENT_TELEGRAM_DROPPED, _OverSource.Key, _Source);
                } else {
                    DestroyTelegram();
                }
            }
        }

        private void OnTelegramOver(SourceController srcCont) {
            _OverSource = srcCont;
        }
    }
}
