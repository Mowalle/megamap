using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class LocationPin : MonoBehaviour {

        public static void HideAllInfos()
        {
            var pins = FindObjectsOfType<LocationPin>();
            foreach (LocationPin pin in pins) {
                pin.ShowInfo(false);
            }
        }

        [SerializeField]
        private GameObject locationPinInfo;

        [SerializeField]
        private bool isTargetPin = false;
        public bool IsTargetPin { get { return isTargetPin; } set { isTargetPin = value; } }

        [SerializeField]
        private Button acceptButton;
        [SerializeField]
        private Color normalColor;
        [SerializeField]
        private Color errorColor;

        // TODO: Rename this to something more semantic.
        public int attribute = 0;

        private int roomNumber;

        public bool IsInfoShown() { return locationPinInfo.activeSelf; }

        public void ShowInfo(bool show)
        {
            locationPinInfo.SetActive(show);

            if (show) {
                // Reset info display in case pin was wrongly selected before.
                locationPinInfo.GetComponent<Image>().color = normalColor;
                acceptButton.interactable = true;
            }
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
                FindObjectOfType<TaskSwitcher>().SetTaskDescription("Raum hat nicht das niedrigste Attribut. Versuche es weiter.");
                locationPinInfo.GetComponent<Image>().color = errorColor;
                acceptButton.interactable = false;
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
            if (locationPinInfo == null) {
                Debug.LogError("LocationPin: locationPinText reference not set; disabling script.");
                enabled = false;
                return;
            }

            roomNumber = Random.Range(100, 1000);
            var text = locationPinInfo.GetComponentInChildren<Text>();
            text.text = "Room " + roomNumber + "\nAttribute: " + attribute;

            ShowInfo(false);
        }

        private void Update()
        {
            // When the user moves behind canvas, contents would be shown mirrored.
            // So we rotate canvas by 180 degrees.
            var canvas = transform.Find("Canvas");
            if (Vector3.Dot(Camera.main.transform.forward, canvas.forward) < 0f) {
                canvas.GetComponent<RectTransform>().Rotate(0f, 180f, 0f);
            }
        }
    }

}
