using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Shanghai.Entities;
using Shanghai.Grid;
using Shanghai.Path;

namespace Shanghai {
    public class GameModel : MonoBehaviour {
        private Grid.Grid _Grid;
        public Grid.Grid Grid {
            get { return _Grid; }
        }

        private List<Client> _Clients;
        public List<Client> Clients {
            get { return _Clients; }
        }

    }
}
