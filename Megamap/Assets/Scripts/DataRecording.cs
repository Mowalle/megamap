using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

public class DataRecording : MonoBehaviour {

    public DirectoryInfo UserFolder { get; set; }

    [SerializeField] private string userID = "";
    private string startTime = "";

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

            UserFolder = IncrementDirectory(di, "user_", "_" + startTime);
            // Just to make sure...
            Debug.Assert(!UserFolder.Exists, "User folder would be overwritten; ABORT!");
            UserFolder.Create();
            userID = UserFolder.Name;
        }
        catch (Exception e) {
            Debug.LogError("Creating directory failed: " + e.ToString());
        }
    }
}

































