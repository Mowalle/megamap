using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class SubtaskMegamap : MonoBehaviour {

        [SerializeField]
        private int absoluteMinimum = 50;
        [SerializeField]
        private int maxTargetAttributeValue = 333;
        [SerializeField]
        private int maxAttributeValue = 1000;

        private void OnEnable()
        {
            Debug.Log("Starting the subtask \"Megamap\"");

            if (maxTargetAttributeValue <= 0) {
                Debug.LogWarning("SubtaskMegamap: Invalid maxTargetAttributeValue <= 0. Using 1 instead.");
                maxTargetAttributeValue = 1;
            }

            if (maxAttributeValue <= maxTargetAttributeValue) {
                maxAttributeValue = maxTargetAttributeValue + 1;
                Debug.LogWarning("SubtaskMegamap: Invalid maxAttributeValue <= " + maxTargetAttributeValue
                                 + ". Using " + maxAttributeValue + " instead (not a good value).");
            }

            // Determine target pins' attribute value.
            var targetValue = Random.Range(absoluteMinimum, maxTargetAttributeValue + 1);
            var pins = FindObjectsOfType<LocationPin>();
            foreach (LocationPin pin in pins) {
                pin.attribute = pin.IsTargetPin ? targetValue : Random.Range(maxTargetAttributeValue + 1, maxAttributeValue);
            }
        }

    }

}
