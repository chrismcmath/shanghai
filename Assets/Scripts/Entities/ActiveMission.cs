using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shanghai.Entities {
    public class ActiveMission : Entity {
        public static readonly string EVENT_CELL_PROGRESSED = "EVENT_CELL_PROGRESSED";

        public Mission Mission;
        public List<IntVect2> Path;
        public int Bounty = 0;

        private float CurrentCellProgress = 0.0f;
        private int CurrentCellID = 0;

        public ActiveMission(Mission mission, List<IntVect2> path) {
            Mission = mission;
            Path = path;
        }

        public bool Progress(float progress) {
            CurrentCellProgress += progress;
            Messenger<IntVect2, float>.Broadcast(EVENT_CELL_PROGRESSED, Path[CurrentCellID], CurrentCellProgress);

            if (CurrentCellProgress >= 1.0f) {
                CurrentCellProgress = 0.0f;
                CurrentCellID++;
                if (CurrentCellID >= Path.Count) {
                    return true;
                }
            }
            return false;
        }
    }
}
