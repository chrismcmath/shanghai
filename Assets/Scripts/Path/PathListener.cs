using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Shanghai.Grid;

namespace Shanghai.Path {
    public class PathListener : MonoBehaviour {

        public void Awake() {
            Debug.Log("Awake");
        }

        public void OnHover(bool hover) {
            Debug.Log("OnHover " + hover);
        }

        public void OnDrag(Vector2 delta) {
            Debug.Log("OnDrag " + delta);
        }
    }
}
