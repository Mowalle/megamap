using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class ChangeFloor : MonoBehaviour {

        [SerializeField, Tooltip("Reference to the indoor map for changing floors.")]
        private IndoorMap indoorMap;

        [SerializeField]
        private Image floorSelectionCursor;

        [SerializeField]
        private Text currentFloorText, nextFloorText, previousFloorText;
        
        private Coroutine waitAndHideRoutine;
        
        private void Start()
        {
            SetFloorDisplayVisible(false);
            UpdateFloorDisplay();
        }

        private void Update()
        {
            // Don't do anything if not inside of building.
            if (!indoorMap.IsEntered)
                return;

            float mouseInput = Input.GetAxis("Mouse ScrollWheel");
            if (mouseInput == 0f)
                return;

            if (mouseInput > 0f && indoorMap.CurrentFloor + 1 < indoorMap.GetNumberOfFloors())
                ++indoorMap.CurrentFloor;
            else if (mouseInput < 0f && indoorMap.CurrentFloor > 0)
                --indoorMap.CurrentFloor;
            UpdateFloorDisplay();

            if (waitAndHideRoutine != null) {
                StopCoroutine(waitAndHideRoutine);
            }
            waitAndHideRoutine = StartCoroutine(WaitAndHideCounter());
            SetFloorDisplayVisible(true);
        }

        private IEnumerator WaitAndHideCounter()
        {
            while (true) {
                yield return new WaitForSeconds(2.0f);
                SetFloorDisplayVisible(false);
            }
        }

        private void UpdateFloorDisplay()
        {
            currentFloorText.text = indoorMap.CurrentFloor.ToString();

            if (indoorMap.CurrentFloor == indoorMap.GetNumberOfFloors() - 1)
                nextFloorText.text = "";
            else
                nextFloorText.text = (indoorMap.CurrentFloor + 1).ToString();

            if (indoorMap.CurrentFloor == 0)
                previousFloorText.text = "";
            else
                previousFloorText.text = (indoorMap.CurrentFloor - 1).ToString();
        }

        private void SetFloorDisplayVisible(bool visible)
        {
            floorSelectionCursor.enabled = visible;
            currentFloorText.enabled = visible;
            nextFloorText.enabled = visible;
            previousFloorText.enabled = visible;
        }
    }

}