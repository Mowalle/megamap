
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

namespace Megamap {

    public class TaskSwitcher : MonoBehaviour {

        [SerializeField] private TaskDisplay.Language displayLanguage = TaskDisplay.Language.German;

        [SerializeField] private bool randomizeTasks = true;
        [SerializeField] private bool preventDirectRepetition = true;

        [SerializeField] private bool skipTutorials = false;
        [SerializeField] private List<Task> tutorials = new List<Task>();
        [SerializeField] private Task[] tasks = new Task[5];

        [SerializeField] private TextAsset taskSequenceFile = null;

        public Task CurrentTask { get { return runningTutorial ? tutorials[numTutorialsFinished] : tasks[CurrentTaskIndex]; } }
        public int CurrentTaskIndex { get { return runningTutorial ? numTutorialsFinished : currentSequence[numTasksFinished % currentSequence.Length]; } }

        private int[][] sequences = null;
        private int[] currentSequence = null;
        private int numTasksFinished = 0;

        bool runningTutorial = false;
        public bool IsTutorialRunning { get { return runningTutorial; } }
        int numTutorialsFinished = 0;

        private bool waitingForKeypress = true;

        public int[] GetSequence() { return currentSequence; }

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
                    + FindObjectOfType<ConditionSwitcher>().CurrentConditionIndex + ".");
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

            RecordData.CurrentRecord.taskEndTime = Time.realtimeSinceStartup;
            RecordData.CurrentRecord.taskDuration = RecordData.CurrentRecord.taskEndTime - RecordData.CurrentRecord.taskStartTime;

            SaveData();

            if (numTasksFinished == tasks.Length - 1) {
                // Switch conditions.
                var condSwitcher = FindObjectOfType<ConditionSwitcher>();
                int lastCondition = condSwitcher.CurrentConditionIndex;
                condSwitcher.NextCondition();
                // If conditions were not switched, it means all conditions were completed.
                // In that case, don't start the next task (there is none) and just keep the
                // task display and wait for user to take off HMD.
                if (lastCondition == condSwitcher.CurrentConditionIndex)
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

            if (!skipTutorials && tutorials.Count > 0)
                runningTutorial = true;

            FindObjectOfType<LaserPointer>().Show(false);
        }

        private void Update()
        {
            FindObjectOfType<TaskDisplay>().SetLanguage(displayLanguage);

            if (!waitingForKeypress) {
                if (Input.GetKeyDown(KeyCode.X)) {
                    RecordData.CurrentRecord.skipped = !RecordData.CurrentRecord.skipped;
                    RecordData.CurrentRecord.skippedAfterSeconds = RecordData.CurrentRecord.skipped ? Time.realtimeSinceStartup - RecordData.CurrentRecord.taskStartTime : 0f;
                    RecordData.Log("Skip status of current task was marked as " + RecordData.CurrentRecord.skipped);
                }
                return;
            }

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
                else if (!skipTutorials && tutorials.Count > 0
                    && numTasksFinished == 0 // So that when pausing between experiment conditions, tutorials cannot be started.
                    && (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.R))) {
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
            int lastTask = (currentSequence == null) ? -1 : CurrentTaskIndex;

            int sequenceIndex;
            do {
                // Pick sequence and remove it from list of available sequences.
                sequenceIndex = Random.Range(0, sequences.GetLength(0));
                currentSequence = sequences[sequenceIndex];
            } while (randomizeTasks && preventDirectRepetition && lastTask == currentSequence[0]);
            Assert.AreEqual(currentSequence.Length, tasks.Length);

            // This removes the used element, but it's quite ugly...
            var tmp = new List<int[]>(sequences);
            tmp.RemoveAt(sequenceIndex);
            sequences = tmp.ToArray();

            RecordData.Log("New task sequence is " + string.Join(", ", new List<int>(currentSequence).ConvertAll(i => i.ToString()).ToArray()));
        }

        private void LoadSequences()
        {
            if (randomizeTasks && taskSequenceFile != null) {
                sequences = SequenceLoader.LoadSequences(taskSequenceFile);
            }
            // In case there is no sequence file or randomization is turned of,
            // just do the tasks in assigned sequence (0, 1, 2, ...).
            else {
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
            RecordData.CurrentRecord.taskStartTime = Time.realtimeSinceStartup;
            tasks[CurrentTaskIndex].gameObject.SetActive(true);
            tasks[CurrentTaskIndex].StartTask();
        }

        private void StartTutorials()
        {
            numTutorialsFinished = 0;
            runningTutorial = true;
            RecordData.Log("Starting tutorial 0 (1/" + tutorials.Count
                + ") with tutorial condition instead of condition "
                + FindObjectOfType<ConditionSwitcher>().CurrentConditionIndex + ".");
            FindObjectOfType<ConditionSwitcher>().tutorialCondition.viewMode = "default";
            tutorials[0].gameObject.SetActive(true);
            tutorials[0].StartTask();
        }

        private void SaveData()
        {
            RecordData.CurrentRecord.conditionIndex = FindObjectOfType<ConditionSwitcher>().CurrentConditionIndex;
            int currentTaskIdx = numTasksFinished % tasks.Length;
            RecordData.CurrentRecord.taskIndex = currentSequence[currentTaskIdx];

            if (RecordData.writeData)
                RecordData.DumpToDisk(RecordData.UserFolder, "data_c_" + RecordData.CurrentRecord.conditionIndex + "_t_" + RecordData.CurrentRecord.taskIndex + ".txt");
        }
    }
}


