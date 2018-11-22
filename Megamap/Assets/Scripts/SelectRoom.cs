using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Valve.VR.InteractionSystem;

namespace Megamap {

    [System.Serializable]
    public class RoomSelectionEvent : UnityEvent<SelectRoom> { }

    [RequireComponent(typeof(BoxCollider), typeof(Interactable))]
    public class SelectRoom : MonoBehaviour {

        [SerializeField] private bool isTargetRoom = false;
        public bool IsTargetRoom { get { return isTargetRoom; } }

        private List<GameObject> balls = new List<GameObject>();
        public List<GameObject> Balls { get { return balls; } }

        public RoomSelectionEvent OnTargetRoomSelected = new RoomSelectionEvent();
        public RoomSelectionEvent OnWrongRoomSelected = new RoomSelectionEvent();

        private Material normalMaterial = null;
        private Material hoverMaterial = null;
        private Material errorMaterial = null;
        private Material Material
        {
            set {
                var renderers = GetComponentsInChildren<Renderer>();
                foreach (var r in renderers) {
                    if (r.gameObject.name.StartsWith("Room") || r.gameObject.name.StartsWith("Baseboard")) {
                        r.material = value;
                    }
                }
            }
        }

        private bool enableInteraction = true;
        private bool wasClicked = false;
        private SelectRoomConfiguration config = null;

        public void ResetMaterial()
        {
            Material = normalMaterial;
        }

        public void EnableInteraction(bool enable)
        {
            enableInteraction = enable;
        }

        private void GenerateBalls()
        {
            if (config.ballPrefab.GetComponentInChildren<SphereCollider>() == null
                || config.ballPrefab.GetComponentInChildren<Rigidbody>() == null) {
                GetComponent<Interactable>().enabled = false;
                enabled = false;
                return;
            }

            int numBalls = isTargetRoom ? config.NumBallsTargetRoom : Random.Range(config.ballMinimum, config.NumBallsTargetRoom);

            if (balls.Count != 0) {
                foreach (var go in balls) { Destroy(go); }
            }
            balls.Clear();

            var roomBounds = GetComponent<Collider>().bounds;
            for (int i = 0; i < numBalls; ++i) {
                var ball = Instantiate(config.ballPrefab, transform);
                ball.transform.rotation = Quaternion.Euler(Vector3.zero);
                balls.Add(ball);
                do {
                    PlaceRandomlyInRoom(ball.GetComponentInChildren<SphereCollider>(), roomBounds);
                } while (IsOverlapping(ball, balls));
                ball.transform.localRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            }
        }

        private void PlaceRandomlyInRoom(SphereCollider coll, Bounds room)
        {
            // To correctly place the balls in the room, we have to scale both by the Megamap's scale
            // and the ball's transform's scale (because the radius is given in LOCAL SPACE).
            float scaledRadius = coll.radius * coll.transform.localScale.y * FindObjectOfType<Megamap>().transform.localScale.y;
            Vector3 range = room.max - room.min;

            float x = Random.Range(scaledRadius, range.x - scaledRadius);
            float z = Random.Range(scaledRadius, range.z - scaledRadius);
            float y = scaledRadius;

            coll.transform.position = new Vector3(room.min.x + x, room.min.y + y, room.min.z + z);
        }

        private bool IsOverlapping(GameObject ball, List<GameObject> balls)
        {
            foreach (var b in balls) {
                if (b.Equals(ball))
                    continue;

                if (ball.GetComponent<Collider>().bounds.Intersects(b.GetComponent<Collider>().bounds))
                    return true;
            }

            return false;
        }

        private void Awake()
        {
            GetComponent<Interactable>().highlightOnHover = false;

            config = FindObjectOfType<SelectRoomConfiguration>();
            if (config == null) {
                GetComponent<Interactable>().enabled = false;
                enabled = false;
                return;
            }

            normalMaterial = config.normalMaterial;
            hoverMaterial = config.hoverMaterial;
            errorMaterial = config.errorMaterial;
        }

        private void OnEnable()
        {
            GenerateBalls();
            ResetMaterial();
            wasClicked = false;
        }

        private void OnHandHoverBegin(Hand hand)
        {
            if (!enableInteraction)
                return;

            if (wasClicked && !isTargetRoom)
                Material = errorMaterial;
            else
                Material = hoverMaterial;

            var map = FindObjectOfType<Megamap>();
            var rooms = map.GetComponentsInChildren<SelectRoom>();
            for (int i = 0; i < rooms.Length; ++i) {
                if (Equals(rooms[i])) {
                    ++RecordData.CurrentRecord.roomSelections[i];
                    break;
                }
            }
            ++RecordData.CurrentRecord.numRoomSelections;
        }

        private void OnHandHoverEnd(Hand hand)
        {
            if (!enableInteraction)
                return;

            Material = normalMaterial;
        }

        private void HandHoverUpdate(Hand hand)
        {
            if (!enableInteraction)
                return;

            if (!wasClicked && (hand.GetGrabStarting() != GrabTypes.None || Input.GetMouseButtonDown(0))) {
                wasClicked = true;

                if (isTargetRoom) {
                    OnTargetRoomSelected.Invoke(this);
                }
                else {
                    OnWrongRoomSelected.Invoke(this);
                    Material = errorMaterial;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = isTargetRoom ? Color.green : Color.red;
            var coll = GetComponent<Collider>();
            Gizmos.DrawWireCube(coll.bounds.center, coll.bounds.size * 0.8f);
        }
    }

}