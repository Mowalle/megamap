using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndoorMap : MonoBehaviour {

    public GameObject outdoorView;
    public GameObject indoorView;

    public bool showIndoorPosition = true;
    public GameObject positionMarker;

    private bool isEntered = false;

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
