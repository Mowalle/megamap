using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace Megamap {

    public class SubtaskPointing : MonoBehaviour {

        public SteamVR_Action_Boolean action;
        public Hand hand;
        public GameObject laser;

        private string taskDescription = "Zeige dorthin, wo sich der ausgewählte Raum befindet.";
        private string confirmation = "Zum Bestätigen, Trigger betätigen.";

        private void OnEnable()
        {
            FindObjectOfType<TaskSwitcher>().SetTaskDescription(taskDescription);
        }

        private void Update()
        {
            laser.transform.position = hand.objectAttachmentPoint.position;
            laser.transform.rotation = hand.transform.rotation;
        }
    }

}