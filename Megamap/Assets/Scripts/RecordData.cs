﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Megamap {

    public class Record {
        public int conditionIndex = 0;
        public int taskIndex = 0;

        // Megamap subtask data.
        public float megamapTime = 0f;
        public int numErrors = 0;
        public int numSelectionsTotal = 0;
        public int[] numSelections = null;
        public int correctPinIdx = 0;

        // Pointing subtask data.
        public float pointingTime = 0f;
        public float confirmationTime = 0f;
        public int numCorrections = 0;
        public Vector3 positionAtConfirmation = new Vector3();
        public Vector3 viewAtConfirmation = new Vector3();
        public Vector3 rayPosition = new Vector3();
        public Vector3 rayDirection = new Vector3();
        public float horizOffsetDeg = 0f;
        public float vertOffsetDeg = 0f;
    }

    public class RecordData : MonoBehaviour {

        public static DirectoryInfo UserFolder { get; set; }
        public static Record CurrentRecord { get; set; }

        public static bool writeData = true;

        [SerializeField] private static string userID = "";

        private static string startTime = "";
        private static StreamWriter logWriter = null;
        private static StreamWriter csvWriter = null;

        private static bool initialized = false;

        public static void DumpToDisk(DirectoryInfo directory, string name)
        {
            var writer = File.AppendText(directory.FullName + "/" + name);

            writer.WriteLine("conditionIndex: " + CurrentRecord.conditionIndex);
            writer.WriteLine("taskIndex: " + CurrentRecord.taskIndex);
            writer.WriteLine("megamapTime: " + CurrentRecord.megamapTime);
            writer.WriteLine("numErrors: " + CurrentRecord.numErrors);
            writer.WriteLine("numSelectionsTotal: " + CurrentRecord.numSelectionsTotal);
            writer.WriteLine("numSelections: " + string.Join(", ", CurrentRecord.numSelections.Select(x => x.ToString()).ToArray()));
            writer.WriteLine("correctPinIdx: " + CurrentRecord.correctPinIdx);
            writer.WriteLine("pointingTime: " + CurrentRecord.pointingTime);
            writer.WriteLine("confirmationTime: " + CurrentRecord.confirmationTime);
            writer.WriteLine("numCorrections: " + CurrentRecord.numCorrections);
            writer.WriteLine("positionAtConfirmation: " + CurrentRecord.positionAtConfirmation.x + ", " + CurrentRecord.positionAtConfirmation.y + ", " + CurrentRecord.positionAtConfirmation.z);
            writer.WriteLine("viewAtConfirmation: " + CurrentRecord.viewAtConfirmation.x + ", " + CurrentRecord.viewAtConfirmation.y + ", " + CurrentRecord.viewAtConfirmation.z);
            writer.WriteLine("rayPosition: " + CurrentRecord.rayPosition.x + ", " + CurrentRecord.rayPosition.y + ", " + CurrentRecord.rayPosition.z);
            writer.WriteLine("rayDirection: " + CurrentRecord.rayDirection.x + ", " + CurrentRecord.rayDirection.y + ", " + CurrentRecord.rayDirection.z);
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

            logWriter.WriteLine(Time.realtimeSinceStartup + "(" + Time.frameCount + "): " + s);
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

        private void Awake()
        {
            Assert.raiseExceptions = true;

            if (!initialized && writeData) {
                CreateUserDir();

                csvWriter = File.AppendText(UserFolder.FullName + "/position_and_view_total.csv");
                logWriter = File.AppendText(UserFolder.FullName + "/logfile.txt");
            }

            CurrentRecord = new Record();
            initialized = true;
        }

        private void OnDestroy()
        {
            if (initialized && writeData) {
                csvWriter.Close();
                logWriter.Close();
            }

            initialized = false;
        }

        private void LateUpdate()
        {
            if (!writeData)
                return;

            var cam = Camera.main.transform;
            csvWriter.WriteLine(Time.time + ", "
                + cam.position.x + ", "
                + cam.position.y + ", "
                + cam.position.z + ", "
                + cam.rotation.eulerAngles.x + ", "
                + cam.rotation.eulerAngles.y + ", "
                + cam.rotation.eulerAngles.z);
        }
    }

}

































