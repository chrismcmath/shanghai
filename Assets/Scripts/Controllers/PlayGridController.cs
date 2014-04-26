using UnityEngine;
using System.Collections;

namespace Shanghai.Controllers {
    public class PlayGridController : MonoBehaviour {
        public static readonly string CELL_PATH = "prefabs/cell";

        private UITable _Table;

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
            Debug.Log("create tble wth size " + size);
            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    GameObject cell = GameObject.Instantiate(cellPrefab) as GameObject;

                    CellController cellCtr = cell.GetComponent<CellController>();
                    cellCtr.Key = new IntVect2(x, y);

                    cell.name = string.Format("{0}_{1}", x, y);
                    cell.transform.parent = transform;
                    cell.transform.localPosition = Vector3.zero;
                    cell.transform.localScale = Vector3.one;
                }
            }
        }

        public void UpdateCell(IntVect2 id, string pipeState, string clientState, string targetState) {
            // find cell and pass info
            Transform cell = transform.Find(string.Format("{0}_{1}", id.x, id.y));
            if (cell != null) {
                Transform cellTrans = transform.Find(string.Format("{0}_{1}", id.x, id.y));
                CellController cellCtr = cellTrans.GetComponent<CellController>();
                if (cellCtr != null) {
                    cellCtr.UpdateCell(pipeState, clientState, targetState);
                }
            }
        }
    }
}
