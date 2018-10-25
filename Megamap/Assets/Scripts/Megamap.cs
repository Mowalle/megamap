using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Megamap {

    public class Megamap : MonoBehaviour {

        [Header("Megamap Settings"), Space]
        private GameObject map;
        private Transform previousParent;
        public GameObject Map { get { return map; } set { SetMap(value); } }

        [Range(0f, 1f)]
        public float scale = 1f;

        [Range(0, 100)]
        public int wallHeight = 10;

        [Range(0f, 1.5f)]
        public float heightOffset = 0f;

        [Header("User Marker Settings"), Space]
        public UserMarker userMarker;
        public Transform labReferenceTransform;
        public Transform mapReferencePoint;

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

            if (mapReferencePoint == null) {
                mapReferencePoint = this.map.transform.Find("ReferencePoint");
            }

            if (mapReferencePoint == null) {
                Debug.LogWarning("Megamap: Map does not contain a child named \"ReferencePoint\". Disabling UserMarker.");
                userMarker.gameObject.SetActive(false);
            }
            else if (labReferenceTransform == null) {
                Debug.LogWarning("Megamap: No reference transform to lab environment given. Disabling UserMarker.");
                userMarker.gameObject.SetActive(false);
            }
            else {
                userMarker.gameObject.SetActive(true);
            }
            
            if (LocationPins != null) {
                foreach (LocationPin pin in LocationPins) {
                    pin.ShowInfo(false);
                }
            }
        }

        private void Awake()
        {
            if (map != null) {
                SetMap(map);
            }
        }

        private void Update()
        {
            // Apply room and wall scale.
            map.transform.localScale = new Vector3(scale, wallHeight / 100f, scale);

            // Apply height offset.
            map.transform.position = new Vector3(map.transform.position.x, heightOffset, map.transform.position.z);

            // "Fix" scaling of LocationPins.
            foreach (var pin in LocationPins) {
                pin.transform.localScale = new Vector3(1f, (100f / wallHeight) * scale, 1f);
            }
        }
    }

}
