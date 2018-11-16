using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Megamap {

    public class SubtaskPointing : Subtask {

        public SteamVR_Action_Boolean acceptAction;
        public SteamVR_Action_Boolean backAction;
        public LaserPointer laser;

        private readonly string taskDescription = "Zeige dorthin, wo sich der ausgewählte Raum befindet.\nBestätige die Richtung mit dem Trigger.";
        private readonly string confirmation = "Trigger: Annehmen\nTrackpad: Korrigieren";

        private Task currentTask;

        private float startTime = 0f;
        private float startConfirmationTime = 0f;
        
        private void OnEnable()
        {
            currentTask = FindObjectOfType<Task>();
            currentTask.Description = taskDescription;
            laser.Show(true);

            startTime = Time.realtimeSinceStartup;
        }

        private void Update()
        {
            var hand = laser.GetHand();
            if (acceptAction.GetStateDown(hand.handType) || Input.GetMouseButtonDown(0)) {
                if (!laser.IsFrozen) {
                    laser.IsFrozen = true;
                    currentTask.Description = confirmation;

                    startConfirmationTime = Time.realtimeSinceStartup;
                } 
                else {
                    float endTime = Time.realtimeSinceStartup;
                    var recorder = FindObjectOfType<RecordData>();
                    recorder.CurrentRecord.pointingTime = endTime - startTime;
                    recorder.CurrentRecord.confirmationTime = endTime - startConfirmationTime;
                    recorder.CurrentRecord.positionAtConfirmation = Camera.main.transform.position;
                    recorder.CurrentRecord.viewAtConfirmation = Camera.main.transform.rotation.eulerAngles;
                    recorder.CurrentRecord.rayPosition = laser.Ray.origin;
                    recorder.CurrentRecord.rayDirection = laser.Ray.direction;
                    // TODO: Calculate horizontal/vertical error...

                    // Do next trial.
                    laser.Show(false);
                    FindObjectOfType<TaskSwitcher>().NextTask();
                }
            }
            else if ((backAction.GetStateDown(hand.handType) || Input.GetKeyDown(KeyCode.Backspace)) && laser.IsFrozen) {
                laser.IsFrozen = false;
                currentTask.Description = taskDescription;

                var recorder = FindObjectOfType<RecordData>();
                ++recorder.CurrentRecord.numCorrections;
            }
        }
    }

}