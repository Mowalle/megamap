using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Megamap {

    [RequireComponent(typeof(Interactable), typeof(LocationPin))]
    public class LocationPinInteraction : MonoBehaviour {

        private Interactable interactable;
        private LocationPin locationPin;

        private void Awake()
        {
            interactable = GetComponent<Interactable>();
            locationPin = GetComponent<LocationPin>();
        }

        private void OnHandHoverBegin(Hand hand)
        {
            locationPin.ShowLocationPinText(true);
        }

        private void OnHandHoverEnd(Hand hand)
        {
            locationPin.ShowLocationPinText(false);
        }
    }

}
