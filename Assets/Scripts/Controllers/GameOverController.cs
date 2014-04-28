using UnityEngine;
using System.Collections;

using Shanghai.Grid;

namespace Shanghai.Controllers {
    public class GameOverController : MonoBehaviour {
        public void OnClick() {
            Debug.Log("on click");
            Messenger.Broadcast(Shanghai.EVENT_GAME_START);
        }
    }
}
