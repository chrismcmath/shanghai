﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shanghai.Grid {
    public class ActiveMission {
        public static readonly string EVENT_CELL_PROGRESSED = "EVENT_CELL_PROGRESSED";
        public static readonly string EVENT_PACKAGE_DELIVERED = "EVENT_PACKAGE_DELIVERED";

        public Mission Mission;
        public List<IntVect2> Path;
        public Source Source;
        public int Bounty;

        private float CurrentCellProgress = 0.0f;
        private int CurrentCellID = 0;
        private ShanghaiConfig _Config;

        public ActiveMission(Mission mission, List<IntVect2> path, Source source) {
            Mission = mission;
            Path = path;
            Source = source;
            Bounty = source.Bounty;
            _Config = ShanghaiConfig.Instance;

            Mission.IsActive = true;
        }

        public bool Progress(float progress) {
            CurrentCellProgress += progress;
            Messenger<IntVect2, float>.Broadcast(EVENT_CELL_PROGRESSED, Path[CurrentCellID], CurrentCellProgress);

            if (CurrentCellProgress >= 1.0f) {
                CurrentCellProgress = 0.0f;
                CurrentCellID++;
                if (CurrentCellID >= Path.Count) {
                    if (Source.Bounty > _Config.PacketSize) {
                        Source.Bounty -= _Config.PacketSize;
                        GameModel.Instance.Money += _Config.PacketSize;
                        CurrentCellID = 0;
                        Messenger<List<IntVect2>, Source>.Broadcast(EVENT_PACKAGE_DELIVERED, Path, Source);
                    } else {
                        GameModel.Instance.Money += Source.Bounty;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
