using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class StartPosition : MonoBehaviour {

        TaskSwitcher taskSwitcher;

        private void Start()
        {
            var go = GameObject.Find("TaskSwitcher");
            if (go != null) {
                taskSwitcher = go.GetComponent<TaskSwitcher>();
            }
            else {
                Debug.LogError("StartPosition: TaskSwitcher cannot be found; disabling script.");
                enabled = false;
                return;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Player enters start position -> now do the gaze task.
            if (taskSwitcher.GetCurrentType() == TaskSwitcher.Type.PositionReset) {
                taskSwitcher.SwitchTask(TaskSwitcher.Type.GazeReset);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // If start position is left before gaze task is completed, redo task.
            if (taskSwitcher.GetCurrentType() == TaskSwitcher.Type.GazeReset) {
                taskSwitcher.SwitchTask(TaskSwitcher.Type.PositionReset);
            }
        }
    }

}
