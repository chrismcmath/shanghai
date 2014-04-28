using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shanghai {
    public class ShanghaiConfig : MonoSingleton<ShanghaiConfig> {
        public float MissionInterval = 8.0f; //the 'agents' that appear on the map
        public float SourceInterval = 3.0f; //the telegrams that come in
        public float CellFillPerSecond = 0.5f; //the amount a cell is filled per second (1 is full)

        public int BountyMin = 50;
        public int BountyMax = 2000;

        public float MissionWaitTimeMedium = 5.0f;
        public float MissionWaitTimeDeviance = 2.0f;

        public int PacketSize = 100;

        /* Ministries */
        public float HealthIncPerSecond = 1f;
        public float MaxHealth = 100f;
        public float MinHealth = 10f;

        /* Embassies */
        public float ReputationIncOnMissionComplete = 20f;
        public float ReputationDecOnMissionFailed = 20f;
        public float ReputationDecPerSecond = 1f;
        public float MaxReputation = 100f;
        public float MinReputation = 10f;

        public float MissionFlagAlpha = 0.7f;
        public float MissionTargetAlpha = 0.5f;

        public Color EducationColour = new Color(0.0f, 0.0f, 0.0f);
        public Color EnvironmentColour = new Color(0.0f, 0.0f, 0.0f);
        public Color HealthColour = new Color(0.0f, 0.0f, 0.0f);
        public Color JusticeColour = new Color(0.0f, 0.0f, 0.0f);
        public Color TradeColour = new Color(0.0f, 0.0f, 0.0f);

        public Dictionary<string, Color> MinistryColours = new Dictionary<string, Color>();

        public void Awake() {
            MinistryColours.Add("education", EducationColour);
            MinistryColours.Add("environment", EnvironmentColour);
            MinistryColours.Add("health", HealthColour);
            MinistryColours.Add("justice", JusticeColour);
            MinistryColours.Add("trade", TradeColour);
        }
    }
}
