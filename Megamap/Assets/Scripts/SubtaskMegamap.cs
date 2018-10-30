using UnityEngine;

namespace Megamap {

    public class SubtaskMegamap : MonoBehaviour {

        [Header("Pin randomization values")]
        [SerializeField]
        private int absoluteMinimum = 50;
        [SerializeField]
        private int maxTargetAttributeValue = 333;
        [SerializeField]
        private int maxAttributeValue = 1000;

        public Megamap map;

        private readonly string description = "Finde den Raum mit dem niedrigsten Attribut.";

        private void CheckIsCorrectPin(LocationPin pin)
        {
            var task = FindObjectOfType<Task>();

            // Selected pin is not correct.
            if (!pin.isTargetPin) {
                task.Description = "Raum hat nicht das niedrigste Attribut. Versuche es weiter.";
                pin.SetStatus(LocationPin.Status.Error);
            }
            else {
                task.NextSubtask();
            }
        }


        private void OnEnable()
        {
            Debug.Log("Starting the subtask \"Megamap\"");
            FindObjectOfType<Task>().Description = description;

            if (maxTargetAttributeValue <= 0) {
                Debug.LogWarning("SubtaskMegamap: Invalid maxTargetAttributeValue <= 0. Using 1 instead.");
                maxTargetAttributeValue = 1;
            }

            if (maxAttributeValue <= maxTargetAttributeValue) {
                maxAttributeValue = maxTargetAttributeValue + 1;
                Debug.LogWarning("SubtaskMegamap: Invalid maxAttributeValue <= " + maxTargetAttributeValue
                                 + ". Using " + maxAttributeValue + " instead (not a good value).");
            }

            // Determine target pins' attribute value (pseudo-randomize).
            int targetValue = Random.Range(absoluteMinimum, maxTargetAttributeValue + 1);
            foreach (LocationPin pin in map.LocationPins) {
                pin.roomNumber = Random.Range(100, 1000);
                pin.attribute = pin.isTargetPin ? targetValue : Random.Range(maxTargetAttributeValue + 1, maxAttributeValue);
                pin.OnSelected.AddListener(CheckIsCorrectPin);
            }

            // Update Megamap with values from condition.
            var condition = FindObjectOfType<ConditionSwitcher>().CurrentCondition;
            map.scale = condition.scale;
            map.wallHeight = condition.wallHeight;
            map.heightOffset = condition.heightOffset;

            // Because the scale is changed (see above) AFTER Megamap's OnEnable(), we need to place it again at the player's position.
            // Otherwise, the old scale will be used for calculating the offset, which would lead to a wrongly placed map.
            if (map.placeAtPlayerOnEnable)
                map.PlaceAtPlayer();
        }

        private void OnDisable()
        {
            map.gameObject.SetActive(false);
            foreach (LocationPin pin in map.LocationPins) {
                pin.OnSelected.RemoveListener(CheckIsCorrectPin);
            }
        }
    }

}
