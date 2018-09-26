using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsCharacterController : MonoBehaviour {
    
    public float speed = 10.0f;

    public Camera cam;

    public float mouseSensivity = 5.0f;

    private void Awake()
    {
        if (cam == null) {
            cam = Camera.main;
        }
    }

    private void Start()
    {
    }

    private void Update()
    {
        // Rotation with mouse.
        var mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        mouseDelta *= mouseSensivity;

        cam.transform.Rotate(Vector3.left, mouseDelta.y);
        transform.Rotate(Vector3.up, mouseDelta.x);
        
        // Prevent upside down rotation.
        // FIXME: Theoretically, angleAroundX could flip sign if mouse is moved fast enough. Should be fixed.
        var angleAroundX = Vector3.SignedAngle(transform.up, cam.transform.up, cam.transform.right);
        if (angleAroundX < -90.0f) {
            cam.transform.eulerAngles = new Vector3(-90.0f, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);
        }
        else if (angleAroundX > 90.0f) {
            cam.transform.eulerAngles = new Vector3(90.0f, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);
        }

        // Movement with keys.
        float translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float strafe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        transform.Translate(strafe, 0.0f, translation);
    }
}

