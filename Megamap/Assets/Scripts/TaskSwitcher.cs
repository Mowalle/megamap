using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class TaskSwitcher : MonoBehaviour {

        [SerializeField] private bool randomizeTasks = true;

        public Task[] tasks = new Task[5];
        
        private int currentTask = 0;

        public void NextTask()
        {
            if (currentTask == tasks.Length - 1) {
                var conditionSwitcher = FindObjectOfType<ConditionSwitcher>();
                if (conditionSwitcher != null) {
                    conditionSwitcher.NextCondition();
                }
                return;
            }

            ++currentTask;
            UpdateTasks();
        }

        public void PreviousTask()
        {
            if (currentTask == 0) {
                return;
            }

            --currentTask;
            UpdateTasks();
        }

        public void ResetTasks()
        {
            foreach (Task t in tasks) {
                t.ResetSubtasks();
            }

            if (randomizeTasks)
                ShuffleTasks();

            currentTask = 0;
            UpdateTasks();
        }

        private void UpdateTasks()
        {
            foreach (Task t in tasks) {
                if (t.gameObject.activeSelf) {
                    t.gameObject.SetActive(false);
                }
            }
            tasks[currentTask].gameObject.SetActive(true);
        }
        
        private void ShuffleTasks()
        {
            System.Random rnd = new System.Random();
            tasks = new List<Task>(tasks).OrderBy(x => rnd.Next()).ToArray();
        }


        private void Start()
        {
            ResetTasks();
        }
    }

}


