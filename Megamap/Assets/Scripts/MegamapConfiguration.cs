using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegamapConfiguration : MonoBehaviour {

    [Header("Megamap Settings"), Space]

    [SerializeField, Tooltip("Reference to the actual indoor map object.")]
    private GameObject map;
    public GameObject Map { get { return map; } }

    [Range(0f, 1f)]
    public float scale = 1f;

    [SerializeField, Range(0, 100)]
    private int wallHeight = 10;
    	
	void Update () {
        map.transform.localScale = new Vector3(scale, wallHeight / 100f, scale);
	}
}
