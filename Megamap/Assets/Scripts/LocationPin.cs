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

    [RequireComponent(typeof(Interactable), typeof(SphereCollider))]
    public class LocationPin : MonoBehaviour {
        
        public enum Status { Normal, Error }

        public UnityEvent OnTargetPinSelected = new UnityEvent();
        public UnityEvent OnWrongPinSelected = new UnityEvent();

        public bool isTargetPin = false;

        // TODO: Rename this to something more semantic.
        public int attribute = 0;
        public int roomNumber;

        [SerializeField] private GameObject locationPinInfo = null;
        [SerializeField] private Button acceptButton = null;
        [SerializeField] private Color normalColor = new Color();
        [SerializeField] private Color errorColor = new Color();

        private bool isShown = false;

        public void Show()
        {
            // Initially, rotate canvas towards user's view.
            var canvas = transform.Find("Canvas");
            canvas.LookAt(Camera.main.transform);

            KeepFacingUser();

            // Update string in case something changed.
            var text = locationPinInfo.GetComponentInChildren<Text>();
            text.text = "Room " + roomNumber + "\nAttribute: " + attribute;

            locationPinInfo.SetActive(true);
            isShown = true;
        }

        public void Hide()
        {
            locationPinInfo.SetActive(false);

            isShown = false;
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

        public void Select()
        {
            if (isTargetPin) {
                Hide();
                OnTargetPinSelected.Invoke();
            }
            else {
                SetStatus(Status.Error);
                OnWrongPinSelected.Invoke();
            }
        }

        private void OnDrawGizmos()
        {
            var coll = GetComponent<SphereCollider>();
            Gizmos.DrawWireSphere(transform.position + coll.center, coll.radius);
        }

        private void OnEnable()
        {
            Hide();
            SetStatus(Status.Normal);
        }

        private void Update()
        {
            if (!isShown)
                return;

            KeepFacingUser();
        }

        //-------------------------------------------------
        // Called when a Hand starts hovering over this object
        //-------------------------------------------------
        private void OnHandHoverBegin(Hand hand)
        {
            foreach (LocationPin pin in FindObjectsOfType<LocationPin>())
                pin.Hide();
            Show();
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
            //GrabTypes startingGrabType = hand.GetGrabStarting();
            ////bool isGrabEnding = hand.IsGrabEnding(this.gameObject);

            //// The GetMouseButtonDown(0) is a workaround for left-click not working currently with SteamVRs fallback hand (in 2D-mode).
            //if (startingGrabType != GrabTypes.None || Input.GetMouseButtonDown(0)) {
            //    if (isShown) {
            //        Hide();
            //    }
            //    else {
            //        foreach (LocationPin pin in FindObjectsOfType<LocationPin>())
            //            pin.Hide();
            //        Show();
            //    }
            //}
            //else if (isGrabEnding) {
            //}
        }

        private void KeepFacingUser()
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
