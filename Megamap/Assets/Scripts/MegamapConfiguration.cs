using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class MegamapConfiguration : MonoBehaviour {

        [Header("Megamap Settings"), Space]

        [SerializeField, Tooltip("Reference to the actual indoor map object.")]
        private GameObject map;
        public GameObject Map { get { return map; } }

        [Range(0f, 1f)]
        public float scale = 1f;

        [SerializeField, Range(0, 100)]
        private int wallHeight = 10;

        [Range(0f, 1.5f)]
        public float heightOffset = 0f;

        void Update() {
            // Apply room and wall scale.
            map.transform.localScale = new Vector3(scale, wallHeight / 100f, scale);

            // Apply height offset.
            var pos = map.transform.position;
            pos[1] = heightOffset;
            map.transform.position = pos;
        }
    }

}
