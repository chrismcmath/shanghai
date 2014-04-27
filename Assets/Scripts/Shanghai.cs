using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Shanghai.Entities;
using Shanghai.Grid;
using Shanghai.Controllers;

namespace Shanghai {
    public class Shanghai : MonoBehaviour {
        public PlayGridController GridController;
        public SourceRowController SourceRowController;
        public UILabel DebugLabel;

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

            // Update GUI

            Messenger<PlayableCell>.AddListener(PlayableCell.EVENT_CELL_UPDATED, OnCellUpdated);
            Messenger<List<IntVect2>>.AddListener(Grid.Grid.EVENT_SET_PATH, OnSetPath);
            Messenger.AddListener(Grid.Grid.EVENT_MISSION_FAILED, OnMissionFailed);
        }

        public void OnDestroy() {
            Messenger<PlayableCell>.RemoveListener(PlayableCell.EVENT_CELL_UPDATED, OnCellUpdated);
            Messenger<List<IntVect2>>.RemoveListener(Grid.Grid.EVENT_SET_PATH, OnSetPath);
            Messenger.RemoveListener(Grid.Grid.EVENT_MISSION_FAILED, OnMissionFailed);
        }

        private void OnCellUpdated(PlayableCell cell) {
            //GridController.UpdateCell(cell.Key, "horizontal", "", "");
        }

        public void Update() {
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

            //Process new events
        }

        public void UpdateActiveMissions(float delta) {
            List<ActiveMission> garbage = new List<ActiveMission>();
            foreach (ActiveMission actMiss in _Model.ActiveMissions) {
                if (actMiss.Progress(delta * _Config.CellFillPerSecond)) {
                    garbage.Add(actMiss);
                    //TODO: active mission finished logic here
                    //_Model.Money += actMiss.Bounty;
                }
            }

            /* Garbage collection */
            foreach (ActiveMission actMiss in garbage) {
                _Model.RemoveActiveMission(actMiss);
            }
        }

        private void OnSetPath(List<IntVect2> path) {
            if (true) { //if we are in play state
                _Model.CanDraw = true;
            }

            Source source = _Model.Grid.GetSourceFromCell(path[0].x);
            PlayableCell cell = _Model.Grid.GetCell(path[path.Count-1]);
            Mission mission = _Model.GetMissionFromCellKey(path[path.Count-1]);

            if (source.TargetID != mission.TargetID) {
                _Model.Grid.ResetCells(path);
                _Model.Grid.ResetSourceCell(path[0].x);
            } else {
                _Model.ActiveMissions.Add(new ActiveMission(mission, path, source));
            }
        }

        private void OnMissionFailed() {
            Debug.Log("mission failed");
        }
    }
}
