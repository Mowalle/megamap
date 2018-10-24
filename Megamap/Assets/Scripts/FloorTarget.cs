using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Megamap {

    public class FloorTarget : MonoBehaviour {

        public UnityEvent OnTargetEnter = new UnityEvent();
        public UnityEvent OnTargetExit = new UnityEvent();
        
        private void OnTriggerEnter(Collider other)
        {
            OnTargetEnter.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            OnTargetExit.Invoke();
        }
    }

}
