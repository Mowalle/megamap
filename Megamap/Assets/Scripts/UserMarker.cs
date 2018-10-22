using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class UserMarker : MonoBehaviour {
        
        [SerializeField]
        private Transform labReferenceTransform;
        [SerializeField]
        private Transform mapReferenceTransform;
        [SerializeField]
        private Camera cam;

        private void Start()
        {
            if (cam == null) {
                cam = Camera.main;
            }
        }

        private void Update()
        {
            var megamapConfig = FindObjectOfType<MegamapConfiguration>();
            if (megamapConfig == null) {
                Debug.LogError("UserMarker: No Megamap object found.");
                return;
            }

            var offset = cam.transform.position - labReferenceTransform.position;
            var newPosition = mapReferenceTransform.position + offset * megamapConfig.scale;
            transform.localPosition = new Vector3(newPosition.x, megamapConfig.heightOffset, newPosition.z);

            transform.rotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f);
            transform.localScale = new Vector3(megamapConfig.scale, megamapConfig.scale, megamapConfig.scale);
        }
    }

}
