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

        [SerializeField] private Material normalMaterial = null;
        [SerializeField] private Material frozenMaterial = null;

        private LineRenderer line;
        private Hand hand;
        public Hand GetHand() {  return hand; }

        private bool isFrozen = false;
        public bool IsFrozen
        {
            get { return isFrozen; }
            set { Freeze(value); }
        }

        public void Freeze(bool freeze)
        {
            isFrozen = freeze;

            line.material = isFrozen ? frozenMaterial : normalMaterial;
        }

        private void Awake()
        {
            line = GetComponent<LineRenderer>();
            line.enabled = false;
            IsFrozen = false;
        }

        private void OnEnable()
        {
            IsFrozen = false;
            line.enabled = true;
            StartCoroutine("LinkToHand");
        }

        private void OnDisable()
        {
            line.enabled = false;
            StopCoroutine("LinkToHand");
        }

        // Have to use LateUpdate because hand position is updated via script, which is too late for Update apparently.
        private void LateUpdate()
        {
            if (isFrozen)
                return;

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
