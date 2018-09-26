using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    [RequireComponent(typeof(MegamapConfiguration))]
    public class ToggleMegamap : MonoBehaviour {

        [SerializeField]
        private GameObject player;

        private GameObject map;

        private void Start()
        {
            map = GetComponent<MegamapConfiguration>().Map;
        }

        private void Update()
        {
            if (Input.GetButtonDown("Fire1")) {
                if (map.activeSelf) {
                    map.SetActive(false);
                }
                else {
                    PlaceRelativeToPlayer();
                    map.SetActive(true);
                }
            }
        }

        private void PlaceRelativeToPlayer()
        {
            var offset = player.GetComponent<FpsCharacterController>().GetPositionRelativeToLab();
            offset *= GetComponent<MegamapConfiguration>().scale;

            map.transform.position = player.transform.position - offset;
        }
    }

}
