using UnityEngine;
using System.Collections;

using Shanghai.Grid;

namespace Shanghai.Controllers {
    public class TitleController : MonoBehaviour {
        public void OnClick() {
            Messenger.Broadcast(Shanghai.EVENT_GAME_START);
        }
    }
}
