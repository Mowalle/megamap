﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public Condition CurrentCondition { get { return conditions[currentCondition]; } }

        private int currentCondition = 0;

        [Header("Condition Settings"), Space]
        [SerializeField] private bool randomizeConditions = true;

        [SerializeField]
        private TextAsset conditionsJson = null;
        [SerializeField]
        private Condition[] conditions = new Condition[0];

        
        public void NextCondition()
        {
            // Last condition was reached -> experiment is over!
            if (currentCondition == conditions.Length - 1) {
                var task = FindObjectOfType<Task>();
                task.Description = "Geschafft!\nDas Experiment ist vorbei.";
                return;
            }

            ++currentCondition;

            var switcher = FindObjectOfType<TaskSwitcher>();
            switcher.ResetTasks();
        }

        public void PreviousCondition()
        {
            if (currentCondition == 0) {
                return;
            }

            --currentCondition;

            var switcher = FindObjectOfType<TaskSwitcher>();
            switcher.ResetTasks();
        }
        
        private void ShuffleConditions()
        {
            System.Random rnd = new System.Random();
            conditions = new List<Condition>(conditions).OrderBy(x => rnd.Next()).ToArray();
        }


        private void Awake()
        {
            var json = conditionsJson.text;
            var config = JsonUtility.FromJson<ConditionConfiguration>(json);
            conditions = config.conditions;

            if (randomizeConditions)
                ShuffleConditions();
        }
    }

}