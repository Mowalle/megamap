using UnityEngine;

namespace Megamap {

    public class SubtaskUserSetup : Subtask {

        [SerializeField]
        private FloorTarget floorTarget = null;
        [SerializeField]
        private WallTarget wallTarget = null;
        [SerializeField]
        private VRStandardAssets.Utils.SelectionRadial selectionRadial = null;
        
        private TaskDisplay taskDisplay = null;

        private LineRenderer guide = null;

        public override void StartSubtask()
        {
            LogSubtask();

            floorTarget.OnTargetEnter.AddListener(HandleFloorTargetEnter);
            floorTarget.OnTargetExit.AddListener(HandleFloorTargetExit);
            wallTarget.OnTargetEnter.AddListener(HandleWallTargetEnter);
            wallTarget.OnTargetExit.AddListener(HandleWallTargetExit);
            selectionRadial.OnSelectionComplete += HandleSelectionComplete;

            taskDisplay.CurrentDescriptionID = "userSetupPosition";

            guide.gameObject.SetActive(true);
            guide.SetPosition(1, floorTarget.transform.position);

            wallTarget.transform.position = new Vector3(floorTarget.transform.position.x,
                                                        wallTarget.transform.position.y,
                                                        wallTarget.transform.position.z);

            // Re-Center TaskDisplay (TV) at WallTarget position.
            float newX = wallTarget.GetComponent<RectTransform>().anchoredPosition.x;
            // Limit tv position on x-axis with these magic numbers (so that tv does not clip the lab wall).
            newX = Mathf.Max(newX, 5.2f);
            newX = Mathf.Min(newX, 6.2f);

            taskDisplay.transform.parent.localPosition = new Vector3(
                newX,
                taskDisplay.transform.parent.localPosition.y,
                taskDisplay.transform.parent.localPosition.z);

            // Reset radial so it won't be shown as filled on start.
            selectionRadial.Hide();

            floorTarget.gameObject.SetActive(true);
            wallTarget.gameObject.SetActive(false);

        }

        public override void StopSubtask()
        {
            floorTarget.OnTargetEnter.AddListener(HandleFloorTargetEnter);
            floorTarget.OnTargetExit.AddListener(HandleFloorTargetExit);
            wallTarget.OnTargetEnter.AddListener(HandleWallTargetEnter);
            wallTarget.OnTargetExit.RemoveListener(HandleWallTargetExit);
            selectionRadial.OnSelectionComplete -= HandleSelectionComplete;

            guide.gameObject.SetActive(false);
            floorTarget.gameObject.SetActive(false);
            wallTarget.gameObject.SetActive(false);
        }

        private void Awake()
        {
            taskDisplay = FindObjectOfType<TaskDisplay>();

            guide = transform.Find("Guide").GetComponent<LineRenderer>();
            guide.SetPosition(0, Camera.main.transform.position + Vector3.down * 0.5f);
        }

        private void Update()
        {
            guide.SetPosition(0, Camera.main.transform.position + Vector3.down * 0.5f);
        }

        private void HandleFloorTargetEnter()
        {
            wallTarget.gameObject.SetActive(true);
            guide.SetPosition(1, wallTarget.transform.position);

            taskDisplay.CurrentDescriptionID = "userSetupGaze";
        }

        private void HandleFloorTargetExit()
        {
            selectionRadial.Hide();
            wallTarget.gameObject.SetActive(false);
            guide.SetPosition(1, floorTarget.transform.position);

            taskDisplay.CurrentDescriptionID = "userSetupPosition";
        }

        private void HandleWallTargetEnter()
        {
            //taskDisplay.Description = "";
            selectionRadial.Show();
        }

        private void HandleWallTargetExit()
        {
            guide.SetPosition(1, wallTarget.transform.position);
            selectionRadial.Hide();
            taskDisplay.CurrentDescriptionID = "userSetupGaze";
        }

        private void HandleSelectionComplete()
        {
            FindObjectOfType<TaskSwitcher>().CurrentTask.NextSubtask();
        }
    }

}
