using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class TaskSwitcher : MonoBehaviour {

        public enum Type {
            Searching, Pointing, PositionReset, GazeReset
        }

        [SerializeField]
        private GameObject taskDisplay;

        private Type currentType = Type.PositionReset;

        public Type GetCurrentType() { return currentType; }

        public void SwitchTask(Type taskType)
        {
            currentType = taskType;
        }
        
        private void Start()
        {
            if (taskDisplay == null) {
                Debug.LogError("TastSwitcher: Reference to taskDisplay not set; disabling script.");
                enabled = false;
                return;
            }
        }

        private void Update()
        {
            string task = "";
            switch (currentType) {
            case Type.PositionReset:
                task = "Bitte stelle dich auf das 'X'.";
                break;
            case Type.GazeReset:
                task = "Bitte schaue auf das Ziel an der Wand.";
                break;
            case Type.Searching:
                task = "Suche nach Raum mit niedrigstem Attribut.";
                break;
            case Type.Pointing:
                task = "Zeige dorthin, wo sich der ausgewählte Raum befindet.";
                break;
            default: break;
            }

            taskDisplay.GetComponent<Text>().text = task;
        }
    }

}


