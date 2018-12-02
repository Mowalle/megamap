using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

namespace Megamap {
    public class ConditionSwitcher : MonoBehaviour {

        // For .json loading

        [Serializable]
        public struct Condition {
            public string mode;
            [Range(0.01f, 1f)] public float scale;
            [Range(0.1f, 1.5f)] public float heightOffset;
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
                return FindObjectOfType<TaskSwitcher>().IsTutorialRunning ? tutorialCondition : conditions[CurrentConditionIdx];
            }
        }

        [Header("Condition Settings"), Space]

        public Condition tutorialCondition = new Condition();

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
                FindObjectOfType<TaskDisplay>().CurrentDescriptionID = "finished";
                RecordData.Log("All conditions completed. The experiment is over.");
            }
            else {
                ++numConditionsFinished;
                RecordData.Log("Starting condition " + CurrentConditionIdx + " (" + (numConditionsFinished + 1) + " / " + mySequence.Length + ")");
            }

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

            RecordData.Log("Condition sequence is "
                + string.Join(", ", new List<int>(mySequence).ConvertAll(i => i.ToString()).ToArray())
                + ", starting with condition "
                + CurrentConditionIdx + " (" + (numConditionsFinished + 1) + " / " + mySequence.Length + ")"
                + ".");
        }
    }

}