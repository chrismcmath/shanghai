using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Shanghai.Grid;
using Shanghai.Controllers;

namespace Shanghai.Path {
    public class PathDrawer : MonoBehaviour {
        private IntVect2 _CurrentCellKey;
        private List<IntVect2> _Path = new List<IntVect2>();

        public void Awake() {
            Messenger<IntVect2>.AddListener(CellController.EVENT_CELL_DRAGGED, OnCellDragged);
            Messenger.AddListener(CellController.EVENT_CELL_DRAG_END, OnCellDragEnd);
        }

        public void OnDestroy() {
            Messenger<IntVect2>.RemoveListener(CellController.EVENT_CELL_DRAGGED, OnCellDragged);
            Messenger.RemoveListener(CellController.EVENT_CELL_DRAG_END, OnCellDragEnd);
        }

        private void OnCellDragged (IntVect2 cellKey) {
            if (GameModel.Instance.Grid.ValidateCellInput(cellKey, _Path)){
                _CurrentCellKey = cellKey;
                _Path.Add(cellKey);
            }
        }

        private void OnCellDragEnd () {
            foreach(IntVect2 key in _Path) {
                Debug.Log("END: x " + key.x + " y " + key.y);
            }
            _Path = new List<IntVect2>();
        }
    }
}
