using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransformStatusText : MonoBehaviour {

    public Text statusText;

    public Transform targetTransform;

    // Use this for initialization
    private void Start()
    {
        if (targetTransform == null) {
            enabled = false;
            return;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        statusText.text = string.Format("Unity Position: {0}\nUnity Rotation:{1}", targetTransform.position, targetTransform.localRotation.eulerAngles);
    }
}
