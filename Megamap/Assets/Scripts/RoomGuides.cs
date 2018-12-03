using System;

using UnityEngine;

namespace Megamap {

    [RequireComponent(typeof(Megamap))]
    public class RoomGuides : MonoBehaviour {

        [SerializeField] private bool showUpperOnly = true;
        [SerializeField] private GameObject guides = null;

        private LineRenderer[] lines;

        private void Awake()
        {
            if (guides == null) {
                enabled = false;
                return;
            }

            lines = guides.GetComponentsInChildren<LineRenderer>();
        }

        private void OnEnable()
        {
            Array.ForEach(lines, l => l.enabled = true);
        }

        private void OnDisable()
        {
            Array.ForEach(lines, l => l.enabled = false);
        }

        private void LateUpdate()
        {
            var map = GetComponent<Megamap>();
            if (map.MapReference == null || map.LabReference == null) {
                enabled = false;
                return;
            }

            Bounds labEnvBounds = new Bounds(map.LabReference.transform.position, Vector3.zero);
            foreach (var r in map.LabReference.GetComponentsInChildren<Renderer>(true)) {
                labEnvBounds.Encapsulate(r.bounds);
            }

            var envPoints = GetBoundsPoints(labEnvBounds);

            for (int i = 0; i < 8; ++i) {
                lines[i].SetPosition(0, envPoints[i]);
                lines[i].SetPosition(1, map.transform.localToWorldMatrix.MultiplyPoint(envPoints[i]));
            }

            for (int i = 0; i < 4; ++i) {
                lines[i].enabled = !showUpperOnly;
            }
        }

        private Vector3[] GetBoundsPoints(Bounds bounds)
        {
            var points = new Vector3[8];

            // Lower four points.
            points[0] = bounds.min;
            points[1] = bounds.min + new Vector3(bounds.size.x, 0f, 0f);
            points[2] = bounds.min + new Vector3(0f, 0f, bounds.size.z);
            points[3] = bounds.min + new Vector3(bounds.size.x, 0f, bounds.size.z);

            // Upper four points.
            points[4] = bounds.min + new Vector3(0f, bounds.size.y, 0f);
            points[5] = bounds.min + new Vector3(bounds.size.x, bounds.size.y, 0f);
            points[6] = bounds.min + new Vector3(0f, bounds.size.y, bounds.size.z);
            points[7] = bounds.min + new Vector3(bounds.size.x, bounds.size.y, bounds.size.z); // Should be bounds.max.

            return points;
        }
    }
}
