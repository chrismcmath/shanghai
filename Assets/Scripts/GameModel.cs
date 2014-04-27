using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Shanghai.Entities;
using Shanghai.Grid;
using Shanghai.Path;

namespace Shanghai {
    public class GameModel : MonoSingleton<GameModel> {
        public static readonly string EVENT_MONEY_CHANGED = "EVENT_MONEY_CHANGED";

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

        private List<Mission> _Missions = new List<Mission>();
        public List<Mission> Missions {
            get { return _Missions; }
        }

        private List<ActiveMission> _ActiveMissions = new List<ActiveMission>();
        public List<ActiveMission> ActiveMissions {
            get { return _ActiveMissions; }
        }

        private bool _CanDraw = true;
        public bool CanDraw {
            get { return _CanDraw; }
            set { _CanDraw = value; }
        }

        private int _Money = 0;
        public int Money {
            get { return _Money; }
            set {
                Debug.Log("set money to " + value);
                if (value != _Money) {
                    _Money = value;
                    Messenger<int>.Broadcast(EVENT_MONEY_CHANGED, _Money);
                }
            }
        }

        public void Awake() {
            Messenger<Mission>.AddListener(EventGenerator.EVENT_MISSION_CREATED, OnMissionCreated);
            Messenger<IntVect2, float>.AddListener(ActiveMission.EVENT_CELL_PROGRESSED, OnCellProgressed);
            Init();
        }

        public void OnDestroy() {
            Messenger<Mission>.RemoveListener(EventGenerator.EVENT_MISSION_CREATED, OnMissionCreated);
            Messenger<IntVect2, float>.RemoveListener(ActiveMission.EVENT_CELL_PROGRESSED, OnCellProgressed);
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
            _Grid.ResetAllCells();
        }

        //NOTE: NOT WORKING Timing issue here, need to update after Controllers have initialized
        private IEnumerator UpdateGridController() {
            yield return new WaitForEndOfFrame();
            Messenger<List<List<PlayableCell>>>.Broadcast(Grid.EVENT_GRID_UPDATED, _Grid.Cells);
        }

        private void AddEntityToCollection<T>(string id, Dictionary<string, T> collection) where T : Entity, new() {
            T entity = new T();
            entity.ID = id;
            collection.Add(id, entity);
        }

        public Mission GetMissionFromCellKey(IntVect2 key) {
            foreach (Mission mission in _Missions) {
                if (ShanghaiUtils.KeysMatch(mission.CellKey, key)) {
                    return mission;
                }
            }
            Debug.Log("ERROR, couldn't get mission from key " + key);
            return null;
        }

        public void RemoveActiveMission(ActiveMission actMiss) {
            _Grid.ResetCells(actMiss.Path);
            _Missions.Remove(actMiss.Mission);
            _ActiveMissions.Remove(actMiss);
        }

        private void OnMissionCreated(Mission mission) {
            Missions.Add(mission);
            PlayableCell cell = _Grid.GetCell(mission.CellKey);
            cell.ClientID = mission.ClientID;
            cell.TargetID = mission.TargetID;
            cell.Bounty = mission.Bounty;
            Messenger<PlayableCell>.Broadcast(PlayableCell.EVENT_CELL_UPDATED, cell);
        } 

        private void OnCellProgressed(IntVect2 cellKey, float progress) {
            _Grid.CellProgressed(cellKey, progress);
        }
    }
}
