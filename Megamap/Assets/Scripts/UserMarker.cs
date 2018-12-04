using UnityEngine;

namespace Megamap {

    [RequireComponent(typeof(Megamap))]
    public class UserMarker : MonoBehaviour {

        [SerializeField] private GameObject usermarkerOnMap = null;
        [SerializeField] private GameObject usermarkerInEnvironment = null;

        private void OnEnable()
        {
            ChangeVisibility.SetVisible(usermarkerOnMap, true);
            ChangeVisibility.SetVisible(usermarkerInEnvironment, true);
        }

        private void OnDisable()
        {
            ChangeVisibility.SetVisible(usermarkerOnMap, false);
            ChangeVisibility.SetVisible(usermarkerInEnvironment, false);
        }

        private void Update()
        {
            var map = GetComponent<Megamap>();
            if (usermarkerOnMap == null || usermarkerInEnvironment == null
                || map.LabReference == null || map.MapReference == null) {
                enabled = false;
                return;
            }

            var cam = Camera.main;
            var offset = (cam.transform.position - map.LabReference.position);
            usermarkerOnMap.transform.localPosition = new Vector3(offset.x, 0.01f, offset.z);
            usermarkerOnMap.transform.localRotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f);

            // Update the user marker circle that is placed in the VE (lab).
            var newPosition = usermarkerInEnvironment.transform.position;
            newPosition.x = Camera.main.transform.position.x;
            newPosition.z = Camera.main.transform.position.z;
            usermarkerInEnvironment.transform.position = newPosition;
        }
    }

}
