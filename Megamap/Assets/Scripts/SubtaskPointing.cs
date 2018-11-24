using UnityEngine;

using Valve.VR;

namespace Megamap {

    public class SubtaskPointing : Subtask {

        public SteamVR_Action_Boolean acceptAction;
        public SteamVR_Action_Boolean backAction;
        public LaserPointer laser = null;

        private readonly string taskDescription = "Zeige dorthin, wo sich der ausgewählte Raum befindet.\nBestätige die Richtung mit dem Trigger.";
        private readonly string confirmation = "Trigger: Annehmen\nTrackpad: Korrigieren";

        private float startTime = 0f;
        private float startConfirmationTime = 0f;

        public override void StartSubtask()
        {
            LogSubtask();
            FindObjectOfType<TaskDisplay>().Description = taskDescription;
            laser.Show(true);
            laser.IsFrozen = false;

            startTime = Time.realtimeSinceStartup;
        }

        public override void StopSubtask()
        {
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
                    // TODO: Calculate horizontal/vertical error...

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
    }

}