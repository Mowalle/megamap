using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class UserMarker : MonoBehaviour {

        [SerializeField] private Megamap megamap;

        private void Update()
        {
            Camera cam = Camera.main;

            transform.position = new Vector3(megamap.MapReference.position.x, transform.position.y, megamap.MapReference.position.z);

            var offset = (cam.transform.position - megamap.LabReference.position);
            transform.localPosition += new Vector3(offset.x, 0f, offset.z);

            transform.localRotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f);
        }
    }

}
