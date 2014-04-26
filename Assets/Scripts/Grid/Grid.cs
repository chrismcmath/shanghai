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
            PlayableCell cell = _Cells[Key.y][Key.x];
            if (cell.Valid) {
                cell.Selected = true;
                UpdateCellPipeType(cell, path);
                Messenger<PlayableCell>.Broadcast(EVENT_CELL_UPDATED, cell);
            }
            return false;
        }

        public PlayableCell GetCell(IntVect2 Key) {
            return _Cells[Key.y][Key.x];
        }

        private void UpdateCellPipeType(PlayableCell cell, List<IntVect2> path) {
            IntVect2 prevKey = path[path.Count - 1];

            if (prevKey.x < cell.Key.x) {

                //TODO: put in pipe types here
                // let a helper function convert them to the string

            } else if (prevKey.x > cell.Key.x) {
            } else if (prevKey.y < cell.Key.y) {
            } else if (prevKey.y > cell.Key.y) {
            }
        }
    }
}
