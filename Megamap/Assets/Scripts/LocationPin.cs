using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;
using System;

namespace Megamap {
    
    [RequireComponent(typeof(Interactable))]
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

        private LaserPointer laser = null;
        
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

        private void Awake()
        {
            laser = FindObjectOfType<LaserPointer>();
        }

        private void OnEnable()
        {
            if (isTargetPin) {
                var map = transform.parent.GetComponentInParent<Megamap>();

                RecordData.CurrentRecord.correctPinIdx = Array.IndexOf(map.LocationPins, this);
            }

            laser.OnLaserTargetChanged.AddListener(HandleOnLaserTargetChanged);

            Hide();
            SetStatus(Status.Normal);
        }

        private void OnDisable()
        {
            if (laser == null)
                return;

            laser.OnLaserTargetChanged.RemoveListener(HandleOnLaserTargetChanged);
        }

        private void Update()
        {
            if (!isShown)
                return;

            KeepFacingUser();
        }

        private void HandleOnLaserTargetChanged(Collider from, Collider to)
        {
            if (to == GetComponent<Collider>() && !isShown) {
                ++RecordData.CurrentRecord.numSelectionsTotal;
                var map = transform.parent.GetComponentInParent<Megamap>();
                ++RecordData.CurrentRecord.numSelections[Array.IndexOf(map.LocationPins, this)];

                Show();
            }
            else if (from == GetComponent<Collider>() && !IsChildCollider(to)) {
                Hide();
            }
            else if (IsChildCollider(from) && !IsChildCollider(to) && to != GetComponent<Collider>()) {
                Hide();
            }
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

        private bool IsChildCollider(Collider c)
        {
            return Array.Exists(GetComponentsInChildren<Collider>(), collider => collider == c);
        }
    }

}
