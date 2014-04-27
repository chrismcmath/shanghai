using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shanghai.Grid {
    public class SourceRow {
        private List<Source> _Sources = new List<Source>();
        public List<Source> Sources {
            get { return _Sources; }
        }

        public SourceRow(int size) {
            for (int i = 0; i < size; i++) {
                _Sources.Add(new Source(i));
            }
        }
    }
}
