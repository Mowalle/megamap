using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

public class DataRecording : MonoBehaviour {

    public DirectoryInfo UserFolder { get; set; }

    [SerializeField] private string userID = "";
    private string startTime = "";

    private void Start()
    {
        Assert.raiseExceptions = true;

        startTime = DateTime.UtcNow.ToString("yyyy-MM-dd_HH_mm_ss");

        DirectoryInfo baseDir = Directory.GetParent(Application.dataPath);

        try {
            DirectoryInfo di = new DirectoryInfo(baseDir.FullName + "/Results");
            if (di.Exists) {
                Debug.Log("Not creating directory \"Results\": already exists.");
            }
            else {
                di.Create();
            }

            var userDirs = di.GetDirectories("user_*");
            userID = "user_" + NextUserID(userDirs);

            UserFolder = new DirectoryInfo(di.FullName + "/" + userID + "_" + startTime);
            // Just to make sure...
            Debug.Assert(!UserFolder.Exists, "User folder would be overwritten; ABORT!");
            UserFolder.Create();
        }
        catch (Exception e) {
            Debug.LogError("Creating directory failed: " + e.ToString());
        }
    }

    private string NextUserID(DirectoryInfo[] dirs)
    {
        List<string> dirNames = new List<string>();

        foreach (var dir in dirs) {
            if (dir.Name.StartsWith("user_")) {
                dirNames.Add(dir.Name.Substring("user_".Length));
            }
        }

        dirNames.Sort();

        for (int i = dirNames.Count - 1; i >= 0; --i) {
            var fields = dirNames[i].Split('_');
            int n;
            if (int.TryParse(fields[0], out n)) {
                return (n + 1).ToString();
            }
        }

        return "0";
    }
}

































