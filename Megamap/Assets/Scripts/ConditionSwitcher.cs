using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Megamap {

    public class ConditionSwitcher : MonoBehaviour {

        // For .json loading

        [Serializable]
        private struct Condition {
            public float scale;
            public int wallHeight;
            public float heightOffset;
        }

        [Serializable]
        private struct ConditionConfiguration {
            public Condition[] conditions;
        }

        //------------------

        private int currentCondition = 0;
        public int CurrentCondition
        {
            get { return currentCondition; }
            set { SwitchCondition(value); }
        }

        [SerializeField]
        private Megamap megamap;

        [Header("Condition Settings"), Space]
        [SerializeField]
        private TextAsset conditionsJson;
        [SerializeField]
        private Condition[] conditions = new Condition[0];
        
        private void Awake()
        {
            var json = conditionsJson.text;
            var config = JsonUtility.FromJson<ConditionConfiguration>(json);
            conditions = config.conditions;

            ShuffleConditions();
            CurrentCondition = 0;
        }

        private void SwitchCondition(int condition)
        {
            if (condition < 0 || condition >= conditions.Length)
                return;

            megamap.scale = conditions[currentCondition].scale;
            megamap.wallHeight = (int)conditions[currentCondition].wallHeight;
            megamap.heightOffset = conditions[currentCondition].heightOffset;

            currentCondition = condition;
        }

        private void ShuffleConditions()
        {
            System.Random rnd = new System.Random();
            conditions = new List<Condition>(conditions).OrderBy(x => rnd.Next()).ToArray();
        }
    }

}