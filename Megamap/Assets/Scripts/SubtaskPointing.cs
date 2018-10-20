using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Megamap {

    public class SubtaskPointing : MonoBehaviour {

        public SteamVR_Action_Boolean action;
        public LineRenderer laser;

        private string taskDescription = "Zeige dorthin, wo sich der ausgewählte Raum befindet.";
        private string confirmation = "Zum Bestätigen, Trigger betätigen.";

        [SerializeField]
        private SteamVR_Input_Sources preferredHandType = SteamVR_Input_Sources.RightHand;
               
        private void OnEnable()
        {
            FindObjectOfType<TaskSwitcher>().SetTaskDescription(taskDescription);
        }

        // Need to use LateUpdate() to follow hand because otherwise hand's position will be constant
        // (probably because it is moved in a script each Update()).
        private void LateUpdate()
        {
            var hand = GetTargetHand();
            laser.SetPosition(0, hand.transform.position);
            laser.SetPosition(1, hand.transform.TransformDirection(Vector3.forward * 2000));
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