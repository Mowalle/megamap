using UnityEngine;

namespace Megamap {

    public abstract class Subtask : MonoBehaviour {

        public bool IsStarted { get; set; }

        protected virtual void LogSubtask()
        {
            // FIXME: Not really the best approach for getting a distinct name.
            RecordData.Log("Starting subtask " + gameObject.name + ".");
        }

        public void CommonStart()
        {
            StartSubtask();
            IsStarted = true;
        }

        public void CommonStop()
        {
            IsStarted = false;
            StopSubtask();
        }

        public abstract void StartSubtask();
        public abstract void StopSubtask();
    }

    public class Task : MonoBehaviour {

        public Subtask[] subtasks = new Subtask[4];

        private bool isStarted = false;
        public bool IsStarted { get { return isStarted; } }

        private int currentSubtask = 0;

        public void StartTask()
        {
            if (isStarted)
                return;

            currentSubtask = 0;

            foreach (var s in subtasks) {
                s.gameObject.SetActive(s.Equals(subtasks[currentSubtask]) ? true : false);
            }

            subtasks[currentSubtask].gameObject.SetActive(true);
            isStarted = true;
            subtasks[currentSubtask].CommonStart();
        }

        public void StopTask()
        {
            if (!isStarted)
                return;

            subtasks[currentSubtask].CommonStop();
            isStarted = false;
            subtasks[currentSubtask].gameObject.SetActive(false);
        }

        public void NextSubtask()
        {
            if (!isStarted)
                return;

            if (currentSubtask == subtasks.Length - 1) {
                return;
            }

            subtasks[currentSubtask].CommonStop();
            subtasks[currentSubtask].gameObject.SetActive(false);
            ++currentSubtask;
            subtasks[currentSubtask].gameObject.SetActive(true);
            subtasks[currentSubtask].CommonStart();
        }
    }
}
