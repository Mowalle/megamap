using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class InstructorUI : MonoBehaviour {

        public Text userID = null;
        public Text runtime = null;
        public Text tutorials = null;
        public Text tasks = null;
        
        void Start()
        {
            userID.text = "Current User: " + RecordData.userID;
        }

        void Update()
        {
            var time = Time.time;
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time - minutes * 60);

            runtime.text = "Runtime: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
