using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class Task : MonoBehaviour {

        public SubtaskUserSetup subtaskUserSetup;
        public SubtaskMegamap subtaskMegamap;
        public SubtaskPointing subtaskPointing;

        // Will be set by subtasks.
        public string Description { get; set; }

        private int currentSubtask = 0;

        public void NextSubtask()
        {
            if (currentSubtask == 2) {
                return;
            }

            ++currentSubtask;
            UpdateSubtask();
        }

        public void PreviousSubtask()
        {
            if (currentSubtask == 0) {
                return;
            }

            --currentSubtask;
            UpdateSubtask();
        }

        public void ResetSubtasks()
        {
            currentSubtask = 0;
            UpdateSubtask();
        }

        private void UpdateSubtask()
        {
            switch (currentSubtask) {
            case 0:
                subtaskUserSetup.gameObject.SetActive(true);
                subtaskMegamap.gameObject.SetActive(false);
                subtaskPointing.gameObject.SetActive(false);
                break;
            case 1:
                subtaskUserSetup.gameObject.SetActive(false);
                subtaskMegamap.gameObject.SetActive(true);
                subtaskPointing.gameObject.SetActive(false);
                break;
            case 2:
                subtaskUserSetup.gameObject.SetActive(false);
                subtaskMegamap.gameObject.SetActive(false);
                subtaskPointing.gameObject.SetActive(true);
                break;
            default: break;
            }
        }


        private void Start()
        {
            //Description = "No subtask activated yet.";
            UpdateSubtask();
        }
    }
}
