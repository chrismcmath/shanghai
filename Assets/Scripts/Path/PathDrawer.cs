using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Shanghai.Grid;
using Shanghai.Controllers;

namespace Shanghai.Path {
    public class PathDrawer : MonoBehaviour {
        private List<IntVect2> _Path = new List<IntVect2>();
        private GameModel _Model;

        public void Awake() {
            Messenger<IntVect2>.AddListener(CellController.EVENT_CELL_DRAGGED, OnCellDragged);
            Messenger.AddListener(CellController.EVENT_CELL_DRAG_END, OnCellDragEnd);
            _Model = GameModel.Instance;
        }

        public void OnDestroy() {
            Messenger<IntVect2>.RemoveListener(CellController.EVENT_CELL_DRAGGED, OnCellDragged);
            Messenger.RemoveListener(CellController.EVENT_CELL_DRAG_END, OnCellDragEnd);
        }

        private void OnCellDragged(IntVect2 cellKey) {
            if (_Model.CanDraw) {
                if (_Model.Grid.ValidateCellInput(cellKey, _Path)){
                    _Path.Add(cellKey);
                    if (ShanghaiUtils.IsEndPoint(_Model.Grid.GetCell(cellKey))) {
                        _Model.CanDraw = false;
                    }
                }
            }
        }

        private void OnCellDragEnd() {
            /*
            foreach(IntVect2 key in _Path) {
                Debug.Log("END: x " + key.x + " y " + key.y);
            }
            */
            _Model.Grid.SetPath(_Path);
            _Path = new List<IntVect2>();
        }
    }
}
