// This code is based on Unity's VR Sample Assets.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class EyeRaycaster : MonoBehaviour {

        public event Action<RaycastHit> OnRaycasthit;
        
        [SerializeField] private Transform cam;
        [SerializeField] private LayerMask exclusionLayers;
        [SerializeField] private GazeSelectionReticle reticle;
        [SerializeField] private bool showDebugRay = false;
        [SerializeField] private float debugRayLength = 5f;
        [SerializeField] private float debugRayDuration = 1f;
        [SerializeField] private float rayLength = 500f;

        private GazeTarget currentGazeTarget;
        private GazeTarget previousGazeTarget;

        private Coroutine selectionFillRoutine;

        public GazeTarget CurrentGazeTarget
        {
            get { return currentGazeTarget; }
        }

        private void OnEnable()
        {
            reticle.OnSelectionComplete += HandleOnSelectionComplete;
        }

        private void OnDisable()
        {
            reticle.OnSelectionComplete -= HandleOnSelectionComplete;
        }

        private void Update()
        {
            EyeRaycast();
        }

        private void EyeRaycast()
        {
            // Show the debug ray.
            if (showDebugRay) {
                Debug.DrawRay(cam.position, cam.forward * debugRayLength, Color.blue, debugRayDuration);
            }

            // Create forward ray from camera.
            Ray ray = new Ray(cam.position, cam.forward);

            // See whether raycast hits GazeTarget object.
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayLength, ~exclusionLayers)) {
                GazeTarget target = hit.collider.GetComponent<GazeTarget>();
                currentGazeTarget = target;

                // If we hit a GazeTarget and it's not the same as last time, ray is "over" the target.
                if (currentGazeTarget != previousGazeTarget) {

                    // Deactivate previous target.
                    DeactivatePreviousGazeTarget();

                    if (currentGazeTarget) {
                        currentGazeTarget.Over();
                        reticle.StartFilling();
                    }
                }
                
                previousGazeTarget = currentGazeTarget;

                if (OnRaycasthit != null) {
                    OnRaycasthit(hit);
                }
            }
            // Nothing was hit
            else {
                DeactivatePreviousGazeTarget();
                currentGazeTarget = null;
            }
        }

        private void DeactivatePreviousGazeTarget()
        {
            if (!previousGazeTarget) {
                return;
            }

            previousGazeTarget.Out();
            previousGazeTarget = null;
            reticle.StopFilling();
        }

        private void HandleOnSelectionComplete()
        {
            currentGazeTarget.Select();
        }
    }

}
