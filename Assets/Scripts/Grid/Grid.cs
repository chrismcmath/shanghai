using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shanghai.Grid {
    public class Grid {
        public static readonly string EVENT_CELL_UPDATED = "EVENT_CELL_UPDATED";
        public Vector2 Dimensions;
        private List<List<PlayableCell>> _Cells = new List<List<PlayableCell>>();

        public Grid(int size) {
            Dimensions = new Vector2(size, size);

            for (int y = 0; y < size; y++) {
                List<PlayableCell> row = new List<PlayableCell>();
                for (int x = 0; x < size; x++) {
                    row.Add(new PlayableCell(new IntVect2(x,y)));
                }
                _Cells.Add(row);
            }
        }

        public bool ValidateCellInput(IntVect2 Key, List<IntVect2> path) {
            PlayableCell cell = GetCell(Key);
            //Debug.Log("cell: " + Key.x + ", " + Key.y + " valid: " + cell.Valid + " path length: " + path.Count);
            Debug.Log("cell: " + Key.x + ", " + Key.y);
            if (cell.Valid) {
                cell.Selected = true;
                UpdateCellPipeType(cell, path);
                Messenger<PlayableCell>.Broadcast(EVENT_CELL_UPDATED, cell);
                return true;
            }
            return false;
        }

        public PlayableCell GetCell(IntVect2 Key) {
            return _Cells[Key.y][Key.x];
        }

        private void UpdateCellPipeType(PlayableCell cell, List<IntVect2> path) {
            if (path.Count < 2) return;
            PlayableCell prevCell = GetCell(path[path.Count - 1]);

            Debug.Log("@cell: " + cell.Key.x + ", " + cell.Key.y);
            //Debug.Log("prev: " + prevCell.Key.x + "," + prevCell.Key.y + " curr: " + cell.Key.x + "," + cell.Key.y);
            if (prevCell.Key.x < cell.Key.x) {
                cell.Pipe = PlayableCell.PipeType.LEFT;
            } else if (prevCell.Key.x > cell.Key.x) {
                cell.Pipe = PlayableCell.PipeType.RIGHT;
            } else if (prevCell.Key.y < cell.Key.y) {
                cell.Pipe = PlayableCell.PipeType.TOP;
            } else if (prevCell.Key.y > cell.Key.y) {
                cell.Pipe = PlayableCell.PipeType.BOTTOM;
            }
        }
    }
}
