using System;

using UnityEngine;

using Valve.VR;

namespace Megamap {

    public class SubtaskPointing : Subtask {

        public SteamVR_Action_Boolean acceptAction;
        public SteamVR_Action_Boolean backAction;
        public LaserPointer laser = null;

        private readonly string taskDescription = "1. ZEIGE, wo sich der Raum in deiner Umgebung befindet.\n(Ziele auf die Mitte des Raums).\n\n2. BESTÄTIGE die Richtung mit dem TRIGGER.";
        private readonly string confirmation = "TRIGGER: Annehmen\n\nTRACKPAD: Korrigieren";

        private float startTime = 0f;
        private float startConfirmationTime = 0f;

        private SelectRoom targetRoom = null;

        public override void StartSubtask()
        {
            LogSubtask();
            FindObjectOfType<TaskDisplay>().Description = taskDescription;

            laser.Show(true);
            laser.IsFrozen = false;

            // We want to enable the target room so that we can use its BoxCollider of Raycasts (otherwise, its size would be 0).
            // All other rooms are disabled to prevent rendering collisions/z-fighting with the virtual lab.
            DisplayTargetRoom(true);
            targetRoom = FindObjectOfType<TaskSwitcher>().CurrentTask.Megamap.TargetRoom;

            startTime = Time.realtimeSinceStartup;
        }

        public override void StopSubtask()
        {
            DisplayTargetRoom(false);
            laser.IsFrozen = false;
            laser.Show(false);
        }

        private void Awake()
        {
            laser = FindObjectOfType<LaserPointer>();
        }

        private void Start()
        {
            laser.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!IsStarted)
                return;

            var hand = laser.GetHand();
            if (acceptAction.GetStateDown(hand.handType) || Input.GetMouseButtonDown(0)) {
                if (!laser.IsFrozen) {
                    laser.IsFrozen = true;
                    FindObjectOfType<TaskDisplay>().Description = confirmation;

                    startConfirmationTime = Time.realtimeSinceStartup;
                }
                else {
                    float endTime = Time.realtimeSinceStartup;

                    RecordData.CurrentRecord.pointingTime = endTime - startTime;
                    RecordData.CurrentRecord.confirmationTime = endTime - startConfirmationTime;
                    RecordData.CurrentRecord.positionAtConfirmation = Camera.main.transform.position;
                    RecordData.CurrentRecord.viewAtConfirmation = Camera.main.transform.rotation.eulerAngles;
                    RecordData.CurrentRecord.rayPosition = laser.Ray.origin;
                    RecordData.CurrentRecord.rayDirection = laser.Ray.direction;

                    var hits = Physics.RaycastAll(laser.Ray, 200f);
                    foreach (var hit in hits) {
                        // Go through all hits. If hit is room AND room is target, record related data.
                        // Second condition should always be true since we disabled all room colliders except for the target room.
                        if (hit.collider.GetComponent<SelectRoom>() != null && hit.collider.GetComponent<SelectRoom>().IsTargetRoom) {
                            RecordData.CurrentRecord.hitRoom = true;
                            RecordData.CurrentRecord.hitLocation = hit.point - hit.transform.position;
                            break;
                        }
                    }

                    var roomBounds = targetRoom.GetComponent<Renderer>().bounds;
                    // Determine pointing error towards room center.
                    Vector3 correctDir = (roomBounds.center - laser.Ray.origin).normalized;
                    Vector3 actualDir = laser.Ray.direction.normalized;

                    // Positive horizontal angle means error to the right of center (from user POV), negative means left.
                    Vector2 correctProjHoriz = new Vector2(correctDir.z, correctDir.x);
                    Vector2 actualProjHoriz = new Vector2(actualDir.z, actualDir.x);
                    float horizAngle = Vector2.SignedAngle(correctProjHoriz, actualProjHoriz);

                    // Positive horizontal angle means error over center (from user POV), negative means under.
                    Vector2 correctProjVert = new Vector2(correctDir.y, correctDir.z);
                    Vector2 actualProjVert = new Vector2(actualDir.y, actualDir.z);
                    float vertAngle = Vector2.SignedAngle(correctProjVert, actualProjVert);

                    RecordData.CurrentRecord.horizOffsetDeg = horizAngle;
                    RecordData.CurrentRecord.vertOffsetDeg = vertAngle;

                    RecordData.Log("Pointing error in degrees: " + horizAngle + ", " + vertAngle);

                    // Do next trial.
                    FindObjectOfType<TaskSwitcher>().NextTask();
                }
            }
            else if ((backAction.GetStateDown(hand.handType) || Input.GetKeyDown(KeyCode.Backspace)) && laser.IsFrozen) {
                laser.IsFrozen = false;
                FindObjectOfType<TaskDisplay>().Description = taskDescription;

                ++RecordData.CurrentRecord.numCorrections;
            }
        }

        private void OnDrawGizmos()
        {
            if (targetRoom == null)
                return;

            var roomBounds = targetRoom.GetComponent<Renderer>().bounds;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(roomBounds.center, roomBounds.size);

            if (laser != null) {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(laser.Ray.origin, roomBounds.center);
            }

        }

        private void DisplayTargetRoom(bool enable)
        {
            var map = FindObjectOfType<TaskSwitcher>().CurrentTask.Megamap;

            map.gameObject.SetActive(enable);

            // Disable/Enable colliders for all selectable rooms and balls.

            // FIXME: This seems to not always reliably disable the colliders?
            // It's not a severe problem for now because when raycasting we cast for all
            // colliders and only act when the collider of the targetRoom is found
            // anyway.

            foreach (var room in map.SelectableRooms) {
                var colls = room.GetComponentsInChildren<Collider>(true);
                Array.ForEach(colls, c => c.enabled = !enable);
            }
            // Re-enable collider for target room so it can be used for raycasting.
            map.TargetRoom.GetComponent<Collider>().enabled = true;

            // Make only target room visible.
            ChangeVisibility.SetVisible(map.gameObject, !enable);
            ChangeVisibility.SetVisible(map.TargetRoom.gameObject, true);
            var marker = GameObject.Find("UserMarker Circle");
            if (marker != null)
                ChangeVisibility.SetVisible(marker, false);
        }
    }

}
