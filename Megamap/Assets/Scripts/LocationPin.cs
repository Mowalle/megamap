using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class LocationPin : MonoBehaviour {

        [SerializeField]
        private GameObject locationPinText;

        // TODO: Rename this to something more semantic.
        public int attribute = 0;

        private int roomNumber;

        public void ShowLocationPinText(bool show)
        {
            locationPinText.SetActive(show);
        }

        private void Start()
        {
            if (locationPinText == null) {
                Debug.LogError("LocationPin: locationPinText reference not set; disabling script.");
                enabled = false;
                return;
            }

            roomNumber = Random.Range(100, 1000);
            var text = locationPinText.GetComponentInChildren<Text>();
            text.text = "Room " + roomNumber + "\nAttribute: " + attribute;

            ShowLocationPinText(false);
        }
    }

}
