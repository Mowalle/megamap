using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

namespace Megamap {
    public class ConditionSwitcher : MonoBehaviour {

        // For .json loading

        [Serializable]
        public struct Condition {
            public string viewMode;
            public string heightMode;
            [Range(0.1f, 1.5f)] public float heightOffset;
            [Range(0.01f, 1f)] public float scale;
        }

        [Serializable]
        private struct ConditionConfiguration {
            public Condition[] conditions;
        }

        //------------------

        public int CurrentConditionIndex
        {
            get { return sequence[numConditionsFinished % conditions.Length]; }
        }

        public Condition CurrentCondition
        {
            get {
                return FindObjectOfType<TaskSwitcher>().IsTutorialRunning ? tutorialCondition : conditions[CurrentConditionIndex];
            }
        }

        [Header("Condition Settings"), Space]

        public Condition tutorialCondition = new Condition();

        [SerializeField] private TextAsset conditionsJson = null;
        [SerializeField] private Condition[] conditions = null;

        [SerializeField] private int[] sequence = null;
        private int numConditionsFinished = 0;

        public int[] GetSequence() { return sequence; }

        public void NextCondition()
        {
            // Last condition was reached -> experiment is over!
            if (numConditionsFinished == conditions.Length - 1) {
                FindObjectOfType<TaskDisplay>().CurrentDescriptionID = "finished";
                RecordData.Log("All conditions completed. The experiment is over.");
            }
            else {
                ++numConditionsFinished;
                RecordData.Log("Starting condition " + CurrentConditionIndex + " (" + (numConditionsFinished + 1) + " / " + sequence.Length + ")");
            }

        }

        private void Awake()
        {
            var json = conditionsJson.text;
            var config = JsonUtility.FromJson<ConditionConfiguration>(json);
            conditions = config.conditions;

            if (sequence == null || sequence.Length != conditions.Length) {
                Debug.LogWarning(conditionsJson.name + " defines " + conditions.Length + " conditions, but set sequence is invalid. Using default sequence.");
                sequence = new int[conditions.Length];
                for (int i = 0; i < sequence.Length; ++i) {
                    sequence[i] = i;
                }
            }

            RecordData.Log("Condition sequence is "
                + string.Join(", ", new List<int>(sequence).ConvertAll(i => i.ToString()).ToArray())
                + ", starting with condition "
                + CurrentConditionIndex + " (" + (numConditionsFinished + 1) + " / " + sequence.Length + ")"
                + ".");
        }
    }

}