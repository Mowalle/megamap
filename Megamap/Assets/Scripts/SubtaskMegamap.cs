using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class SubtaskMegamap : Subtask {

        [Header("Pin randomization values")]
        [SerializeField]
        private int absoluteMinimum = 50;
        [SerializeField]
        private int maxTargetAttributeValue = 333;
        [SerializeField]
        private int maxAttributeValue = 1000;

        public Megamap map;

        private readonly string description = "Finde den Raum mit dem niedrigsten Attribut.";

        private LaserPointer laser;

        private void Awake()
        {
            laser = FindObjectOfType<LaserPointer>();
            laser.Show(false);
        }

        private void OnEnable()
        {
            Debug.Log("Starting the subtask \"Megamap\"");
            FindObjectOfType<Task>().Description = description;

            laser.Show(true);

            // Randomization of LocationPins.
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
                pin.OnTargetPinSelected.AddListener(HandleTargetPinSelected);
                pin.OnWrongPinSelected.AddListener(HandleWrongPinSelected);
            }

            // Update Megamap with values from condition.
            var condition = FindObjectOfType<ConditionSwitcher>().CurrentCondition;
            map.scale = condition.scale;
            map.heightOffset = condition.heightOffset;

            // Map animation.
            StartCoroutine(map.Show());
        }

        private void OnDisable()
        {
            if (laser != null)
                laser.Show(false);

            foreach (LocationPin pin in map.LocationPins) {
                pin.OnTargetPinSelected.RemoveListener(HandleTargetPinSelected);
                pin.OnWrongPinSelected.RemoveListener(HandleWrongPinSelected);
            }
        }

        private void HandleTargetPinSelected()
        {
            StopCoroutine("CompleteTask");
            StartCoroutine("CompleteTask");
        }

        private void HandleWrongPinSelected()
        {
            var task = FindObjectOfType<Task>();
            task.Description = "Raum hat nicht das niedrigste Attribut. Versuche es weiter.";
        }

        private IEnumerator CompleteTask()
        {
            yield return StartCoroutine(map.Hide());
            FindObjectOfType<Task>().NextSubtask();
        }
        
       
    }

}
