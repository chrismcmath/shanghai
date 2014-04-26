using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shanghai.Grid {
    public class Grid {
        public Vector2 Dimensions;
        private List<GridCell> _Cells = new List<GridCell>();

        public Grid(int size) {
            Dimensions = new Vector2(size, size);

            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    /* top row */
                    if (y == 0) {
                        /* top left is an empty cell*/
                        if (x == 0) {
                            _Cells.Add(new UnplayableCell(new Vector2(x,y)));
                        } else {
                            _Cells.Add(new SourceCell(new Vector2(x,y)));
                        }
                    } else {
                        /* all left cells are ChanceCells */
                        if (x == 0) {
                            _Cells.Add(new ChanceCells(new Vector2(x,y)));
                        } else {
                            _Cells.Add(new PlayableCell(new Vector2(x,y)));
                        }
                    }
                }
            }
        }
    }
}
