using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Shanghai.Grid;

namespace Shanghai.Controllers {
    public class MusicController : MonoBehaviour {

        public List<AudioSource> _Sources;

        private int _CurrentSource;

        public void Awake() {
            _CurrentSource = Random.Range(0, _Sources.Count -1);
            _Sources[_CurrentSource].Play();
        }

        public void Update() {
            if (!_Sources[_CurrentSource].isPlaying) {
                _CurrentSource++;
                _CurrentSource = _CurrentSource >= _Sources.Count ? 0 : _CurrentSource;
                _Sources[_CurrentSource].Play();
            }
        }
    }
}
