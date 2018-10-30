using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Megamap {

    public class WallTarget : MonoBehaviour {

        public UnityEvent OnTargetEnter = new UnityEvent();
        public UnityEvent OnTargetStay = new UnityEvent();
        public UnityEvent OnTargetExit = new UnityEvent();

        private bool onTarget = false;

        private void OnEnable()
        {
            onTarget = false;
        }

        private void Update()
        {
            var cam = Camera.main;
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 50.0f, Color.magenta, 3.0f);

            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity)) {
                if (hit.collider.gameObject.Equals(gameObject)) {
                    if (!onTarget) {
                        onTarget = true;
                        OnTargetEnter.Invoke();
                    }
                    else {
                        OnTargetStay.Invoke();
                    }
                }
            }
            else {
                if (onTarget) {
                    onTarget = false;
                    OnTargetExit.Invoke();
                }
            }
        }
    }

}
