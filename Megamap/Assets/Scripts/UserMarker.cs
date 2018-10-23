using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class UserMarker : MonoBehaviour {

        [SerializeField]
        private Megamap megamap;
        [SerializeField]
        private Transform labReferenceTransform;
        [SerializeField]
        private Transform mapReferenceTransform;

        private void Start()
        {
            if (megamap == null) {
                Debug.LogError("UserMarker: No reference to Megamap; disabling object.");
                gameObject.SetActive(false);
                return;
            }

            if (labReferenceTransform == null) {
                Debug.LogError("UserMarker: No reference to lab reference point; disabling object.");
                gameObject.SetActive(false);
                return;
            }
        }

        private void Update()
        {
            if (mapReferenceTransform == null) {
                var point = megamap.Map.transform.Find("ReferencePoint");
                if (point != null) {
                    mapReferenceTransform = point.transform;
                }
                else {
                    Debug.LogError("UserMarker: Map reference point cannot be found; disabling object.");
                    gameObject.SetActive(false);
                    return;
                }
            }

            Camera cam = Camera.main;

            var offset = cam.transform.position - labReferenceTransform.position;
            var newPosition = mapReferenceTransform.localPosition + offset * megamap.scale;
            transform.localPosition = new Vector3(newPosition.x, megamap.heightOffset, newPosition.z);

            transform.rotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f);
            transform.localScale = new Vector3(megamap.scale, megamap.scale, megamap.scale);
        }
    }

}
