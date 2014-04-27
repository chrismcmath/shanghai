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

        public bool ValidateCellInput(IntVect2 key, List<IntVect2> path) {
            PlayableCell cell = GetCell(key);
            Debug.Log("key " + key + " valid " + cell.Valid + " selected " + cell.Selected + " connected " + CellIsConnected(key, path));
            if (cell.Valid && !cell.Selected && CellIsConnected(key, path)) {
                cell.Selected = true;
                UpdateCellPipeType(cell, path);
                Messenger<PlayableCell>.Broadcast(EVENT_CELL_UPDATED, cell);
                return true;
            }
            return false;
        }

        public PlayableCell GetCell(IntVect2 key) {
            return _Cells[key.y][key.x];
        }

        private bool CellIsConnected(IntVect2 key, List<IntVect2> path) {
            if (path.Count < 1) {
                if (key.y == 0) {
                    //TODO: check clients
                    return true;
                }
            } else {
                IntVect2 prevKey = GetCell(path[path.Count - 1]).Key;
                if ((Mathf.Abs(key.x - prevKey.x) == 1 && (key.y == prevKey.y)) ||
                        (Mathf.Abs(key.y - prevKey.y) == 1 && (key.x == prevKey.x))) {
                    return true;
                }
            }
            return false;
        }

        private void UpdateCellPipeType(PlayableCell cell, List<IntVect2> path) {
        //TODO: Left is coming from chance card
            if (path.Count < 1) {
                cell.Pipe = PlayableCell.PipeType.TOP;
                return;
            }
            PlayableCell prevCell = GetCell(path[path.Count - 1]);

            if (prevCell.Key.x < cell.Key.x) {
                cell.Pipe = PlayableCell.PipeType.LEFT;
            } else if (prevCell.Key.x > cell.Key.x) {
                cell.Pipe = PlayableCell.PipeType.RIGHT;
            } else if (prevCell.Key.y < cell.Key.y) {
                cell.Pipe = PlayableCell.PipeType.TOP;
            } else if (prevCell.Key.y > cell.Key.y) {
                cell.Pipe = PlayableCell.PipeType.BOTTOM;
            }

            UpdatePrevCell(prevCell, cell);
        }

        private void UpdatePrevCell(PlayableCell prevCell, PlayableCell cell) {
            if (prevCell.Pipe == cell.Pipe) {
                prevCell.Pipe = (prevCell.Pipe == PlayableCell.PipeType.LEFT ||
                        prevCell.Pipe == PlayableCell.PipeType.RIGHT) ?  
                    PlayableCell.PipeType.HORI  : PlayableCell.PipeType.VERT;
            }
            switch (prevCell.Pipe) {
                case PlayableCell.PipeType.LEFT:
                    switch (cell.Pipe) {
                        case PlayableCell.PipeType.TOP:
                            prevCell.Pipe = PlayableCell.PipeType.SW;
                            break;
                        case PlayableCell.PipeType.BOTTOM:
                            prevCell.Pipe = PlayableCell.PipeType.NW;
                            break;
                    }
                    break;
                case PlayableCell.PipeType.RIGHT:
                    switch (cell.Pipe) {
                        case PlayableCell.PipeType.TOP:
                            prevCell.Pipe = PlayableCell.PipeType.SE;
                            break;
                        case PlayableCell.PipeType.BOTTOM:
                            prevCell.Pipe = PlayableCell.PipeType.NE;
                            break;
                    }
                    break;
                case PlayableCell.PipeType.TOP:
                    switch (cell.Pipe) {
                        case PlayableCell.PipeType.LEFT:
                            prevCell.Pipe = PlayableCell.PipeType.NE;
                            break;
                        case PlayableCell.PipeType.RIGHT:
                            prevCell.Pipe = PlayableCell.PipeType.NW;
                            break;
                    }
                    break;
                case PlayableCell.PipeType.BOTTOM:
                    switch (cell.Pipe) {
                        case PlayableCell.PipeType.LEFT:
                            prevCell.Pipe = PlayableCell.PipeType.SE;
                            break;
                        case PlayableCell.PipeType.RIGHT:
                            prevCell.Pipe = PlayableCell.PipeType.SW;
                            break;
                    }
                    break;
                case PlayableCell.PipeType.NONE:
                    Debug.Log("Prev cell cant be NONE");
                    break;
            }
            Debug.Log("update prev cell, type: " + prevCell.Pipe);
            Messenger<PlayableCell>.Broadcast(EVENT_CELL_UPDATED, prevCell);
        }
    }
}
