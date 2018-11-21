using System.Collections;

using UnityEngine;

namespace Megamap {

    public class Megamap : MonoBehaviour {

        [Header("References"), Space]
        [SerializeField] private GameObject mapModel = null;
        [SerializeField] private UserMarker userMarker = null;

        [Header("Megamap Settings"), Space]
        [Range(0.01f, 1f)]
        public float scale = 1f;

        [Range(0.1f, 1.5f)]
        public float heightOffset = 0f;

        [SerializeField] private bool placeAtPlayer = true;

        // --- Properties/Getter/Setter --- //

        [SerializeField] private Transform labReference = null;
        public Transform LabReference
        {
            get { return labReference; }
            set { labReference = value; }
        }

        [SerializeField] private Transform mapReference = null;
        public Transform MapReference
        {
            get { return mapReference; }
            set { mapReference = value; }
        }

        private bool isShown = false;
        private bool shouldChangeVisible = true;

        public bool useTransition = true;
        public float transitionDuration = 1f;
        private Coroutine transitionRoutine = null;

        // -------------------------------- //

        public void Show(bool showMap)
        {
            shouldChangeVisible = isShown != showMap;
        }

        public bool IsShown() { return isShown; }

        private void Awake()
        {
            if (mapModel == null
                || userMarker == null
                || labReference == null
                || mapReference == null) {
                Debug.LogError("Megamap: References not set up correctly; disabling script");
                enabled = false;
                return;
            }
        }

        private void Update()
        {
            if (shouldChangeVisible) {
                if (transitionRoutine == null)
                    transitionRoutine = isShown ? StartCoroutine(HideRoutine()) : StartCoroutine(ShowRoutine());

                return;
            }

            if (!shouldChangeVisible && !isShown)
                return;

            // Apply room and wall scale.
            transform.localScale = new Vector3(scale, scale, scale);

            // Apply height offset.
            transform.position = new Vector3(transform.position.x, heightOffset, transform.position.z);
        }

        private IEnumerator ShowRoutine()
        {
            mapModel.SetActive(true);
            userMarker.gameObject.SetActive(false);

            var targetPosition = placeAtPlayer ? GetPlayerOffsetPosition() : transform.position;
            targetPosition.y = heightOffset;
            yield return StartCoroutine(Transition(
                transform,
                labReference.position,
                targetPosition,
                Vector3.one,
                new Vector3(scale, scale, scale),
                transitionDuration));

            userMarker.gameObject.SetActive(true);

            isShown = true;
            shouldChangeVisible = false;
            transitionRoutine = null;
        }

        private IEnumerator HideRoutine()
        {
            userMarker.gameObject.SetActive(false);

            yield return StartCoroutine(Transition(
                transform,
                transform.position,
                labReference.position,
                new Vector3(scale, scale, scale),
                Vector3.one,
                transitionDuration));

            mapModel.SetActive(false);

            isShown = false;
            shouldChangeVisible = false;
            transitionRoutine = null;
        }

        private IEnumerator Transition(
            Transform transform,
            Vector3 startPosition,
            Vector3 endPosition,
            Vector3 startScale,
            Vector3 endScale,
            float duration)
        {
            // Lerp position, scale and alpha (transparency).
            float rate = 1.0f / duration;
            float t = 0f;
            while (t < 1f) {
                t += Time.deltaTime * rate;
                transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0f, 1f, t));
                transform.localScale = Vector3.Lerp(startScale, endScale, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
        }

        private Vector3 GetPlayerOffsetPosition()
        {
            var player = Camera.main;
            var offset = player.transform.position - labReference.position;
            offset *= scale;

            return player.transform.position - offset;
        }
    }

}
