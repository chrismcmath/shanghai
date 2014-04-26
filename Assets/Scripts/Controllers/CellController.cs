using UnityEngine;
using System.Collections;

namespace Shanghai.Controllers {
    public class CellController : MonoBehaviour {
        public static readonly string EVENT_CELL_DRAGGED = "EVENT_CELL_DRAGGED";
        public static readonly string EVENT_CELL_DRAG_END = "EVENT_CELL_DRAG_END";

        public static readonly string PIPE_PREFIX = "default";
        public static readonly string CLIENT_PREFIX = "mission";
        public static readonly string TARGET_PREFIX = "target";
        public static readonly string OBSTACLE_PREFIX = "obstacle";

        public IntVect2 Key;
        private GameObject _CurrentObject = null;
        private bool _IsDragging = false;
        public UISprite PipeSprite;
        public UISprite ClientSprite;
        public UISprite TargetSprite;

        public void UpdateCell(string pipeState, string clientState, string targetState) {
            UpdateSprite(PipeSprite, PIPE_PREFIX, pipeState);
            UpdateSprite(ClientSprite, CLIENT_PREFIX, clientState);
            UpdateSprite(TargetSprite, TARGET_PREFIX, targetState);
        }

        private void UpdateSprite(UISprite sprite, string prefix, string state) {
            if (state == "") { 
                sprite.alpha = 0;
            } else {
                sprite.alpha = 1;
                sprite.spriteName = string.Format("{0}_{1}", prefix, state);
            }
        }

        public void Update() {
            //Debug.Log("UICamera hovered object: " + UICamera.hoveredObject);
            //Debug.Log("isDragging: " + UICamera.isDragging);
        }

        public void OnDrag(Vector2 delta) {
            if (!_IsDragging) {
                Messenger<IntVect2>.Broadcast(EVENT_CELL_DRAGGED, Key);
                _IsDragging = true;
            }
        }

        public void OnDragOver(GameObject go) {
            Messenger<IntVect2>.Broadcast(EVENT_CELL_DRAGGED, Key);
        }

        public void OnDragEnd() {
            Messenger.Broadcast(EVENT_CELL_DRAG_END);
            _IsDragging = false;
        }
    }
}
