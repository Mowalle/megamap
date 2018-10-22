using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class FloorTarget : MonoBehaviour {

        private bool onTarget = false;
        public bool OnTarget
        { get { return onTarget; } }

        private void OnEnable()
        {
            onTarget = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            onTarget = true;
        }

        private void OnTriggerExit(Collider other)
        {
            onTarget = false;
        }
    }

}
