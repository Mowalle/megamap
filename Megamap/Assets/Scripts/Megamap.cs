﻿using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Megamap {

    public class Megamap : MonoBehaviour {

        [Header("References"), Space]
        [SerializeField] private GameObject mapModel = null;
        [SerializeField] private UserMarker userMarker = null;
        [SerializeField] private Transform labReference = null;
        [SerializeField] private Transform mapReference = null;
        [SerializeField] private GameObject pins = null;

        [Header("Megamap Settings"), Space]
        [Range(0.01f, 1f)]
        public float scale = 1f;

        [Range(0.1f, 1.5f)]
        public float heightOffset = 0f;

        [SerializeField] private float transitionDuration = 1f;
        [SerializeField] private bool placeAtPlayer = true;
        [SerializeField] private bool initializeShown = true;

        // --- Properties/Getter/Setter --- //

        public Transform LabReference
        {
            get { return labReference; }
            set { labReference = value; }
        }

        public Transform MapReference
        {
            get { return mapReference; }
            set { mapReference = value; }
        }

        private bool isShown;
        public bool IsShown
        {
            get { return isShown; }
            set {
                if (value)
                    Show();
                else
                    Hide();
            }
        }

        public LocationPin[] LocationPins
        {
            get {
                var locationPins = pins.GetComponentsInChildren<LocationPin>();
                if (locationPins == null || locationPins.Length == 0) {
                    Debug.LogWarning("Megamap: Map does not contain any LocationPins.");
                }
                return locationPins;
            }
        }

        // -------------------------------- //

        public IEnumerator Show()
        {
            if (isShown)
                yield return null;

            mapModel.SetActive(true);
            pins.SetActive(true);

            // Deactivate colliders on all pins during transition,
            // so that a pin does not show its info by accident.
            foreach (var pin in LocationPins) {
                var colliders = pin.GetComponents<Collider>();
                foreach (var collider in colliders)
                    collider.enabled = false;
            }

            var targetPosition = placeAtPlayer ? GetPlayerOffsetPosition() : transform.position;
            targetPosition.y = heightOffset;
            yield return StartCoroutine(Transition(
                transform,
                labReference.position,
                targetPosition,
                Vector3.one,
                new Vector3(scale, scale, scale),
                transitionDuration));

            // Re-Activate colliders so pin-info can be shown.
            foreach (var pin in LocationPins) {
                var colliders = pin.GetComponents<Collider>();
                foreach (var collider in colliders)
                    collider.enabled = true;
            }
            userMarker.gameObject.SetActive(true);

            isShown = true;
        }

        public IEnumerator Hide()
        {
            if (!isShown)
                yield return null;

            userMarker.gameObject.SetActive(false);

            yield return StartCoroutine(Transition(
                transform,
                transform.position,
                labReference.position,
                new Vector3(scale, scale, scale),
                Vector3.one,
                transitionDuration));

            mapModel.SetActive(false);
            pins.SetActive(false);

            isShown = false;
        }

        public void PlaceAtPlayer()
        {
            transform.position = GetPlayerOffsetPosition();
        }

        private void Awake()
        {
            if (mapModel == null
                || userMarker == null
                || labReference == null
                || mapReference == null
                || pins == null) {
                Debug.LogError("Megamap: References not set up correctly; disabling script");
                enabled = false;
                return;
            }

            isShown = initializeShown;
            mapModel.SetActive(isShown);
            userMarker.gameObject.SetActive(false);
            pins.SetActive(isShown);
        }

        private void Update()
        {
            if (!isShown)
                return;

            // Apply room and wall scale.
            transform.localScale = new Vector3(scale, scale, scale);

            // Apply height offset.
            transform.position = new Vector3(transform.position.x, heightOffset, transform.position.z);
        }

        private IEnumerator Transition(
            Transform transform,
            Vector3 startPosition,
            Vector3 endPosition,
            Vector3 startScale,
            Vector3 endScale,
            float duration)
        {
            // Lerp position, scale and alpha (transparency).
            float rate = 1.0f / duration;
            float t = 0f;
            while (t < 1f) {
                t += Time.deltaTime * rate;
                transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0f, 1f, t));
                transform.localScale = Vector3.Lerp(startScale, endScale, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
        }

        private Vector3 GetPlayerOffsetPosition()
        {
            var player = Camera.main;
            var offset = player.transform.position - labReference.position;
            offset *= scale;

            return player.transform.position - offset;
        }
    }

}
