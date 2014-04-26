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

        public void Awake() {
            Debug.Log("Awake");
        }

        public void OnClick() {
            Debug.Log("OnClick");
        }

        public void OnPress(bool press) {
            Debug.Log("OnPress " + press);
        }

        public void OnHover(bool hover) {
            Debug.Log("OnHover " + hover);
        }

        public void OnDrag(Vector2 delta) {
            Debug.Log("OnDrag " + delta);
        }
    }
}
