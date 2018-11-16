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

        private float startTime = 0f;

        private RecordData recorder = null;

        private void Awake()
        {
            laser = FindObjectOfType<LaserPointer>();
            recorder = FindObjectOfType<RecordData>();
        }

        private void OnEnable()
        {
            LogSubtask();

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
            StopCoroutine("StartSubtask");
            StartCoroutine("StartSubtask");
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
            float completionTime = Time.realtimeSinceStartup;
            recorder.CurrentRecord.megamapTime = completionTime - startTime;

            StopCoroutine("CompleteSubtask");
            StartCoroutine("CompleteSubtask");
        }

        private void HandleWrongPinSelected()
        {
            var task = FindObjectOfType<Task>();
            task.Description = "Raum hat nicht das niedrigste Attribut. Versuche es weiter.";

            ++recorder.CurrentRecord.numErrors;
        }

        private IEnumerator StartSubtask()
        {
            yield return StartCoroutine(map.Show());
            startTime = Time.realtimeSinceStartup;
            recorder.CurrentRecord.numSelections = new int[map.LocationPins.Length];
        }

        private IEnumerator CompleteSubtask()
        {
            yield return StartCoroutine(map.Hide());
            FindObjectOfType<Task>().NextSubtask();
        }
    }

}
