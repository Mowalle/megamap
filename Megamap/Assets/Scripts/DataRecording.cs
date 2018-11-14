using System;
using System.IO;
using UnityEngine;

public class DataRecording : MonoBehaviour {

    [SerializeField] private string userID = "";

    DirectoryInfo baseDir = null;

    private void Start()
    {
        baseDir = Directory.GetParent(Application.dataPath);

        try {
            DirectoryInfo di = new DirectoryInfo(baseDir.FullName + "/Results");
            if (di.Exists) {
                Debug.Log("Not creating directory \"Results\": already exists.");
            }
            else {
                di.Create();
            }
        }
        catch (Exception e) {
            Debug.LogError("Creating directory failed: " + e.ToString());
        }
    }
}

































