using System.Collections;
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

            if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None) {
                bool alreadyShown = pin.IsInfoShown();
                LocationPin.HideAllInfos();
                if (!alreadyShown) {
                    pin.ShowInfo(true);
                }
            }
            //else if (isGrabEnding) {
            //}
        }
    }

}
