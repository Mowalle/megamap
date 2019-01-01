using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class InstructorUI : MonoBehaviour {

        public Text userID = null;
        public Text runtime = null;
        public Text conditions = null;
        public Text tasks = null;
        public Text skipped = null;
        
        void Start()
        {
            userID.text = "Current User: " + RecordData.userID;
        }

        void Update()
        {
            var time = Time.time;
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time - minutes * 60);

            runtime.text = "Runtime: " + string.Format("{0:00}:{1:00}", minutes, seconds);


            var taskSwitcher = FindObjectOfType<TaskSwitcher>();

            conditions.text = "Conditions: ";
            if (taskSwitcher.IsTutorialRunning) {
                conditions.text += "[Tutorial condition] ";
            }
            var condSwitcher = FindObjectOfType<ConditionSwitcher>();
            var condSequence = condSwitcher.GetSequence();
            for (int i = 0; i < condSequence.Length; ++i) {
                if (condSequence[i] == condSwitcher.CurrentConditionIndex)
                    conditions.text += "[" + condSequence[i] + "] ";
                else
                    conditions.text += condSequence[i] + " ";
            }

            tasks.text = "Tasks: ";
            if (taskSwitcher.IsTutorialRunning) {
                tasks.text += "Running tutorials";
                return;
            }

            var taskSequence = taskSwitcher.GetSequence();
            if (taskSequence == null) {
                tasks.text += "null";
                return;
            }

            for (int i = 0; i < taskSequence.Length; ++i) {
                if (taskSequence[i] == taskSwitcher.CurrentTaskIndex)
                    tasks.text += "[" + taskSequence[i] + "] ";
                else
                    tasks.text += taskSequence[i] + " ";
            }

            if (RecordData.CurrentRecord.skipped) {
                skipped.text = "Current task skipped (after " + RecordData.CurrentRecord.skippedAfterSeconds + "s).";
            }
            else {
                skipped.text = "";
            }
        }
    }
}
