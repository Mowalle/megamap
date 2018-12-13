using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;

using Valve.VR.InteractionSystem;

namespace Megamap {

    public class Record {
        public int conditionIndex = 0;
        public Megamap.ViewMode viewMode = Megamap.ViewMode.Default;
        public Megamap.HeightMode heightMode = Megamap.HeightMode.Fixed;
        public float heightOffset = 0f;
        public float scale = 0f;
        public string mapName = "";
        public int taskIndex = 0;
        public float taskStartTime = 0f;
        public float taskEndTime = 0f;
        public float taskDuration = 0f;
        public bool skipped = false;
        public float skippedAfterSeconds = 0f;

        // Megamap subtask data.
        public float megamapTime = 0f;
        public int numErrors = 0;
        public int numRoomSelections = 0;
        public int[] roomSelections = null;
        public int correctRoomIndex = 0;
        public string correctRoomName = "";
        public int[] numBallsPerRoom = null;

        // Pointing subtask data.
        public float pointingTime = 0f;
        public float confirmationTime = 0f;
        public int numCorrections = 0;
        public Vector3 positionAtConfirmation = new Vector3();
        public Vector3 viewAtConfirmation = new Vector3();
        public Vector3 rayPosition = new Vector3();
        public Vector3 rayDirection = new Vector3();
        public bool hitRoom = false;
        public Vector3 hitLocation = Vector3.zero;
        public float horizOffsetDeg = 0f;
        public float vertOffsetDeg = 0f;
    }

    public class RecordData : MonoBehaviour {

        public static DirectoryInfo UserFolder { get; set; }
        public static Record CurrentRecord { get; set; }

        public static bool writeData = true;

        public static string userID = "";

        private static string startTime = "";
        private static StreamWriter logWriter = null;
        private static StreamWriter hmdLog = null;
        private static StreamWriter handLog = null;

        private static bool initialized = false;

        public static void DumpToDisk(DirectoryInfo directory, string name)
        {
            var writer = File.AppendText(directory.FullName + "/" + name);

            writer.WriteLine("conditionIndex: " + CurrentRecord.conditionIndex);
            writer.WriteLine("viewMode: " + CurrentRecord.viewMode);
            writer.WriteLine("heightMode: " + CurrentRecord.heightMode);
            writer.WriteLine("heightOffset: " + CurrentRecord.heightOffset);
            writer.WriteLine("scale: " + CurrentRecord.scale);
            writer.WriteLine("mapName: " + CurrentRecord.mapName);
            writer.WriteLine("taskIndex: " + CurrentRecord.taskIndex);
            writer.WriteLine("taskStartTime: " + CurrentRecord.taskStartTime);
            writer.WriteLine("taskEndTime: " + CurrentRecord.taskEndTime);
            writer.WriteLine("taskDuration: " + CurrentRecord.taskDuration);
            writer.WriteLine("skipped: " + CurrentRecord.skipped);
            writer.WriteLine("skippedAfterSeconds: " + CurrentRecord.skippedAfterSeconds);
            writer.WriteLine("megamapTime: " + CurrentRecord.megamapTime);
            writer.WriteLine("correctRoomName: " + CurrentRecord.correctRoomName);
            writer.WriteLine("correctRoomIndex: " + CurrentRecord.correctRoomIndex);
            writer.WriteLine("numBallsPerRoom: [ " + string.Join(", ", CurrentRecord.numBallsPerRoom.Select(x => x.ToString()).ToArray()) + " ]");
            writer.WriteLine("numRoomSelections: " + CurrentRecord.numRoomSelections);
            writer.WriteLine("roomSelections: [ " + string.Join(", ", CurrentRecord.roomSelections.Select(x => x.ToString()).ToArray()) + " ]");
            writer.WriteLine("numErrors: " + CurrentRecord.numErrors);
            writer.WriteLine("pointingTime: " + CurrentRecord.pointingTime);
            writer.WriteLine("confirmationTime: " + CurrentRecord.confirmationTime);
            writer.WriteLine("numCorrections: " + CurrentRecord.numCorrections);
            writer.WriteLine("positionAtConfirmation: [ " + CurrentRecord.positionAtConfirmation.x + ", " + CurrentRecord.positionAtConfirmation.y + ", " + CurrentRecord.positionAtConfirmation.z + " ]");
            writer.WriteLine("viewAtConfirmation: [ " + CurrentRecord.viewAtConfirmation.x + ", " + CurrentRecord.viewAtConfirmation.y + ", " + CurrentRecord.viewAtConfirmation.z + " ]");
            writer.WriteLine("rayPosition: [ " + CurrentRecord.rayPosition.x + ", " + CurrentRecord.rayPosition.y + ", " + CurrentRecord.rayPosition.z + " ]");
            writer.WriteLine("rayDirection: [ " + CurrentRecord.rayDirection.x + ", " + CurrentRecord.rayDirection.y + ", " + CurrentRecord.rayDirection.z + " ]");
            writer.WriteLine("hitRoom: " + CurrentRecord.hitRoom);
            writer.WriteLine("hitLocation: [ " + CurrentRecord.hitLocation.x + ", " + CurrentRecord.hitLocation.y + ", " + CurrentRecord.hitLocation.z + " ]");
            writer.WriteLine("horizOffsetDeg: " + CurrentRecord.horizOffsetDeg);
            writer.WriteLine("vertOffsetDeg: " + CurrentRecord.vertOffsetDeg);

            writer.Close();

            CurrentRecord = new Record();
        }

        public static void Log(string s)
        {
            Debug.Log(s + " [Logging]");

            if (!writeData)
                return;

            logWriter.WriteLine(Time.frameCount + " | " + Time.realtimeSinceStartup + ": " + s);
        }

        private static void CreateUserDir()
        {
            startTime = DateTime.UtcNow.ToString("yyyy-MM-dd_HH_mm_ss");

            DirectoryInfo baseDir = Directory.GetParent(Application.dataPath);

            try {
                DirectoryInfo resultsDir = baseDir.CreateSubdirectory("Results");

                UserFolder = IncrementDirectory(resultsDir, "user_", "_" + startTime);
                resultsDir.CreateSubdirectory(UserFolder.Name);

                userID = UserFolder.Name;
            }
            catch (Exception e) {
                Debug.LogError("Creating directory failed: " + e.ToString());
            }
        }

        private static DirectoryInfo IncrementDirectory(DirectoryInfo rootDir, string dirNameStem, string suffix)
        {
            var dirs = rootDir.GetDirectories();

            List<string> dirNames = new List<string>();

            foreach (var dir in dirs) {
                if (dir.Name.StartsWith(dirNameStem)) {
                    dirNames.Add(dir.Name.Substring(dirNameStem.Length));
                }
            }

            var comp = new Tempesta.Extensions.OrdinalStringComparer();
            dirNames.Sort(comp);

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

        private void Awake()
        {
            Assert.raiseExceptions = true;

            if (!initialized && writeData) {
                CreateUserDir();

                hmdLog = File.AppendText(UserFolder.FullName + "/hmd_pos_and_view.csv");
                handLog = File.AppendText(UserFolder.FullName + "/hands_pos_and_rot.csv");
                logWriter = File.AppendText(UserFolder.FullName + "/logfile.txt");
            }

            CurrentRecord = new Record();
            initialized = true;
        }

        private void OnDestroy()
        {
            if (initialized && writeData) {
                hmdLog.Close();
                handLog.Close();
                logWriter.Close();
            }

            initialized = false;
        }

        private void LateUpdate()
        {
            if (!writeData)
                return;

            var cam = Camera.main.transform;
            hmdLog.WriteLine(Time.renderedFrameCount + ", "
                + Time.time + ", "
                + cam.position.x + ", "
                + cam.position.y + ", "
                + cam.position.z + ", "
                + cam.eulerAngles.x + ", "
                + cam.eulerAngles.y + ", "
                + cam.eulerAngles.z + ", "
                + cam.name);

            foreach (var hand in FindObjectsOfType<Hand>()) {
                handLog.WriteLine(Time.renderedFrameCount + ", "
                + Time.time + ", "
                + hand.transform.position.x + ", "
                + hand.transform.position.y + ", "
                + hand.transform.position.z + ", "
                + hand.transform.eulerAngles.x + ", "
                + hand.transform.eulerAngles.y + ", "
                + hand.transform.eulerAngles.z + ", "
                + hand.name + ", "
                + hand.handType.ToString());
            }
        }
    }

}

































