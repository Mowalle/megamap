using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class TaskSwitcher : MonoBehaviour {

        public enum Type {
            UserPositionSetup, UserGazeSetup, Searching, Pointing
        }

        [SerializeField]
        private ConditionSwitcher conditionSwitcher;

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

        [Header("Maps"), Space]
        public GameObject[] indoorMaps;
        private int currentMap = 0;

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

            if (indoorMaps == null || indoorMaps.Length == 0) {
                DisableOnError("No maps set");
                return;
            }

            foreach (var map in indoorMaps) {
                if (map == null) {
                    DisableOnError("One of the maps is null");
                    return;
                }
            }

            // Setup first condition.
            conditionSwitcher.CurrentCondition = 0;
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
                userPositionSetupTask.SetActive(false);
                // Sets next map (wraps around to first one).
                megamapTask.GetComponent<SubtaskMegamap>().SetMap(indoorMaps[currentMap % indoorMaps.Length]);
                ++currentMap;
                megamapTask.SetActive(true);
                pointingTask.SetActive(false);
                break;
            case Type.Pointing:
                userPositionSetupTask.SetActive(false);
                megamapTask.SetActive(false);
                pointingTask.SetActive(true);
                break;
            default: break;
            }

            // If we completed all maps once, switch to next condition.
            if (CurrentType == Type.UserPositionSetup
                && (currentMap >= indoorMaps.Length)
                && (currentMap % indoorMaps.Length == 0)) {
                ++conditionSwitcher.CurrentCondition;
            }
        }
                
        private void DisableOnError(string message)
        {
            Debug.LogError("TaskSwitcher: " + message + " Disabling script.");
            enabled = false;
        }
    }

}


