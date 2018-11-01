using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGuides : MonoBehaviour {

    [SerializeField] private Renderer labRoomRenderer;
    [SerializeField] private Renderer mapRoomRenderer;

    private void Update()
    {
        var labPoints = GetBoundsPoints(labRoomRenderer.bounds);
        var roomPoints = GetBoundsPoints(mapRoomRenderer.bounds);

        for (int i = 0; i < 8; ++i) {
            Debug.DrawLine(labPoints[i], roomPoints[i], Color.red);
        }
    }

    private Vector3[] GetBoundsPoints(Bounds bounds)
    {
        var points = new Vector3[8];

        points[0] = bounds.min;
        points[1] = bounds.min + new Vector3(bounds.size.x, 0f, 0f);
        points[2] = bounds.min + new Vector3(0f, 0f, bounds.size.z);
        points[3] = bounds.min + new Vector3(bounds.size.x, 0f, bounds.size.z);
        points[4] = bounds.min + new Vector3(0f, bounds.size.y, 0f);
        points[5] = bounds.min + new Vector3(bounds.size.x, bounds.size.y, 0f);
        points[6] = bounds.min + new Vector3(0f, bounds.size.y, bounds.size.z);
        points[7] = bounds.min + new Vector3(bounds.size.x, bounds.size.y, bounds.size.z); // Should be bounds.max.

        return points;
    }
}
