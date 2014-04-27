using UnityEngine;
using System.Collections;

namespace Shanghai.Controllers {
    public class MoneyController : MonoBehaviour {

        public UILabel MoneyLabel;

        public void Awake() {
            Messenger<int>.AddListener(GameModel.EVENT_MONEY_CHANGED, OnMoneyChanged);
        }

        public void OnDestroy() {
            Messenger<int>.RemoveListener(GameModel.EVENT_MONEY_CHANGED, OnMoneyChanged);
        }

        public void OnMoneyChanged(int money) {
            MoneyLabel.text = string.Format("元{0}", money);
        }
    }
}
