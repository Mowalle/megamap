using UnityEngine;

namespace Megamap {

    public class Subtask : MonoBehaviour { }

    public class Task : MonoBehaviour {

        public Subtask[] subtasks = new Subtask[4];

        // Will be set by subtasks.
        public string Description { get; set; }

        private int currentSubtask = 0;

        public void NextSubtask()
        {
            if (currentSubtask == 3) {
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
            for (int i = 0; i < subtasks.Length; ++i)
                if (i != currentSubtask)
                    subtasks[i].gameObject.SetActive(false);

            subtasks[currentSubtask].gameObject.SetActive(true);
        }


        private void Start()
        {
            //Description = "No subtask activated yet.";
            UpdateSubtask();
        }
    }
}
