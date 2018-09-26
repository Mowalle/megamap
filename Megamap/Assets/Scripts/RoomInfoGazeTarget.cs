using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    [RequireComponent(typeof(GazeTarget))]
    public class RoomInfoGazeTarget : MonoBehaviour {

        private void OnEnable()
        {
            var gazeTarget = GetComponent<GazeTarget>();
            gazeTarget.OnSelection += HandleOnSelection;
            gazeTarget.OnOut += HandleOnOut;
        }

        private void OnDisable()
        {
            var gazeTarget = GetComponent<GazeTarget>();
            gazeTarget.OnSelection -= HandleOnSelection;
            gazeTarget.OnOut -= HandleOnOut;
        }

        private void HandleOnSelection()
        {
            GetComponentInChildren<Image>().enabled = true;
            GetComponentInChildren<Text>().enabled = true;
        }

        private void HandleOnOut()
        {
            GetComponentInChildren<Image>().enabled = false;
            GetComponentInChildren<Text>().enabled = false;
        }
    }

}