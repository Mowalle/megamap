using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Megamap {

    public class TaskSwitcher : MonoBehaviour {

        [SerializeField] private bool randomizeTasks = true;
        [SerializeField] private bool preventDirectRepitition = true;

        [SerializeField] private Task[] tasks = new Task[5];

        [SerializeField] private TextAsset taskSequenceFile = null;
        private int[][] sequences = null;
        private int[] currentSequence = null;
        private int startOffset = -1;
        private int numTasksFinished = 0;

        public void NextTask()
        {
            SaveData();

            if (numTasksFinished == tasks.Length - 1) {
                var conditionSwitcher = FindObjectOfType<ConditionSwitcher>();
                if (conditionSwitcher != null) {
                    conditionSwitcher.NextCondition();
                }
                return;
            }

            ++numTasksFinished;
            UpdateTasks();
        }

        public void PreviousTask()
        {
            if (numTasksFinished == 0) {
                return;
            }

            --numTasksFinished;
            UpdateTasks();
        }

        public void ResetTasks()
        {
            // Remember previous task in case we don't want to repeat it. 
            int lastTask = (startOffset == -1) ? -1 : currentSequence[(startOffset + numTasksFinished) % tasks.Length];

            foreach (Task t in tasks) {
                t.ResetSubtasks();
            }

            numTasksFinished = 0;

            // Load new sequences in case old ones are all used up, which should not be a problem when using a file with all possible permutations.
            if (sequences.GetLength(0) == 0)
                LoadSequences();

            // Pick sequence and remove it from list of available sequences.
            {
                var sequenceIndex = Random.Range(0, sequences.GetLength(0));
                currentSequence = sequences[sequenceIndex];
                Assert.AreEqual(currentSequence.Length, tasks.Length);

                // This remove the used element, but it's quite ugly...
                var tmp = new List<int[]>(sequences);
                tmp.RemoveAt(sequenceIndex);
                sequences = tmp.ToArray();
            }

            if (randomizeTasks) {
                do {
                    startOffset = Random.Range(0, currentSequence.Length);
                } while (preventDirectRepitition && lastTask == currentSequence[startOffset]);
            }
            else {
                startOffset = 0;
            }

            RecordData.Log("Task sequence is " + string.Join(", ", new List<int>(currentSequence).ConvertAll(i => i.ToString()).ToArray()));

            UpdateTasks();
        }

        private void Awake()
        {
            LoadSequences();
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
            tasks[currentSequence[(startOffset + numTasksFinished) % tasks.Length]].gameObject.SetActive(true);

            RecordData.Log("Starting task " + (currentSequence[(startOffset + numTasksFinished) % tasks.Length] + 1) + " / " + currentSequence.Length);
        }

        private void LoadSequences()
        {
            if (taskSequenceFile != null) {
                sequences = SequenceLoader.LoadSequences(taskSequenceFile);
            }
            else {
                // In case file is not setup, just do [0, 1, 2, 3, ...].
                sequences = new int[1][];
                sequences[0] = new int[tasks.Length];
                for (int i = 0; i < sequences[0].Length; ++i) {
                    sequences[0][i] = i;
                }
            }
        }

        private void SaveData()
        {
            RecordData.CurrentRecord.conditionIndex = FindObjectOfType<ConditionSwitcher>().CurrentConditionIdx;
            int currentTaskIdx = (startOffset + numTasksFinished) % tasks.Length;
            RecordData.CurrentRecord.taskIndex = currentSequence[currentTaskIdx];

            if (RecordData.writeData)
                RecordData.DumpToDisk(RecordData.UserFolder, "data_c_" + RecordData.CurrentRecord.conditionIndex + "_t_" + RecordData.CurrentRecord.taskIndex);
        }
    }
}


