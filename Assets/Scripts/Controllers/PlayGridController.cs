using UnityEngine;
using System.Collections;
using Shanghai.Grid;

namespace Shanghai.Controllers {
    public class PlayGridController : MonoBehaviour {
        public static readonly string CELL_PATH = "prefabs/cell";
        public const string CELL_NAME_FORMAT = "y{0}_x{1}";

        private UITable _Table;

        public void Awake() {
            Messenger<PlayableCell>.AddListener(Grid.Grid.EVENT_CELL_UPDATED, OnCellUpdated);
        }

        public void OnDestroy() {
            Messenger<PlayableCell>.RemoveListener(Grid.Grid.EVENT_CELL_UPDATED, OnCellUpdated);
        }

        private void OnCellUpdated(PlayableCell cell) {
            Transform cellTrans = transform.Find(string.Format(CELL_NAME_FORMAT, cell.Key.y, cell.Key.x));
            if (cell != null) {
                CellController cellCtr = cellTrans.GetComponent<CellController>();
                if (cellCtr != null) {
                    cellCtr.UpdateCell(cell);
                }
            }
        }

        public void CreateTable(int size) {
            _Table = gameObject.GetComponent<UITable>();
            if (_Table == null) {
                Debug.Log("UITable not initialized ");
                return;
            }

            GameObject cellPrefab = Resources.Load(CELL_PATH) as GameObject;
            if (cellPrefab == null) {
                Debug.Log("Could not load cell from path " + CELL_PATH);
            }

            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    GameObject cell = GameObject.Instantiate(cellPrefab) as GameObject;

                    CellController cellCtr = cell.GetComponent<CellController>();
                    cellCtr.Key = new IntVect2(x, y);

                    cell.name = string.Format(CELL_NAME_FORMAT, y, x);
                    Debug.Log("created " + cell.name);
                    cell.transform.parent = transform;
                    cell.transform.localPosition = Vector3.zero;
                    cell.transform.localScale = Vector3.one;
                }
            }
        }
    }
}
