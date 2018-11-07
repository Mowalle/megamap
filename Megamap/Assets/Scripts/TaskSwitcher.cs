using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class TaskSwitcher : MonoBehaviour {

        [SerializeField] private bool randomizeTasks = true;
        [SerializeField] private bool preventDirectRepitition = true;

        public Task[] tasks = new Task[5];
        private int[] taskOrder = null;
        
        private int currentTaskIdx = 0;
        private int lastTask = -1;

        public void NextTask()
        {
            if (currentTaskIdx == tasks.Length - 1) {
                var conditionSwitcher = FindObjectOfType<ConditionSwitcher>();
                if (conditionSwitcher != null) {
                    conditionSwitcher.NextCondition();
                }
                return;
            }

            lastTask = taskOrder[currentTaskIdx];
            ++currentTaskIdx;
            UpdateTasks();
        }

        public void PreviousTask()
        {
            if (currentTaskIdx == 0) {
                return;
            }

            lastTask = taskOrder[currentTaskIdx];
            --currentTaskIdx;
            UpdateTasks();
        }

        public void ResetTasks()
        {
            foreach (Task t in tasks) {
                t.ResetSubtasks();
            }

            lastTask = taskOrder[currentTaskIdx];
            currentTaskIdx = 0;

            if (randomizeTasks)
                ShuffleTasks();

            UpdateTasks();
        }

        private void Awake()
        {
            taskOrder = new int[tasks.Length];
            for (int i = 0; i < taskOrder.Length; ++i) {
                taskOrder[i] = i;
            }
        }

        private void Start()
        {
            ResetTasks();
        }

        private void UpdateTasks()
        {
            foreach (Task t in tasks) {
                if (t.gameObject.activeSelf) {
                    t.gameObject.SetActive(false);
                }
            }
            tasks[taskOrder[currentTaskIdx]].gameObject.SetActive(true);
        }

        private void ShuffleTasks()
        {
            if (taskOrder.Length <= 1)
                return;

            do {
                Debug.Log("Shuffling tasks...");
                System.Random rnd = new System.Random();
                taskOrder = new List<int>(taskOrder).OrderBy(x => rnd.Next()).ToArray();
            } while (preventDirectRepitition && lastTask == taskOrder[currentTaskIdx]);

            Debug.Log("New task order: " + string.Join(", ", new List<int>(taskOrder).ConvertAll(i => i.ToString()).ToArray()));
        }
    }

}


