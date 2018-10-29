using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class UserMarker : MonoBehaviour {

        [SerializeField]
        Megamap megamap;

        public Transform MapReferenceTransform { get; set; }

        private void Update()
        {
            Camera cam = Camera.main;

            transform.position = new Vector3(MapReferenceTransform.position.x, megamap.heightOffset, MapReferenceTransform.position.z);

            var offset = (cam.transform.position - megamap.labReferenceTransform.position) * megamap.scale;
            transform.localPosition += new Vector3(offset.x, 0f, offset.z);

            transform.localRotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f);
            transform.localScale = new Vector3(megamap.scale, megamap.scale, megamap.scale);
        }
    }

}
