using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Megamap {

    public class SubtaskPointing : MonoBehaviour {

        public SteamVR_Action_Boolean acceptAction;
        public SteamVR_Action_Boolean backAction;
        public LaserPointer laser;

        private readonly string taskDescription = "Zeige dorthin, wo sich der ausgewählte Raum befindet.\nBestätige die Richtung mit dem Trigger.";
        private readonly string confirmation = "Trigger: Annehmen\nTrackpad: Korrigieren";

        private Task currentTask;
        
        private void OnEnable()
        {
            currentTask = FindObjectOfType<Task>();
            currentTask.Description = taskDescription;
            laser.Show(true);
        }

        private void Update()
        {
            var hand = laser.GetHand();
            if (acceptAction.GetStateDown(hand.handType) || Input.GetMouseButtonDown(0)) {
                if (!laser.IsFrozen) {
                    laser.IsFrozen = true;
                    currentTask.Description = confirmation;
                } 
                else {
                    // TODO: Record data etc...
                    // ...

                    // Do next trial.
                    laser.Show(false);
                    FindObjectOfType<TaskSwitcher>().NextTask();
                }
            }
            else if ((backAction.GetStateDown(hand.handType) || Input.GetKeyDown(KeyCode.Backspace)) && laser.IsFrozen) {
                laser.IsFrozen = false;
                currentTask.Description = taskDescription;
            }
        }
    }

}