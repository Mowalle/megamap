using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Megamap {

    [System.Serializable]
    public class LaserEvent : UnityEvent<RaycastHit> {}

    [RequireComponent(typeof(LineRenderer))]
    public class LaserPointer : MonoBehaviour {

        public SteamVR_Input_Sources preferredHand;
        
        private LineRenderer line;
        private Hand hand;

        private void Awake()
        {
            line = GetComponent<LineRenderer>();
        }

        private void OnEnable()
        {
            StartCoroutine("LinkToHand");
        }

        private void OnDisable()
        {
            StopCoroutine("LinkToHand");
        }

        // Have to use LateUpdate because hand position is updated via script, which is too late for Update apparently.
        private void LateUpdate()
        {
            Vector3 start, dir;
            if (hand.gameObject.name.Equals("FallbackHand")) {
                start = Camera.main.transform.position;
                dir = (hand.transform.position - start).normalized;
            }
            else {
                start = hand.transform.position;
                dir = hand.transform.forward;
            }

            Ray ray = new Ray(start, dir);
            RaycastHit hit;

            line.SetPosition(0, ray.origin);
            if (Physics.Raycast(ray, out hit, 100f)) {
                line.SetPosition(1, hit.point);
                // This should *theoretically* move the hover point of the hand,
                // so that SteamVR-Interactables at the end of the ray should be
                // clickable etc.
                hand.hoverSphereTransform.position = hit.point;
            }
            else {
                line.SetPosition(1, ray.GetPoint(100f));
            }
        }

        private IEnumerator LinkToHand()
        {
            while (true) {
                var hands = FindObjectsOfType<Hand>();
                if (hands.Length == 0)
                    continue;

                hand = hands[0];
                foreach (Hand h in hands) {
                    if (h.handType == preferredHand) {
                        hand = h;
                        break;
                    }
                }
                yield return null;
            }
        }
    }

}
