using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndoorMap : MonoBehaviour {

    [Header("Outdoor Settings"), Space]
    public GameObject outdoorView;

    [Header("Indoor Settings"), Space]
    public GameObject[] floors;

    public bool showIndoorPosition = true;
    public GameObject positionMarker;

    private bool isEntered = false;
    public bool IsEntered
    {
        get { return isEntered; }
        set {
            isEntered = value;

            // When entering indoor map, disable outdoor view and show current floor.
            // Vice versa when exiting.
            outdoorView.SetActive(!isEntered);

            // When entering, ensure that correct floor is displayed.
            CurrentFloor = currentFloor;
            
            // Hide current floor when exiting to avoid overlap with outside model.
            if (!isEntered)
                floors[currentFloor].SetActive(false);
            
            if (showIndoorPosition) {
                positionMarker.SetActive(isEntered);
            }
        }
    }

    private int currentFloor = 0;
    public int CurrentFloor
    {
        get {
            return currentFloor;
        }

        set {
            currentFloor = value;
            HideFloors();
            floors[currentFloor].SetActive(true);
        }
    }

    public int GetNumberOfFloors()
    {
        return floors.Length;
    }

    private void Start()
    {
        if (floors.Length < 1) {
            Debug.LogError("Indoor Map must have at least one floor.");
            enabled = false;
            return;
        }

        IsEntered = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            IsEntered = !IsEntered;
        }

        if (!showIndoorPosition)
            return;

        // Get characters position offset from MZH southwest corner.
        var player = GameObject.FindGameObjectWithTag("Player");
        var mzh = GameObject.Find("MZH_Outside");
        var offset = player.transform.position - mzh.transform.position;

        // Position marker accordingly.
        positionMarker.transform.localPosition = offset;
        positionMarker.transform.rotation = player.transform.rotation;
    }

    private void HideFloors()
    {
        foreach (GameObject floor in floors)
            floor.SetActive(false);
    }
}
