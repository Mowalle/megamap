using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class SubtaskUserSetup : MonoBehaviour {

        [SerializeField]
        private FloorTarget floorTarget;
        [SerializeField]
        private WallTarget wallTarget;

        private TaskSwitcher switcher;

        private void Awake()
        {
            switcher = GameObject.FindObjectOfType<TaskSwitcher>();
        }

        private void OnEnable()
        {
            Debug.Log("Starting the subtask \"User Setup\"");
        }

        private void Update()
        {
            if (floorTarget.OnTarget) {
                switcher.SwitchTask(TaskSwitcher.Type.UserGazeSetup);
            }
            else {
                // If user steps off floor target before completing gaze setup, return to position setup.
                switcher.SwitchTask(TaskSwitcher.Type.UserPositionSetup);
                return;
            }
            
            // User stands on floor target. If looking at wall target, continue to searching.
            if (wallTarget.OnTarget) {
                switcher.SwitchTask(TaskSwitcher.Type.Searching);
            }
        }
    }

}
