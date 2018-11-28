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

        public void ResetRoom()
        {
            Material = normalMaterial;
            wasClicked = false;
        }

        public void EnableInteraction(bool enable)
        {
            enableInteraction = enable;
        }

        public void GenerateBalls()
        {
            var config = FindObjectOfType<SelectRoomConfiguration>();
            int numBalls = isTargetRoom ? config.NumBallsTargetRoom : Random.Range(config.ballMinimum, config.NumBallsTargetRoom);

            if (balls.Count != 0) {
                foreach (var go in balls) { Destroy(go); }
            }
            balls.Clear();

            var roomBounds = GetComponent<Renderer>().bounds;
            for (int i = 0; i < numBalls; ++i) {
                var ball = Instantiate(config.ballPrefab, transform);
                ball.transform.rotation = Quaternion.Euler(Vector3.zero);
                balls.Add(ball);
                do {
                    PlaceRandomlyInRoom(ball, roomBounds);
                } while (IsOverlapping(ball, balls));
                ball.transform.localRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            }
        }

        private void PlaceRandomlyInRoom(GameObject go, Bounds room)
        {
            // To correctly place the balls in the room, we have to scale both by the Megamap's scale
            Vector3 range = room.max - room.min;

            var objBounds = go.GetComponent<Renderer>().bounds;

            float x = Random.Range(objBounds.extents.x, range.x - objBounds.extents.x);
            float z = Random.Range(objBounds.extents.z, range.z - objBounds.extents.z);
            float y = objBounds.extents.y;

            go.transform.position = new Vector3(room.min.x + x, room.min.y + y, room.min.z + z);
        }

        private bool IsOverlapping(GameObject ball, List<GameObject> balls)
        {
            foreach (var b in balls) {
                if (b.Equals(ball))
                    continue;

                if (ball.GetComponent<Renderer>().bounds.Intersects(b.GetComponent<Renderer>().bounds))
                    return true;
            }

            return false;
        }

        private void Awake()
        {
            GetComponent<Interactable>().highlightOnHover = false;

            var config = FindObjectOfType<SelectRoomConfiguration>();
            normalMaterial = config.normalMaterial;
            hoverMaterial = config.hoverMaterial;
            errorMaterial = config.errorMaterial;

            ResetRoom();
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