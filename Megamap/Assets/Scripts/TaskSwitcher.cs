using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class TaskSwitcher : MonoBehaviour {

        public enum Type {
            UserPositionSetup, UserGazeSetup, Searching, Pointing
        }

        private Type currentType = Type.UserPositionSetup;
        public Type CurrentType
        {
            get { return currentType; }
        }

        [SerializeField]
        private GameObject taskDisplay;

        [Header("Subtasks"), Space]

        [SerializeField]
        private GameObject userPositionSetupTask;
        [SerializeField]
        private GameObject megamapTask;
        [SerializeField]
        private GameObject pointingTask;

        public void SetTaskDescription(string description)
        {
            taskDisplay.GetComponent<Text>().text = description;
        }

        private void Start()
        {
            if (taskDisplay == null) {
                DisableOnError("Reference to taskDisplay not set");
                return;
            }

            if (userPositionSetupTask == null) {
                DisableOnError("Reference to user position setup task not set");
                return;
            }

            if (megamapTask == null) {
                DisableOnError("Reference to megamap task no set");
                return;
            }

            if (pointingTask == null) {
                DisableOnError("Reference to pointing task no set");
                return;
            }

            // Enable first task.
            SwitchTask(currentType);
        }

        public void SwitchTask(Type taskType)
        {
            currentType = taskType;

            switch (currentType) {
            case Type.UserPositionSetup:
                userPositionSetupTask.SetActive(true);
                megamapTask.SetActive(false);
                pointingTask.SetActive(false);
                break;
            case Type.UserGazeSetup:
                userPositionSetupTask.SetActive(true);
                megamapTask.SetActive(false);
                pointingTask.SetActive(false);
                break;
            case Type.Searching:
                //task = "Suche nach Raum mit niedrigstem Attribut.";
                userPositionSetupTask.SetActive(false);
                megamapTask.SetActive(true);
                pointingTask.SetActive(false);
                break;
            case Type.Pointing:
                //task = "Zeige dorthin, wo sich der ausgewählte Raum befindet.";
                userPositionSetupTask.SetActive(false);
                megamapTask.SetActive(false);
                pointingTask.SetActive(true);
                break;
            default: break;
            }
        }
                
        private void DisableOnError(string message)
        {
            Debug.LogError("TaskSwitcher: " + message + " Disabling script.");
            enabled = false;
        }
    }

}


