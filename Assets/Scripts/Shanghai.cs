using UnityEngine;
using System.Collections;

using Shanghai.Grid;
using Shanghai.Controllers;

namespace Shanghai {
    public class Shanghai : MonoBehaviour {

        public PlayGridController GridController;
        public UILabel DebugLabel;

        private GameModel _Model;
        private float _CurrentTime;

        public void Awake() {
            _Model = new GameModel();

            if (GridController != null) {
                GridController.CreateTable(GameModel.GRID_SIZE+1);
            } else {
                Debug.Log("GridController not set");
            }

            // Update GUI
        }

        public void Update() {
            _CurrentTime += Time.deltaTime;
            DebugLabel.text = string.Format("{0}", _CurrentTime);
            //Process new events
        }

    }
}
