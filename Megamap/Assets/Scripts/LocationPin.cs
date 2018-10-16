using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationPin : MonoBehaviour {

    [SerializeField]
    private GameObject locationPinText;

    public bool isTargetPin = false;

    public void showLocationPinText(bool show)
    {
        locationPinText.SetActive(show);
    }

    private void Start()
    {
        if (locationPinText == null) {
            Debug.LogError("LocationPin: locationPinText reference not set; disabling script.");
            enabled = false;
            return;
        }

        showLocationPinText(false);
    }
}
