using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Megamap {

    public class SubtaskPointing : MonoBehaviour {

        public SteamVR_Action_Boolean acceptAction;
        public SteamVR_Action_Boolean backAction;
        public LineRenderer laser;

        [SerializeField]
        private SteamVR_Input_Sources preferredHandType = SteamVR_Input_Sources.RightHand;
        private Hand hand;

        private readonly string taskDescription = "Zeige dorthin, wo sich der ausgewählte Raum befindet.\nBestätige die Richtung mit dem Trigger.";
        private readonly string confirmation = "Trigger: Annehmen\nTrackpad: Korrigieren";

        private bool laserLocked = false;

        private Task currentTask;
               
        private void OnEnable()
        {
            currentTask = FindObjectOfType<Task>();
            currentTask.Description = taskDescription;

            hand = GetTargetHand();
            laserLocked = false;
        }

        private void Update()
        {
            if (acceptAction.GetStateDown(hand.handType) || Input.GetMouseButtonDown(0)) {
                if (!laserLocked) {
                    laserLocked = true;
                    currentTask.Description = confirmation;
                } 
                else {
                    // TODO: Record data etc...
                    // ...

                    // Do next trial.
                    FindObjectOfType<TaskSwitcher>().NextTask();
                }
            }
            else if ((backAction.GetStateDown(hand.handType) || Input.GetKeyDown(KeyCode.Backspace)) && laserLocked) {
                laserLocked = false;
                currentTask.Description = taskDescription;
            }
        }

        // Need to use LateUpdate() to follow hand because otherwise hand's position will be constant
        // (probably because it is moved in a script each Update()).
        private void LateUpdate()
        {
            if (laserLocked)
                return;


            //if (!hand.gameObject.name.Equals("FallbackHand")) {
            //    laser.SetPosition(0, hand.transform.position);
            //    laser.SetPosition(1, hand.transform.TransformDirection(Vector3.forward * 2000));
            //}
            //else {
            //    laser.SetPosition(0, Camera.main.transform.position);
            //    laser.SetPosition(1, Camera.main.transform.position + Vector3.Normalize(hand.transform.position - Camera.main.transform.position) * 2000);
            //}

            laser.transform.position = hand.transform.position;
            laser.transform.rotation = hand.transform.rotation;
        }

        // TODO: Same code as in HandDisplay -> Refactor?
        private Hand GetTargetHand()
        {
            var hands = FindObjectsOfType<Hand>();
            if (hands.Length == 0) {
                return null;
            } else if (hands.Length == 1) {
                return hands[0];
            } else {
                // If preferred hand type was found (left or right handed), use it.
                // Otherwise, use the first hand found.
                foreach (Hand hand in hands) {
                    if (hand.handType == preferredHandType) {
                        return hand;
                    }
                }
                return hands[0];
            }
        }
    }

}