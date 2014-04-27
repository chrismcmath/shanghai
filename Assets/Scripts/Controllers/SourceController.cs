using UnityEngine;
using System.Collections;

using Shanghai.Grid;

namespace Shanghai.Controllers {
    public class SourceController : MonoBehaviour {
        public static readonly string EVENT_TELEGRAM_OVER = "EVENT_TELEGRAM_OVER";

        private bool _TelegramIsOver = false;

        public UISprite TargetSprite;
        public UILabel BountyLabel;

        public int Key = 0;
        public Source Source = null;

        public void UpdateSource(SourceCell sourceCell) {
            Key = sourceCell.Key;
            Source = sourceCell.Source;
            if (Source != null) {
                TargetSprite.color = ShanghaiConfig.Instance.MinistryColours[Source.TargetID];
                BountyLabel.text = string.Format("{0}元", Source.Bounty);
            } else {
                TargetSprite.color = Color.white;
                BountyLabel.text = "";
            }
        }

        public void OnDragOver(GameObject draggedGO) {
            Messenger<SourceController>.Broadcast(EVENT_TELEGRAM_OVER, this);
        }

        public void OnDragOut(GameObject draggedGO) {
        }

        public void OnDragEnd() {
            Messenger.Broadcast(CellController.EVENT_CELL_DRAG_END);
        }
    }
}
