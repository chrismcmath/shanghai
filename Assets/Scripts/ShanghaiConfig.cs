using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shanghai {
    public class ShanghaiConfig : MonoSingleton<ShanghaiConfig> {
        public float MissionInterval = 2.0f;
        public float CellFillPerSecond = 0.5f;

        public int BountyMedium = 1000;
        public int BountyDeviance = 1000;

        public float MissionWaitTimeMedium = 5.0f;
        public float MissionWaitTimeDeviance = 2.0f;

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
