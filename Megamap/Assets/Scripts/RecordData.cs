using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace Megamap {

    public class Record {
        public int conditionIndex;
        public int taskIndex;
        public float megamapTime;
        public int numErrors;
        public int numSelectionsTotal;
        public int[] numSelections;
        public float pointingTime;
        public float confirmationTime;
        public Vector3 positionAtConfirmation;
        public Vector3 viewAtConfirmation;
        public Vector3 handPosition;
        public Vector3 rayDirection;
        public float horizOffsetDeg;
        public float vertOffsetDeg;

        public void WriteToDisk(DirectoryInfo directory, string name)
        {}
    }

    public class RecordData : MonoBehaviour {

        public DirectoryInfo UserFolder { get; set; }
        public Record CurrentRecord { get; set; }

        public bool writeData = true;

        [SerializeField] private string userID = "";

        private string startTime = "";
        private StreamWriter csvWriter = null;
        private StreamWriter logWriter = null;

        public static DirectoryInfo IncrementDirectory(DirectoryInfo rootDir, string dirNameStem, string suffix)
        {
            var dirs = rootDir.GetDirectories();

            List<string> dirNames = new List<string>();

            foreach (var dir in dirs) {
                if (dir.Name.StartsWith(dirNameStem)) {
                    dirNames.Add(dir.Name.Substring(dirNameStem.Length));
                }
            }

            dirNames.Sort();

            string newDirNumber = "0";
            for (int i = dirNames.Count - 1; i >= 0; --i) {
                var fields = dirNames[i].Split('_');
                int n;
                if (int.TryParse(fields[0], out n)) {
                    newDirNumber = (n + 1).ToString();
                    break;
                }
            }

            var newDir = new DirectoryInfo(rootDir.FullName + "/" + dirNameStem + newDirNumber + suffix);
            // Just to make sure...
            Debug.Assert(!newDir.Exists, "User folder would be overwritten; ABORT!");
            newDir.Create();
            return newDir;
        }

        public void Log(string s) { logWriter.WriteLine(Time.realtimeSinceStartup.ToString() + ": " + s); }

        private void Awake()
        {
            enabled = writeData;
        }

        private void Start()
        {
            Assert.raiseExceptions = true;

            CreateUserDir();

            csvWriter = File.AppendText(UserFolder.FullName + "/position_and_view_total.csv");
            logWriter = File.AppendText(UserFolder.FullName + "/logfile.txt");

            CurrentRecord = new Record();
        }

        private void OnDestroy()
        {
            csvWriter.Close();
            logWriter.Close();
        }

        private void LateUpdate()
        {
            var cam = Camera.main.transform;
            csvWriter.WriteLine(Time.time + ", "
                + cam.position.x + ", "
                + cam.position.y + ", "
                + cam.position.z + ", "
                + cam.rotation.eulerAngles.x + ", "
                + cam.rotation.eulerAngles.y + ", "
                + cam.rotation.eulerAngles.z);
        }

        private void CreateUserDir()
        {
            startTime = DateTime.UtcNow.ToString("yyyy-MM-dd_HH_mm_ss");

            DirectoryInfo baseDir = Directory.GetParent(Application.dataPath);

            try {
                DirectoryInfo resultsDir = baseDir.CreateSubdirectory("Results");

                UserFolder = IncrementDirectory(resultsDir, "user_", "_" + startTime);
                resultsDir.CreateSubdirectory(UserFolder.Name);

                userID = UserFolder.Name;
            } catch (Exception e) {
                Debug.LogError("Creating directory failed: " + e.ToString());
                enabled = false;
            }
        }
    }

}

































