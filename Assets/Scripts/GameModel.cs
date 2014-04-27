using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Shanghai.Entities;
using Shanghai.Grid;
using Shanghai.Path;

namespace Shanghai {
    public class GameModel : MonoSingleton<GameModel> {
        public static readonly int GRID_SIZE = 6;

        private Grid.Grid _Grid;
        public Grid.Grid Grid {
            get { return _Grid; }
        }

        private Dictionary<string, Client> _Clients;
        public Dictionary<string, Client> Clients {
            get { return _Clients; }
        }

        private Dictionary<string, Target> _Targets;
        public Dictionary<string, Target> Targets {
            get { return _Targets; }
        }

        public GameModel() {
            Init();
        }

        public void Init() {
            /* Add clients (embassies) */
            _Clients = new Dictionary<string, Client>();
            AddEntityToCollection("uk", _Clients);
            AddEntityToCollection("france", _Clients);
            AddEntityToCollection("usa", _Clients);
            AddEntityToCollection("japan", _Clients);
            AddEntityToCollection("russia", _Clients);

            /* Add targets (minitries) */
            _Targets = new Dictionary<string, Target>();
            AddEntityToCollection("education", _Targets);
            AddEntityToCollection("environment", _Targets);
            AddEntityToCollection("health", _Targets);
            AddEntityToCollection("justice", _Targets);
            AddEntityToCollection("trade", _Targets);

            _Grid = new Grid.Grid(GRID_SIZE);
        }

        private void AddEntityToCollection<T>(string id, Dictionary<string, T> collection) where T : Entity, new() {
            T entity = new T();
            entity.ID = id;
            collection.Add(id, entity);
        }
    }
}
