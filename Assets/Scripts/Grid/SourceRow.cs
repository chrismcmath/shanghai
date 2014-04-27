using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shanghai.Grid {
    public class SourceRow {
        private List<SourceCell> _SourceCells = new List<SourceCell>();
        public List<SourceCell> SourceCells {
            get { return _SourceCells; }
        }

        public SourceRow(int size) {
            for (int i = 0; i < size; i++) {
                _SourceCells.Add(new SourceCell(i));
            }
        }

        public void UpdateSourceCell(int key, Source source) {
            SourceCell cell = GetCell(key);
            cell.Source = source;
            Messenger<SourceCell>.Broadcast(SourceCell.EVENT_SOURCE_CELL_UPDATED, cell);
        }

        public bool HasSource(int key) {
            SourceCell cell = GetCell(key);
            return cell.Source != null;
        }

        public void ResetSourceCell(int key) {
            SourceCell cell = GetCell(key);
            cell.Source = null;
            Messenger<SourceCell>.Broadcast(SourceCell.EVENT_SOURCE_CELL_UPDATED, cell);
        }

        public SourceCell GetCell(int key) {
            foreach (SourceCell cell in _SourceCells) {
                if (cell.Key == key) {
                    return cell;
                }
            }
            return null;
        }
    }
}
