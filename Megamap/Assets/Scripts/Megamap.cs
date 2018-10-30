using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Megamap {

    public class Megamap : MonoBehaviour {

        [Header("Megamap Settings"), Space]
        [Range(0.01f, 1f)]
        public float scale = 1f;

        [Range(1, 100)]
        public int wallHeight = 10;
        
        [Range(0.1f, 1.5f)]
        public float heightOffset = 0f;

        [Space]

        public bool placeAtPlayerOnEnable = true;

        [Space]

        public Transform labReferenceTransform;
        public Transform mapReferenceTransform;

        [Header("User Marker Settings"), Space]
        [SerializeField]
        private UserMarker userMarker;

        public LocationPin[] LocationPins
        {
            get {
                var pins = GetComponentsInChildren<LocationPin>();
                if (pins == null || pins.Length == 0) {
                    Debug.LogWarning("Megamap: Map does not contain any LocationPins.");
                }
                return pins;
            }
        }

        public void PlaceAtPlayer()
        {
            var player = Camera.main;
            var offset = player.transform.position - labReferenceTransform.position;
            offset *= scale;

            transform.position = player.transform.position - offset;
        }
        
        private void OnEnable()
        {
            if (LocationPins != null) {
                foreach (LocationPin pin in LocationPins) {
                    pin.ShowInfo(false);
                }
            }

            userMarker.MapReferenceTransform = mapReferenceTransform;

            if (placeAtPlayerOnEnable) {
                PlaceAtPlayer();
            }
        }


        private void Update()
        {
            // Apply room and wall scale.
            transform.localScale = new Vector3(scale, wallHeight / 100f, scale);

            // Apply height offset.
            transform.position = new Vector3(transform.position.x, heightOffset, transform.position.z);

            // "Fix" scaling of LocationPins.
            foreach (var pin in LocationPins) {
                pin.transform.localScale = new Vector3(1f, (100f / wallHeight) * scale, 1f);
            }

            // "Fix" scaling of UserMarker.
            userMarker.transform.localScale = new Vector3(1f, (100f / wallHeight) * scale, 1f);
        }
    }

}
