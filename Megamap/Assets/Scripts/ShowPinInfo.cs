﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Megamap {

    [RequireComponent(typeof(Interactable))]
    public class ShowPinInfo : MonoBehaviour {

        [SerializeField]
        private LocationPin pin;

        private Interactable interactable;
        
        private void Awake() 
        {
            interactable = GetComponent<Interactable>();
        }

        //-------------------------------------------------
        // Called when a Hand starts hovering over this object
        //-------------------------------------------------
        private void OnHandHoverBegin(Hand hand)
        {
        }


        //-------------------------------------------------
        // Called when a Hand stops hovering over this object
        //-------------------------------------------------
        private void OnHandHoverEnd(Hand hand)
        {
        }


        //-------------------------------------------------
        // Called every Update() while a Hand is hovering over this object
        //-------------------------------------------------
        private void HandHoverUpdate(Hand hand)
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();
            //bool isGrabEnding = hand.IsGrabEnding(this.gameObject);

            // The GetMouseButtonDown(0) is a workaround for left-click not working currently with SteamVRs fallback hand (in 2D-mode).
            if ((interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
                || Input.GetMouseButtonDown(0)) {
                bool shownBefore = pin.IsInfoShown();

                // Hide all other pins.
                foreach (LocationPin lPin in FindObjectsOfType<LocationPin>()) {
                    lPin.ShowInfo(false);
                }
                if (!shownBefore) {
                    pin.ShowInfo(true);
                }
            }
            //else if (isGrabEnding) {
            //}
        }
    }

}
