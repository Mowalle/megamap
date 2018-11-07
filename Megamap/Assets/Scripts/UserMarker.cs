using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class UserMarker : MonoBehaviour {

        [SerializeField] private Megamap megamap;
        [SerializeField] private GameObject usermarkerInEnvironment;

        private void OnEnable()
        {
            usermarkerInEnvironment.GetComponent<Renderer>().enabled = true;
        }

        private void OnDisable()
        {
            if (usermarkerInEnvironment != null)
                usermarkerInEnvironment.GetComponent<Renderer>().enabled = false;
        }

        private void Update()
        {
            Camera cam = Camera.main;

            transform.position = new Vector3(megamap.MapReference.position.x, transform.position.y, megamap.MapReference.position.z);

            var offset = (cam.transform.position - megamap.LabReference.position);
            transform.localPosition += new Vector3(offset.x, 0f, offset.z);

            transform.localRotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f);

            // Update the user marker circle that is placed in the VE (lab).
            var newPosition = usermarkerInEnvironment.transform.position;
            newPosition.x = Camera.main.transform.position.x;
            newPosition.z = Camera.main.transform.position.z;
            usermarkerInEnvironment.transform.position = newPosition;
        }
    }

}
