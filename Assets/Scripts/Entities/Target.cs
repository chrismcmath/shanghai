using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shanghai.Entities {
    public class Target : Entity {
        private int _Health = 100;

        public Target() {}
        public Target(string id) : base (id) {
        }
    }
}
