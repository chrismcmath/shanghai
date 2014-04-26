using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Shanghai.Grid;

namespace Shanghai.Path {
    public class PathListener : MonoBehaviour {

        public void Awake() {
            Debug.Log("Awake");
        }

        public void OnClick() {
            Debug.Log("OnClick");
        }
    }
}
