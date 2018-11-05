using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class SubtaskUserSetup : MonoBehaviour {

        [SerializeField]
        private FloorTarget floorTarget;
        [SerializeField]
        private WallTarget wallTarget;
        [SerializeField]
        private VRStandardAssets.Utils.SelectionRadial selectionRadial;

        [SerializeField] private GameObject taskDisplay;

        private string positionDescription = "Bitte stelle dich auf das Ziel ('X').";
        private string gazeDescription = "Schaue das Ziel an, um den Test zu starten.";

        private Task currentTask;
        
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

        private void HandleFloorTargetEnter()
        {
            floorTarget.gameObject.SetActive(true);
            wallTarget.gameObject.SetActive(true);

            currentTask.Description = gazeDescription;
        }

        private void HandleFloorTargetExit()
        {
            selectionRadial.Hide();
            floorTarget.gameObject.SetActive(true);
            wallTarget.gameObject.SetActive(false);

            currentTask.Description = positionDescription;
        }

        private void HandleWallTargetEnter()
        {
            selectionRadial.Show();
        }

        private void HandleWallTargetExit()
        {
            selectionRadial.Hide();
            currentTask.Description = gazeDescription;
        }

        private void HandleSelectionComplete()
        {
            currentTask.NextSubtask();
        }
    }

}
