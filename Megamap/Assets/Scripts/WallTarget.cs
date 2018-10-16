using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class WallTarget : MonoBehaviour {
        
        private TaskSwitcher taskSwitcher;
        
        void Start()
        {
            var go = GameObject.Find("TaskSwitcher");
            if (go != null) {
                taskSwitcher = go.GetComponent<TaskSwitcher>();
            }
            else {
                Debug.LogError("WallTarget: TaskSwitcher cannot be found; disabling script.");
                enabled = false;
                return;
            }
        }

        void Update()
        {
            // Only execute when player should look at wall target.
            if (taskSwitcher.GetCurrentType() != TaskSwitcher.Type.GazeReset) {
                return;
            }

            var cam = Camera.main;
            RaycastHit hit;
            // Is the player looking at this wall target? -> Start with search task.
            if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity)) {
                if (hit.collider.gameObject.Equals(this.gameObject)) {
                    taskSwitcher.SwitchTask(TaskSwitcher.Type.Searching);
                }
            }
        }
    }

}
