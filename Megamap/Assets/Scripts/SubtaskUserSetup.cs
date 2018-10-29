using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class SubtaskUserSetup : MonoBehaviour {

        [SerializeField]
        private FloorTarget floorTarget;
        [SerializeField]
        private WallTarget wallTarget;
        [SerializeField]
        private VRStandardAssets.Utils.SelectionRadial selectionRadial;

        private TaskSwitcher switcher;

        private string positionDescription = "Bitte stelle dich auf das Ziel ('X').";
        private string gazeDescription = "Schaue das Ziel an, um den Test zu starten.";

        private void Awake()
        {
            switcher = FindObjectOfType<TaskSwitcher>();
        }

        private void OnEnable()
        {
            Debug.Log("Starting the subtask \"User Setup\"");
            
            floorTarget.OnTargetEnter.AddListener(HandleFloorTargetEnter);
            floorTarget.OnTargetExit.AddListener(HandleFloorTargetExit);
            wallTarget.OnTargetEnter.AddListener(HandleWallTargetEnter);
            wallTarget.OnTargetExit.AddListener(HandleWallTargetExit);
            selectionRadial.OnSelectionComplete += HandleSelectionComplete;

            switcher.SetTaskDescription(positionDescription);
            floorTarget.gameObject.SetActive(true);
            wallTarget.gameObject.SetActive(false);
            selectionRadial.Hide();
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
            switcher.SetTaskDescription(gazeDescription);
            switcher.SwitchTask(TaskSwitcher.Type.UserGazeSetup);
        }

        private void HandleFloorTargetExit()
        {
            selectionRadial.Hide();
            floorTarget.gameObject.SetActive(true);
            wallTarget.gameObject.SetActive(false);
            switcher.SetTaskDescription(positionDescription);
            switcher.SwitchTask(TaskSwitcher.Type.UserPositionSetup);
        }

        private void HandleWallTargetEnter()
        {
            selectionRadial.Show();
        }

        private void HandleWallTargetExit()
        {
            selectionRadial.Hide();
            switcher.SetTaskDescription(gazeDescription);
        }

        private void HandleSelectionComplete()
        {
            switcher.SwitchTask(TaskSwitcher.Type.Searching);
        }
    }

}
