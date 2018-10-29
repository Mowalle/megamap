﻿using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Megamap {

    public class Megamap : MonoBehaviour {

        [Header("Megamap Settings"), Space]
        private GameObject map;
        private Transform previousParent;
        public GameObject Map { get { return map; } set { SetMap(value); } }

        [Range(0.01f, 1f)]
        public float scale = 1f;

        [Range(1, 100)]
        public int wallHeight = 10;

        [Range(0.1f, 1.5f)]
        public float heightOffset = 0f;

        public bool placeAtPlayerOnEnable = true;

        public Transform labReferenceTransform;

        [Header("User Marker Settings"), Space]
        [SerializeField]
        private UserMarker userMarker;

        public LocationPin[] LocationPins
        {
            get {
                var pins = map.GetComponentsInChildren<LocationPin>();
                if (pins == null || pins.Length == 0) {
                    Debug.LogWarning("Megamap: Map does not contain any LocationPins.");
                }
                return pins;
            }
        }

        public void SetMap(GameObject map)
        {
            // 'Reset' previous map object.
            if (this.map != null) {
                this.map.SetActive(false);
                this.map.transform.SetParent(previousParent);
            }

            this.map = map;
            previousParent = this.map.transform.parent;
            this.map.transform.SetParent(transform);
            this.map.transform.localPosition = Vector3.zero;
            this.map.SetActive(true);
            
            var mapReferencePoint = this.map.transform.Find("ReferencePoint");
            if (mapReferencePoint == null) {
                Debug.LogWarning("Megamap: Map does not contain a child named \"ReferencePoint\". Disabling UserMarker.");
                userMarker.MapReferenceTransform = null;
                userMarker.gameObject.SetActive(false);
            }
            else {
                userMarker.MapReferenceTransform = mapReferencePoint;
                userMarker.gameObject.SetActive(true);
            }
            
            if (LocationPins != null) {
                foreach (LocationPin pin in LocationPins) {
                    pin.ShowInfo(false);
                }
            }
        }

        public void PlaceAtPlayer()
        {
            if (map == null) {
                return;
            }

            var player = Camera.main;
            var offset = player.transform.position - labReferenceTransform.position;
            offset *= scale;

            map.transform.position = player.transform.position - offset;
        }

        private void Awake()
        {
            if (map != null) {
                SetMap(map);
            }
        }

        private void OnEnable()
        {
            UpdateMapTransform();
        }

        private void UpdateMapTransform()
        {
            // Apply room and wall scale.
            map.transform.localScale = new Vector3(scale, wallHeight / 100f, scale);

            if (placeAtPlayerOnEnable) {
                PlaceAtPlayer();
            }

            // Apply height offset.
            map.transform.position = new Vector3(map.transform.position.x, heightOffset, map.transform.position.z);

            // "Fix" scaling of LocationPins.
            foreach (var pin in LocationPins) {
                pin.transform.localScale = new Vector3(1f, (100f / wallHeight) * scale, 1f);
            }
        }
    }

}
