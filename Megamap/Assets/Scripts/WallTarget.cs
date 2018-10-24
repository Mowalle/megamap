using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class WallTarget : MonoBehaviour {

        private bool onTarget = false;
        public bool OnTarget
        { get { return onTarget; } }

        private void OnEnable()
        {
            onTarget = false;
        }

        private void Update()
        {
            // Only execute when player should look at wall target.
            var floorTarget = FindObjectOfType<FloorTarget>();
            if (floorTarget == null || !floorTarget.OnTarget) {
                onTarget = false;
                return;
            }

            var cam = Camera.main;
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 50.0f, Color.magenta, 3.0f);
            RaycastHit hit;
            // Is the player looking at this wall target? -> Start with search task.
            if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity)) {
                onTarget = hit.collider.gameObject.Equals(gameObject);
            }
        }
    }

}
