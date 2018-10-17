using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class LocationPin : MonoBehaviour {

        [SerializeField]
        private GameObject locationPinText;

        // TODO: Rename this to something more semantic.
        public int attribute = 0;

        private int roomNumber;

        public void ShowLocationPinText(bool show)
        {
            locationPinText.SetActive(show);
        }

        public void CheckIsCorrectPin()
        {
            var pins = FindObjectsOfType<LocationPin>();
            if (pins.Length == 0) {
                Debug.LogError("LocationPin: No LocationPins found in scene.");
                return;
            }

            var minimum = pins.Min(p => p.attribute);

            // Selected pin is not correct.
            if (attribute != minimum) {
                return;
            }

            var taskSwitcher = FindObjectOfType<TaskSwitcher>();
            if (taskSwitcher == null) {
                Debug.LogError("LocationPin: TaskSwitcher not found in scene.");
                return;
            }

            taskSwitcher.SwitchTask(TaskSwitcher.Type.Pointing);
        }

        private void Start()
        {
            if (locationPinText == null) {
                Debug.LogError("LocationPin: locationPinText reference not set; disabling script.");
                enabled = false;
                return;
            }

            roomNumber = Random.Range(100, 1000);
            var text = locationPinText.GetComponentInChildren<Text>();
            text.text = "Room " + roomNumber + "\nAttribute: " + attribute;

            ShowLocationPinText(false);
        }
    }

}
