using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class ConditionSwitcher : MonoBehaviour {

        private int currentCondition = 0;
        public int CurrentCondition
        {
            get { return currentCondition; }
            set { SwitchCondition(value); }
        }

        [SerializeField]
        private Megamap megamap;

        [Header("Condition Settings"), Space]
        [SerializeField, Tooltip("Values for Megamap's scale, wallheight and height offset.")]
        private Vector3[] conditions;

        private void Awake()
        {
            CurrentCondition = 0;
        }

        private void SwitchCondition(int condition)
        {
            if (condition < 0 || condition >= conditions.Length)
                return;

            megamap.scale = conditions[currentCondition].x;
            megamap.wallHeight = (int)conditions[currentCondition].y;
            megamap.heightOffset = conditions[currentCondition].z;

            currentCondition = condition;
        }
    }

}