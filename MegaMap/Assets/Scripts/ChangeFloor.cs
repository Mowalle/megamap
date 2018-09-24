using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class ChangeFloor : MonoBehaviour {

        [Tooltip("Reference to the indoor map for changing floors.")]
        public IndoorMap indoorMap;

        public Image floorSelectionCursor;
        public Image floorSelectionNumbers;
        
        private Coroutine waitAndHideRoutine;
        
        private void Start()
        {
            floorSelectionCursor.enabled = false;
            floorSelectionNumbers.enabled = false;
        }

        private void Update()
        {
            float mouseInput = Input.GetAxis("Mouse ScrollWheel");
            if (mouseInput == 0f)
                return;

            if (mouseInput > 0f && indoorMap.CurrentFloor + 1 < indoorMap.GetNumberOfFloors())
                ChangeFloorNumberDisplay(++indoorMap.CurrentFloor);
            else if (mouseInput < 0f && indoorMap.CurrentFloor > 0)
                ChangeFloorNumberDisplay(--indoorMap.CurrentFloor);

            if (waitAndHideRoutine != null) {
                StopCoroutine(waitAndHideRoutine);
            }
            waitAndHideRoutine = StartCoroutine(WaitAndHideCounter());
            floorSelectionCursor.enabled = true;
            floorSelectionNumbers.enabled = true;
        }

        private IEnumerator WaitAndHideCounter()
        {
            while (true) {
                yield return new WaitForSeconds(2.0f);
                floorSelectionCursor.enabled = false;
                floorSelectionNumbers.enabled = false;
            }
        }

        private void ChangeFloorNumberDisplay(int floor)
        {
            floorSelectionNumbers.GetComponent<RectTransform>().localPosition = new Vector3(0f, -floor, 0f);
        }
    }

}