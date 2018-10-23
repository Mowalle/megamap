using UnityEngine;

namespace Megamap {

    public class SubtaskMegamap : MonoBehaviour {

        [SerializeField]
        private int absoluteMinimum = 50;
        [SerializeField]
        private int maxTargetAttributeValue = 333;
        [SerializeField]
        private int maxAttributeValue = 1000;

        public Megamap map;

        private void OnEnable()
        {
            Debug.Log("Starting the subtask \"Megamap\"");
            FindObjectOfType<TaskSwitcher>().SetTaskDescription("Finde den Raum mit dem niedrigsten Attribut.");
            
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
        }

        private void OnDisable()
        {
            foreach (LocationPin pin in map.LocationPins) {
                pin.OnSelected.RemoveListener(CheckIsCorrectPin);
            }
        }

        private void CheckIsCorrectPin(LocationPin pin)
        {
            var taskSwitcher = FindObjectOfType<TaskSwitcher>();

            // Selected pin is not correct.
            if (!pin.isTargetPin) {
                taskSwitcher.SetTaskDescription("Raum hat nicht das niedrigste Attribut. Versuche es weiter.");
                pin.SetStatus(LocationPin.Status.Error);
                return;
            }

            taskSwitcher.SwitchTask(TaskSwitcher.Type.Pointing);
        }

        public void SetMap(GameObject map)
        {
            this.map.Map = map;
        }
    }

}
