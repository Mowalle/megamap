using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Megamap {

    public class ConditionSwitcher : MonoBehaviour {

        // For .json loading

        [Serializable]
        public struct Condition {
            public float scale;
            public int wallHeight;
            public float heightOffset;
        }

        [Serializable]
        private struct ConditionConfiguration {
            public Condition[] conditions;
        }

        //------------------

        public int CurrentConditionIdx
        {
            get { return mySequence[(startOffset + numConditionsFinished) % conditions.Length]; }
        }

        public Condition CurrentCondition
        {
            get {
                return conditions[CurrentConditionIdx];
            }
        }

        [Header("Condition Settings"), Space]

        [SerializeField] private TextAsset conditionSequenceFile = null;

        [SerializeField] private bool randomizeConditions = true;

        [SerializeField] private TextAsset conditionsJson = null;
        [SerializeField] private Condition[] conditions = null;

        private int[] mySequence = null;
        private int startOffset = 0;
        private int numConditionsFinished = 0;

        public void NextCondition()
        {
            // Last condition was reached -> experiment is over!
            if (numConditionsFinished == conditions.Length - 1) {
                var task = FindObjectOfType<Task>();
                task.Description = "Geschafft!\nDas Experiment ist vorbei.";
                return;
            }

            ++numConditionsFinished;

            var switcher = FindObjectOfType<TaskSwitcher>();
            switcher.ResetTasks();
        }

        public void PreviousCondition()
        {
            if (numConditionsFinished == 0) {
                return;
            }

            --numConditionsFinished;

            var switcher = FindObjectOfType<TaskSwitcher>();
            switcher.ResetTasks();
        }
        
        private void Awake()
        {
            var json = conditionsJson.text;
            var config = JsonUtility.FromJson<ConditionConfiguration>(json);
            conditions = config.conditions;

            if (conditionSequenceFile == null)
                randomizeConditions = false;

            if (randomizeConditions) {
                var conditionSequences = SequenceLoader.LoadSequences(conditionSequenceFile);
                mySequence = conditionSequences[UnityEngine.Random.Range(0, conditionSequences.GetLength(0))];
                Assert.AreEqual(conditions.Length, mySequence.Length);
                startOffset = UnityEngine.Random.Range(0, mySequence.Length);
            }
            else {
                mySequence = new int[conditions.Length];
                for (int i = 0; i < mySequence.Length; ++i) {
                    mySequence[i] = i;
                }
            }

            FindObjectOfType<RecordData>().Log("Condition sequence is "
                + string.Join(", ", new List<int>(mySequence).ConvertAll(i => i.ToString()).ToArray())
                + ", starting with condition "
                + (mySequence[startOffset] + 1) + "/" + mySequence.Length
                + ".");
        }
    }

}