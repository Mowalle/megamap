using System;
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

        public Condition CurrentCondition
        {
            get {
                return randomizeConditions ? conditions[mySequence[currentConditionIdx]] : conditions[currentConditionIdx];
            }
        }

        [Header("Condition Settings"), Space]

        [SerializeField] private TextAsset conditionSequenceFile = null;

        [SerializeField] private bool randomizeConditions = true;

        [SerializeField] private TextAsset conditionsJson = null;
        [SerializeField] private Condition[] conditions = null;

        private int[] mySequence = null;
        private int currentConditionIdx = 0;

        public void NextCondition()
        {
            // Last condition was reached -> experiment is over!
            if (currentConditionIdx == conditions.Length - 1) {
                var task = FindObjectOfType<Task>();
                task.Description = "Geschafft!\nDas Experiment ist vorbei.";
                return;
            }

            ++currentConditionIdx;

            var switcher = FindObjectOfType<TaskSwitcher>();
            switcher.ResetTasks();
        }

        public void PreviousCondition()
        {
            if (currentConditionIdx == 0) {
                return;
            }

            --currentConditionIdx;

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
            }
        }
    }

}