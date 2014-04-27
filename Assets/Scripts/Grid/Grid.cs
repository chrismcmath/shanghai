using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shanghai.Grid {
    public class Grid {
        public static readonly string EVENT_SET_PATH = "EVENT_SET_PATH";
        public static readonly string EVENT_CELL_UPDATED = "EVENT_CELL_UPDATED";
        public static readonly string EVENT_GRID_UPDATED = "EVENT_GRID_UPDATED";
        public static readonly string EVENT_MISSION_FAILED = "EVENT_MISSION_FAILED";

        private List<List<PlayableCell>> _Cells = new List<List<PlayableCell>>();
        public List<List<PlayableCell>> Cells {
            get { return _Cells; }
        }

        public Grid(int size) {
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
            if (CellIsConnected(key, path) &&
                    CheckPrevCellPositions(key, path) &&
                    cell.Valid) {
                cell.Valid = false;
                UpdateCellPipeType(cell, path);
                Messenger<PlayableCell>.Broadcast(PlayableCell.EVENT_CELL_UPDATED, cell);
                return true;
            }
            return false;
        }

        public PlayableCell GetCell(IntVect2 key) {
            return _Cells[key.y][key.x];
        }

        public void SetPath(List<IntVect2> path) {
            if (path.Count < 1) {
                return;
            }
            PlayableCell finalCell = GetCell(path[path.Count - 1]);
            if (finalCell.TargetID == "") {
                ResetCells(path);
                Messenger.Broadcast(EVENT_MISSION_FAILED);
                return;
            } else {
                //TODO create an active mission.
            }
            Messenger<List<IntVect2>>.Broadcast(EVENT_SET_PATH, path);
        }

        public bool GetRandomCell(ref IntVect2 key) {
            List<PlayableCell> availableCells = new List<PlayableCell>();
            foreach (List<PlayableCell> row in _Cells) {
                foreach (PlayableCell cell in row) {
                    if (cell.IsFree()) {
                        availableCells.Add(cell);
                    }
                }
            }

            if (availableCells.Count < 1) {
                return false;
            } else {
                key = availableCells.ElementAt(Random.Range(0, availableCells.Count)).Key;
                return true;
            }
        }

        public void CellProgressed(IntVect2 cellKey, float progress) {
            PlayableCell cell = GetCell(cellKey);
            cell.Progress = progress;
            Messenger<PlayableCell>.Broadcast(PlayableCell.EVENT_CELL_UPDATED, cell);
        }

        public void ResetAllCells() {
            foreach (List<PlayableCell> row in _Cells) {
                foreach (PlayableCell cell in row) {
                    cell.Reset();
                }
            }
            Messenger<List<List<PlayableCell>>>.Broadcast(EVENT_GRID_UPDATED, _Cells, MessengerMode.DONT_REQUIRE_LISTENER);
        }

        public void ResetCells(List<IntVect2> path) {
            foreach (IntVect2 cellKey in path) {
                PlayableCell cell = GetCell(cellKey);
                cell.Reset();
            }
            Messenger<List<List<PlayableCell>>>.Broadcast(EVENT_GRID_UPDATED, _Cells);
        }

        private bool CheckPrevCellPositions(IntVect2 key, List<IntVect2> path) {
            if (CellInPath(key, path)) {
                Messenger.Broadcast(EVENT_MISSION_FAILED);
                return false;
            }
            return true;
        }

        private bool CellInPath(IntVect2 key, List<IntVect2> path) {
            foreach (IntVect2 cellKey in path) {
                if (cellKey == key) {
                    return true;
                }
            }
            return false;
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
            Messenger<PlayableCell>.Broadcast(PlayableCell.EVENT_CELL_UPDATED, prevCell);
        }
    }
}
