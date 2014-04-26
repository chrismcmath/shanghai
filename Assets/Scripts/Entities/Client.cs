using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Shanghai.Entities {
    public class Client : Entity {
        private int _Reputation = 100;

        public Client(string id) : base (id) {
        }
    }
}
