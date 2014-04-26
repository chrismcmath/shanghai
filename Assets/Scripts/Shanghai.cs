using UnityEngine;
using System.Collections;

using Shanghai.Grid;

namespace Shanghai {
    public class Shanghai : MonoBehaviour {
        private GameModel _Model;

        public PlayGridController gridController;

        public void Awake() {
            _Model = new GameModel();

            if (gridController != null) {

            } else {
                Debug.Error("gridController not set");
            }

            // Update GUI
        }

        public void Update() {
            //Process new events
        }

    }
}
