using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndoorMap : MonoBehaviour {

    [Header("Outdoor Settings"), Space]
    public GameObject outdoorView;

    [Header("Indoor Settings"), Space]
    public GameObject indoorView;
    public GameObject[] floors;

    public bool showIndoorPosition = true;
    public GameObject positionMarker;

    private bool isEntered = false;

    private int currentFloor = 0;
    public int CurrentFloor
    {
        get {
            return currentFloor;
        }

        set {
            currentFloor = value;

            // Hide all floors but enable current floor again.
            foreach (GameObject floor in floors)
                floor.SetActive(false);
            floors[currentFloor].SetActive(true);
        }
    }

    // TODO: Refactor these methods.
    public void Enter()
    {
        outdoorView.SetActive(false);
        indoorView.SetActive(true);
        isEntered = true;

        if (showIndoorPosition) {
            positionMarker.SetActive(true);
        }
    }

    public void Exit()
    {
        outdoorView.SetActive(true);
        indoorView.SetActive(false);
        isEntered = false;

        if (showIndoorPosition) {
            positionMarker.SetActive(false);
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

        CurrentFloor = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (isEntered)
                Exit();
            else
                Enter();
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
}
