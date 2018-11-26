using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class TaskDisplay : MonoBehaviour {

        [SerializeField]
        private Text taskDisplay = null;

        public string Description { get; set; }

        private void Update()
        {
            taskDisplay.text = Description;
        }
    }

}
