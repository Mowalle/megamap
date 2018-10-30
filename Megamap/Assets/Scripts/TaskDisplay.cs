using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class TaskDisplay : MonoBehaviour {

        [SerializeField]
        private Text taskDisplay;

        private void Update()
        {
            var task = FindObjectOfType<Task>();
            if (task == null) {
                return;
            }

            taskDisplay.text = task.Description;
        }
    }

}
