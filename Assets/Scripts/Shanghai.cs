using UnityEngine;
using System.Collections;

using Shanghai.Grid;
using Shanghai.Controllers;

namespace Shanghai {
    public class Shanghai : MonoBehaviour {
        private GameModel _Model;

        public PlayGridController GridController;

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
            //Process new events
        }

    }
}
