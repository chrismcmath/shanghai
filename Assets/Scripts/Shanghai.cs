using UnityEngine;
using System.Collections;

using Shanghai.Grid;
using Shanghai.Controllers;

namespace Shanghai {
    public class Shanghai : MonoBehaviour {

        public PlayGridController GridController;
        public UILabel DebugLabel;

        private float _CurrentTime;

        public void Awake() {
            if (GridController != null) {
                GridController.CreateTable(GameModel.GRID_SIZE);
            } else {
                Debug.Log("GridController not set");
            }

            // Update GUI

            //Messenger
            Messenger<PlayableCell>.AddListener(Grid.Grid.EVENT_CELL_UPDATED, OnCellUpdated);
        }

        public void OnDestroy() {
            Messenger<PlayableCell>.RemoveListener(Grid.Grid.EVENT_CELL_UPDATED, OnCellUpdated);
        }

        private void OnCellUpdated(PlayableCell cell) {
            GridController.UpdateCell(cell.Key, "horizontal", "", "");
        }

        public void Update() {
            _CurrentTime += Time.deltaTime;
            DebugLabel.text = string.Format("{0}", _CurrentTime);
            //Process new events
        }

    }
}
