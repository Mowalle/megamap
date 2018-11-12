using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class SubtaskUserSetup : MonoBehaviour {

        [SerializeField]
        private FloorTarget floorTarget = null;
        [SerializeField]
        private WallTarget wallTarget = null;
        [SerializeField]
        private VRStandardAssets.Utils.SelectionRadial selectionRadial = null;

        [SerializeField] private GameObject taskDisplay = null;

        private string positionDescription = "Bitte stelle dich auf das Ziel ('X').";
        private string gazeDescription = "Schaue das Ziel an, um den Test zu starten.";

        private Task currentTask;

        private LineRenderer guide = null;

        private void Awake()
        {
            guide = transform.Find("Guide").GetComponent<LineRenderer>();
            guide.SetPosition(0, Camera.main.transform.position + Vector3.down * 0.5f);
        }

        private void OnEnable()
        {
            Debug.Log("Starting the subtask \"User Setup\"");
            
            floorTarget.OnTargetEnter.AddListener(HandleFloorTargetEnter);
            floorTarget.OnTargetExit.AddListener(HandleFloorTargetExit);
            wallTarget.OnTargetEnter.AddListener(HandleWallTargetEnter);
            wallTarget.OnTargetExit.AddListener(HandleWallTargetExit);
            selectionRadial.OnSelectionComplete += HandleSelectionComplete;

            // Should return the only active task.
            currentTask = FindObjectOfType<Task>();
            currentTask.Description = positionDescription;

            floorTarget.gameObject.SetActive(true);
            wallTarget.gameObject.SetActive(false);
            selectionRadial.Hide();
            guide.enabled = true;
            guide.SetPosition(1, floorTarget.transform.position);

            wallTarget.transform.position = new Vector3(floorTarget.transform.position.x,
                                                        wallTarget.transform.position.y,
                                                        wallTarget.transform.position.z);

            // Re-Center TaskDisplay (TV) at WallTarget position.
            taskDisplay.transform.position = new Vector3(
                wallTarget.transform.position.x,
                wallTarget.transform.position.y,
                taskDisplay.transform.position.z);
        }

        private void OnDisable()
        {
            floorTarget.OnTargetEnter.AddListener(HandleFloorTargetEnter);
            floorTarget.OnTargetExit.AddListener(HandleFloorTargetExit);
            wallTarget.OnTargetEnter.AddListener(HandleWallTargetEnter);
            wallTarget.OnTargetExit.RemoveListener(HandleWallTargetExit);
            selectionRadial.OnSelectionComplete -= HandleSelectionComplete;
        }

        private void Update()
        {
            guide.SetPosition(0, Camera.main.transform.position + Vector3.down * 0.5f);
        }

        private void HandleFloorTargetEnter()
        {
            floorTarget.gameObject.SetActive(true);
            wallTarget.gameObject.SetActive(true);
            guide.SetPosition(1, wallTarget.transform.position);

            currentTask.Description = gazeDescription;
        }

        private void HandleFloorTargetExit()
        {
            selectionRadial.Hide();
            floorTarget.gameObject.SetActive(true);
            wallTarget.gameObject.SetActive(false);
            guide.SetPosition(1, floorTarget.transform.position);

            currentTask.Description = positionDescription;
        }

        private void HandleWallTargetEnter()
        {
            currentTask.Description = "";
            selectionRadial.Show();
        }

        private void HandleWallTargetExit()
        {
            guide.SetPosition(1, wallTarget.transform.position);
            selectionRadial.Hide();
            currentTask.Description = gazeDescription;
        }

        private void HandleSelectionComplete()
        {
            currentTask.NextSubtask();
        }
    }

}
