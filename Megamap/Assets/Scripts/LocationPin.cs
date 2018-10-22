using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

namespace Megamap {

    [System.Serializable]
    public class LocationPinEvent : UnityEvent<LocationPin> { }

    public class LocationPin : MonoBehaviour {

        public LocationPinEvent OnSelected = new LocationPinEvent();

        public enum Status { Normal, Error }

        [SerializeField]
        private GameObject locationPinInfo;

        public bool isTargetPin = false;

        [SerializeField]
        private Button acceptButton;
        [SerializeField]
        private Color normalColor;
        [SerializeField]
        private Color errorColor;

        // TODO: Rename this to something more semantic.
        public int attribute = 0;
        public int roomNumber;

        public void Select()
        {
            OnSelected.Invoke(this);
        }

        public void ShowInfo(bool show)
        {
            if (show) {
                // Reset info display in case pin was wrongly selected before.
                SetStatus(Status.Normal);

                // Initially, rotate canvas towards user's view.
                var canvas = transform.Find("Canvas");
                canvas.LookAt(Camera.main.transform);

                // Update string in case something changed.
                var text = locationPinInfo.GetComponentInChildren<Text>();
                text.text = "Room " + roomNumber + "\nAttribute: " + attribute;
            }

            locationPinInfo.SetActive(show);
        }

        public bool IsInfoShown()
        {
            return locationPinInfo.activeSelf;
        }

        public void SetStatus(Status status)
        {
            if (status == Status.Normal) {
                locationPinInfo.GetComponent<Image>().color = normalColor;
                acceptButton.interactable = true;
            }
            else {
                locationPinInfo.GetComponent<Image>().color = errorColor;
                acceptButton.interactable = false;
            }
        }
        
        private void Update()
        {
            // When the user moves behind canvas, contents would be shown mirrored.
            // So we rotate canvas by 180 degrees.
            var canvas = transform.Find("Canvas");
            if (Vector3.Dot(Camera.main.transform.forward, canvas.forward) < 0f) {
                canvas.GetComponent<RectTransform>().Rotate(0f, 180f, 0f);
            }
        }
    }

}
