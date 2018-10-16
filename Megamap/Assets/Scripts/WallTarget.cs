using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class WallTarget : MonoBehaviour {

        private bool onTarget = false;
        public bool OnTarget
        { get { return onTarget; } }
        
        void Update()
        {
            // Only execute when player should look at wall target.
            var floorTarget = FindObjectOfType<FloorTarget>();
            if (floorTarget == null || !floorTarget.OnTarget) {
                onTarget = false;
                return;
            }

            var cam = Camera.main;
            RaycastHit hit;
            // Is the player looking at this wall target? -> Start with search task.
            if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity)) {
                onTarget = hit.collider.gameObject.Equals(gameObject);
            }
        }
    }

}
