using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Megamap {

    public class HandDisplay : MonoBehaviour {

        [SerializeField]
        private SteamVR_Input_Sources preferredHandType = SteamVR_Input_Sources.LeftHand;

        // Need to use LateUpdate() to follow hand because otherwise hand's position will be constant
        // (probably because it is moved in a script each Update()).
        private void LateUpdate()
        {
            var hand = GetTargetHand();

            transform.position = hand.transform.position + hand.transform.TransformDirection(Vector3.up * 0.1f);
            transform.rotation = hand.transform.rotation;
        }

        private Hand GetTargetHand()
        {
            var hands = FindObjectsOfType<Hand>();
            if (hands.Length == 0) {
                return null;
            }
            else if (hands.Length == 1) {
                return hands[0];
            }
            else {
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
