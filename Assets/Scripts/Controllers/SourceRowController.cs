using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Shanghai.Grid;

namespace Shanghai.Controllers {
    public class SourceRowController : MonoBehaviour {
        public static readonly string SOURCE_PATH = "prefabs/source";

        private UITable _Table;

        public void Awake() {
            Messenger<SourceCell>.AddListener(SourceCell.EVENT_SOURCE_CELL_UPDATED, OnSourceCellUpdated);
        }

        public void OnDestroy() {
            Messenger<SourceCell>.RemoveListener(SourceCell.EVENT_SOURCE_CELL_UPDATED, OnSourceCellUpdated);
        }

        private void OnSourceCellUpdated(SourceCell sourceCell) {
            Debug.Log("OnSourceCellUpdated");
            Transform sourceTrans = transform.Find(string.Format("{0}", sourceCell.Key));
            if (sourceTrans != null) {
                SourceController sourceCtr = sourceTrans.GetComponent<SourceController>();
                if (sourceCtr != null) {
                    sourceCtr.UpdateSource(sourceCell);
                }
            }
        }

        public void CreateTable(int size) {
            _Table = gameObject.GetComponent<UITable>();
            if (_Table == null) {
                Debug.Log("UITable not initialized ");
                return;
            }

            GameObject sourcePrefab = Resources.Load(SOURCE_PATH) as GameObject;
            if (sourcePrefab == null) {
                Debug.Log("Could not load source from path " + SOURCE_PATH);
            }

            for (int x = 0; x < size; x++) {
                GameObject source = GameObject.Instantiate(sourcePrefab) as GameObject;

                SourceController sourceCtr = source.GetComponent<SourceController>();
                sourceCtr.Key = x;

                source.name = string.Format("{0}", x);
                source.transform.parent = transform;
                source.transform.localPosition = Vector3.zero;
                source.transform.localScale = Vector3.one;
            }
        }
    }
}
