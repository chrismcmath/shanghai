using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Shanghai.Entities;
using Shanghai.Grid;
using Shanghai.Controllers;

namespace Shanghai {
    public class Shanghai : MonoBehaviour {
        public static readonly string EVENT_GAME_START = "EVENT_GAME_START";

        public enum GameState {START=0, PLAY, END_GAME};

        public PlayGridController GridController;
        public SourceRowController SourceRowController;
        public TargetsController TargetsController;
        public ClientsController ClientsController;
        public TitleController TitleController;
        public GameOverController GameOverController;

        public UILabel DebugLabel;

        private GameState _State = GameState.START;
        private float _CurrentTime;
        private float _MissionInterval = 0.0f;
        private float _SourceInterval = 0.0f;
        private GameModel _Model;
        private ShanghaiConfig _Config;
        private EventGenerator _Generator;

        public void Awake() {
            _Model = GameModel.Instance;
            _Config = ShanghaiConfig.Instance;
            _Generator = new EventGenerator();


            // Update GUI

            Messenger<List<IntVect2>>.AddListener(Grid.Grid.EVENT_SET_PATH, OnSetPath);
            Messenger.AddListener(Grid.Grid.EVENT_MISSION_FAILED, OnMissionFailed);
            Messenger.AddListener(EVENT_GAME_START, OnGameStart);
        }

        public void CreateAssets() {
            if (GridController != null) {
                GridController.CreateTable(GameModel.GRID_SIZE);
            } else {
                Debug.Log("GridController not set");
            }
            if (SourceRowController != null) {
                SourceRowController.CreateTable(GameModel.GRID_SIZE);
            } else {
                Debug.Log("SourceRowController not set");
            }
            if (TargetsController != null) {
                TargetsController.CreateTable(_Model.Targets);
            } else {
                Debug.Log("TargetsControlle rnot set");
            }
            if (ClientsController != null) {
                ClientsController.CreateTable(_Model.Clients);
            } else {
                Debug.Log("ClientsController not set");
            }
        }

        public void OnDestroy() {
            Messenger<List<IntVect2>>.RemoveListener(Grid.Grid.EVENT_SET_PATH, OnSetPath);
            Messenger.RemoveListener(Grid.Grid.EVENT_MISSION_FAILED, OnMissionFailed);
            Messenger.RemoveListener(EVENT_GAME_START, OnGameStart);
        }

        public void Update() {
            switch (_State) {
                case GameState.START:
                    break;
                case GameState.PLAY:
                    GameLoop();
                    break;
                case GameState.END_GAME:
                    break;
            }   
        }

        public void OnGameStart() {
            TitleController.gameObject.SetActive(false);
            GameOverController.gameObject.SetActive(false);
            _Model.Reset();
            CreateAssets();
            _State = GameState.PLAY;
        }
            

        private void GameLoop() {
            _CurrentTime += Time.deltaTime;
            _MissionInterval -= Time.deltaTime;
            _SourceInterval -= Time.deltaTime;
            DebugLabel.text = string.Format("{0:00}\nMission:{1:00}\nSource:{1:00}", _CurrentTime, _MissionInterval, _SourceInterval);

            if (_MissionInterval < 0.0f) {
                _MissionInterval = _Config.MissionInterval;
                _Generator.GenerateMission();
            }
            if (_SourceInterval < 0.0f) {
                _SourceInterval = _Config.SourceInterval;
                _Generator.GenerateSource();
            }

            UpdateActiveMissions(Time.deltaTime);
            ReplinishTargets(Time.deltaTime);
            DrainClients(Time.deltaTime);
            TickMissions(Time.deltaTime);

            CheckForEndGame();
        }

        public void CheckForEndGame() {
            foreach (KeyValuePair<string, Client> client in _Model.Clients) {
                if (client.Value.Reputation <= _Config.MinReputation) {
                    EndGame();
                }
            }
        }

        private void EndGame() {
            GameOverController.gameObject.SetActive(true);
            GameOverController.Populate(_Model.Targets, _Model.Money);
            _Model.CanDraw = false;
            _State = GameState.END_GAME;
        }

        public void ReplinishTargets(float delta) {
            foreach (KeyValuePair<string, Target> target in _Model.Targets) {
                target.Value.ReplenishHealth(delta);
            }
        }

        public void DrainClients(float delta) {
            foreach (KeyValuePair<string, Client> client in _Model.Clients) {
                client.Value.DrainReputation(delta);
            }
        }

        public void TickMissions(float delta) {
            foreach (Mission mission in _Model.Missions) {
                if (!mission.IsActive && mission.IsTTD(delta)) {
                    PlayableCell cell = _Model.Grid.GetCell(mission.CellKey);
                    cell.TargetID = "";
                    cell.ClientID = "";
                    Messenger<PlayableCell>.Broadcast(PlayableCell.EVENT_CELL_UPDATED, cell);
                }
            }
        }

        public void UpdateActiveMissions(float delta) {
            List<ActiveMission> garbage = new List<ActiveMission>();
            foreach (ActiveMission actMiss in _Model.ActiveMissions) {
                if (actMiss.Progress(delta * _Config.CellFillPerSecond)) {
                    garbage.Add(actMiss);
                    //TODO: active mission finished logic here

                    Target target = _Model.Targets[actMiss.Mission.TargetID];
                    target.DockHealth(actMiss.Bounty);
                    Client client = _Model.Clients[actMiss.Mission.ClientID];
                    client.IncReputation();
                }
            }

            /* Garbage collection */
            foreach (ActiveMission actMiss in garbage) {
                _Model.RemoveActiveMission(actMiss);
            }
        }

        private void OnSetPath(List<IntVect2> path) {
            if (_State == GameState.PLAY) {
                _Model.CanDraw = true;
            }

            Source source = _Model.Grid.GetSourceFromCell(path[0].x);
            PlayableCell cell = _Model.Grid.GetCell(path[path.Count-1]);
            Mission mission = _Model.GetMissionFromCellKey(path[path.Count-1]);

            if (source.TargetID != mission.TargetID) {
                _Model.Grid.ResetCellsInPath(path);
                _Model.Grid.ResetSourceCell(path[0].x);
                Client client = _Model.Clients[mission.ClientID];
                client.DockReputation();
            } else {
                _Model.ActiveMissions.Add(new ActiveMission(mission, path, source));
            }
        }

        private void OnMissionFailed() {
            Debug.Log("mission failed");
        }
    }
}
