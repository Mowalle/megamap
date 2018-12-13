using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Megamap {

    [System.Serializable]
    public class LaserStayEvent : UnityEvent<Collider> { }
    public class LaserChangeEvent : UnityEvent<Collider, Collider> { }

    [RequireComponent(typeof(LineRenderer))]
    public class LaserPointer : MonoBehaviour {

        public SteamVR_Input_Sources preferredHand;

        [SerializeField] private Material normalMaterial = null;
        [SerializeField] private Material frozenMaterial = null;

        public LaserChangeEvent OnLaserTargetChanged = new LaserChangeEvent();
        public LaserStayEvent OnLaserStay = new LaserStayEvent();

        private Collider lastHit = null;

        private LineRenderer line;
        private Hand hand;
        public Hand GetHand() { return hand; }

        private bool isFrozen = false;
        public bool IsFrozen
        {
            get { return isFrozen; }
            set { Freeze(value); }
        }

        private Ray ray = new Ray();
        public Ray Ray { get { return ray; } }

        private Coroutine linkToHandRoutine = null;

        public void Freeze(bool freeze)
        {
            isFrozen = freeze;

            line.material = isFrozen ? frozenMaterial : normalMaterial;
        }

        public void Show(bool show)
        {
            // Reset freeze status when hiding.
            if (!show)
                IsFrozen = false;

            line.enabled = show;
        }

        private void Awake()
        {
            line = GetComponent<LineRenderer>();
        }

        // Have to use LateUpdate because hand position is updated via script, which is too late for Update apparently.
        private void LateUpdate()
        {
            if (isFrozen)
                return;

            LinkToHand();

            if (hand == null)
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

            ray = new Ray(start, dir);
            RaycastHit hit;

            line.SetPosition(0, ray.origin);
            if (Physics.Raycast(ray, out hit, 100f) && !hit.collider.isTrigger) {
                line.SetPosition(1, hit.point);
                // This should *theoretically* move the hover point of the hand,
                // so that SteamVR-Interactables at the end of the ray should be
                // clickable etc.
                hand.hoverSphereTransform.position = hit.point;

                if (lastHit != hit.collider) {
                    OnLaserTargetChanged.Invoke(lastHit, hit.collider);
                    lastHit = hit.collider;
                }
                else {
                    OnLaserStay.Invoke(hit.collider);
                }
            }
            else {
                line.SetPosition(1, ray.GetPoint(100f));
                if (lastHit != null) {
                    OnLaserTargetChanged.Invoke(lastHit, null);
                }
                lastHit = null;
            }
        }

        private void LinkToHand()
        {
            var hands = FindObjectsOfType<Hand>();
            if (hands.Length == 0) {
                hand = null;
                return;
            }

            hand = hands[0];
            foreach (Hand h in hands) {
                if (h.handType == preferredHand) {
                    hand = h;
                    return;
                }
            }
        }
    }

}
