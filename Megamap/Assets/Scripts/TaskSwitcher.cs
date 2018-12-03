
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

namespace Megamap {

    public class TaskSwitcher : MonoBehaviour {

        [SerializeField] private TaskDisplay.Language displayLanguage = TaskDisplay.Language.German;

        [SerializeField] private bool randomizeTasks = true;
        [SerializeField] private bool preventDirectRepetition = true;

        [SerializeField] private List<Task> tutorials = new List<Task>();
        [SerializeField] private Task[] tasks = new Task[5];

        [SerializeField] private TextAsset taskSequenceFile = null;

        public Task CurrentTask { get { return runningTutorial ? tutorials[numTutorialsFinished] : tasks[CurrentTaskIndex]; } }

        private int[][] sequences = null;
        private int[] currentSequence = null;
        private int startOffset = -1;
        private int numTasksFinished = 0;
        private int CurrentTaskIndex { get { return runningTutorial ? numTutorialsFinished : currentSequence[(startOffset + numTasksFinished) % currentSequence.Length]; } }

        bool runningTutorial = false;
        public bool IsTutorialRunning { get { return runningTutorial; } }
        int numTutorialsFinished = 0;

        private bool waitingForKeypress = true;

        public void NextTask()
        {
            if (runningTutorial) {
                tutorials[numTutorialsFinished].StopTask();
                tutorials[numTutorialsFinished].gameObject.SetActive(false);

                ++numTutorialsFinished;
                if (numTutorialsFinished == tutorials.Count) {
                    runningTutorial = false;
                    waitingForKeypress = true;
                    return;
                }
                RecordData.Log("Starting tutorial " + numTutorialsFinished
                    + " (" + (numTutorialsFinished + 1) + "/" + tutorials.Count
                    + ") with tutorial condition instead of condition "
                    + FindObjectOfType<ConditionSwitcher>().CurrentConditionIdx + ".");
                if (numTutorialsFinished % 2 != 0)
                    FindObjectOfType<ConditionSwitcher>().tutorialCondition.viewMode = "flat";
                else
                    FindObjectOfType<ConditionSwitcher>().tutorialCondition.viewMode = "default";

                tutorials[CurrentTaskIndex].gameObject.SetActive(true);
                tutorials[CurrentTaskIndex].StartTask();

                return;
            }

            // ------
            // The following will only be executed when all tutorials are finished.
            // ------

            tasks[CurrentTaskIndex].StopTask();
            tasks[CurrentTaskIndex].gameObject.SetActive(false);

            SaveData();

            if (numTasksFinished == tasks.Length - 1) {
                // Switch conditions.
                var condSwitcher = FindObjectOfType<ConditionSwitcher>();
                int lastCondition = condSwitcher.CurrentConditionIdx;
                condSwitcher.NextCondition();
                // If conditions were not switched, it means all conditions were completed.
                // In that case, don't start the next task (there is none) and just keep the
                // task display and wait for user to take off HMD.
                if (lastCondition == condSwitcher.CurrentConditionIdx)
                    return;

                waitingForKeypress = true;
            }
            else {
                ++numTasksFinished;
                StartTask();
            }
        }

        private void Awake()
        {
            if (FindObjectOfType<ConditionSwitcher>() == null) {
                gameObject.SetActive(false);
                return;
            }
        }

        private void Start()
        {
            foreach (var t in tasks) {
                t.gameObject.SetActive(false);
            }

            tutorials.ForEach(t => t.gameObject.SetActive(false));

            if (tutorials.Count > 0)
                runningTutorial = true;

            FindObjectOfType<LaserPointer>().Show(false);
        }

        private void Update()
        {
            FindObjectOfType<TaskDisplay>().SetLanguage(displayLanguage);

            if (!waitingForKeypress)
                return;

            if (runningTutorial) {
                // After start, tutorials need to be run by pressing Space or Return.
                FindObjectOfType<TaskDisplay>().CurrentDescriptionID = "waitTutorial";
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) {
                    waitingForKeypress = false;
                    StartTutorials();
                }
            }
            // Finished tutorials; waiting for experiment or repeating tutorials.
            else {
                FindObjectOfType<TaskDisplay>().CurrentDescriptionID = "waitExperiment";
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) {
                    waitingForKeypress = false;
                    NextSequence();
                    numTasksFinished = 0;
                    StartTask();
                }
                else if (numTasksFinished == 0 && (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.R))) {
                    runningTutorial = true;
                }
            }
        }

        private void NextSequence()
        {
            // Load new sequences in case old ones are all used up, which should not be a problem when using a file with all possible permutations.
            if (sequences == null || sequences.GetLength(0) == 0) {
                LoadSequences();
            }

            // Remember previous task in case we don't want to repeat it. 
            int lastTask = (startOffset == -1) ? -1 : CurrentTaskIndex;

            // Pick sequence and remove it from list of available sequences.
            var sequenceIndex = Random.Range(0, sequences.GetLength(0));
            currentSequence = sequences[sequenceIndex];
            Assert.AreEqual(currentSequence.Length, tasks.Length);

            // This removes the used element, but it's quite ugly...
            var tmp = new List<int[]>(sequences);
            tmp.RemoveAt(sequenceIndex);
            sequences = tmp.ToArray();

            if (tasks.Length > 1 && randomizeTasks) {
                do {
                    startOffset = Random.Range(0, currentSequence.Length);
                } while (preventDirectRepetition && lastTask == currentSequence[startOffset]);
            }
            else {
                startOffset = 0;
            }

            RecordData.Log("New task sequence is " + string.Join(", ", new List<int>(currentSequence).ConvertAll(i => i.ToString()).ToArray()));
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

        private void StartTask()
        {
            RecordData.Log("Starting task " + CurrentTaskIndex + " (" + (numTasksFinished + 1) + " / " + currentSequence.Length + ")");
            tasks[CurrentTaskIndex].gameObject.SetActive(true);
            tasks[CurrentTaskIndex].StartTask();
        }

        private void StartTutorials()
        {
            numTutorialsFinished = 0;
            runningTutorial = true;
            RecordData.Log("Starting tutorial 0 (1/" + tutorials.Count
                + ") with tutorial condition instead of condition "
                + FindObjectOfType<ConditionSwitcher>().CurrentConditionIdx + ".");
            FindObjectOfType<ConditionSwitcher>().tutorialCondition.viewMode = "default";
            tutorials[0].gameObject.SetActive(true);
            tutorials[0].StartTask();
        }

        private void SaveData()
        {
            RecordData.CurrentRecord.conditionIndex = FindObjectOfType<ConditionSwitcher>().CurrentConditionIdx;
            int currentTaskIdx = (startOffset + numTasksFinished) % tasks.Length;
            RecordData.CurrentRecord.taskIndex = currentSequence[currentTaskIdx];

            if (RecordData.writeData)
                RecordData.DumpToDisk(RecordData.UserFolder, "data_c_" + RecordData.CurrentRecord.conditionIndex + "_t_" + RecordData.CurrentRecord.taskIndex + ".txt");
        }
    }
}


