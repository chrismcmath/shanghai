using UnityEngine;
using System.Collections;

namespace Shanghai.Controllers {
    public class CellController : MonoBehaviour {
        public static readonly string PIPE_PREFIX = "default";
        public static readonly string CLIENT_PREFIX = "mission";
        public static readonly string TARGET_PREFIX = "target";
        public static readonly string OBSTACLE_PREFIX = "obstacle";

        public void UpdateCell(string pipeState, string clientState, string targetState) {
            Debug.Log("update cell");
        }
    }
}
