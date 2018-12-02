using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Megamap {

    public class Megamap : MonoBehaviour {

        public enum Mode { Flat, Default }

        [Header("General Settings"), Space]
        [SerializeField] private Mode mode = Mode.Default;
        [SerializeField] private bool useAnimation = true;
        public bool UseAnimation { get { return useAnimation; } set { useAnimation = value; } }
        public float animationDuration = 1.5f;

        [Header("2D Settings"), Space]
        [SerializeField] private Transform targetTransform = null;

        [Header("3D Settings"), Space]
        [Range(0.01f, 1f)] public float scale = 1f;
        [Range(0.1f, 1.5f)] public float heightOffset = 0f;
        [SerializeField] private Transform labReference = null;
        public Transform LabReference { get { return labReference; } set { labReference = value; } }

        public List<GameObject> Rooms { get { return indoorMap.Rooms; } }
        public List<SelectRoom> SelectableRooms { get { return indoorMap.SelectableRooms; } }
        public SelectRoom TargetRoom { get { return indoorMap.TargetRoom; } }

        private IndoorMap indoorMap = null;
        private Transform previousParent = null;
        private Transform mapReference = null;
        public Transform MapReference { get { return mapReference; } }

        // Animation state.
        public bool IsShown { get { return isShown; } }
        private bool isShown = false;
        private Coroutine animationRoutine = null;

        // -------------------------------- //

        public Mode GetMode() { return mode; }
        public void SetMode(Mode mode) { this.mode = mode; }

        public void SetMap(IndoorMap indoorMap, Transform referencePoint = null)
        {
            if (indoorMap == null) {
                this.indoorMap = null;
                mapReference = null;
                return;
            }

            if (this.indoorMap != null) {
                this.indoorMap.transform.SetParent(previousParent);
                this.indoorMap.transform.localPosition = Vector3.zero;
                this.indoorMap.gameObject.SetActive(false);
            }

            this.indoorMap = indoorMap;
            previousParent = this.indoorMap.transform.parent;
            this.indoorMap.transform.SetParent(transform);
            this.indoorMap.transform.localPosition = Vector3.zero;
            this.indoorMap.gameObject.SetActive(true);
            mapReference = referencePoint != null ? referencePoint : indoorMap.transform;
        }

        public void Show()
        {
            if (isShown || animationRoutine != null)
                return;

            // Will be overriden in 2D-Mode.
            transform.position = GetPlayerOffsetPosition();
            gameObject.SetActive(true);
            // Animaiton.
            animationRoutine = StartCoroutine(ShowRoutine());
        }

        public void Hide()
        {
            if (!isShown || animationRoutine != null)
                return;

            // Animation.
            animationRoutine = StartCoroutine(HideRoutine());
        }

        private void Update()
        {
            if (MapReference == null) {
                enabled = false;
                return;
            }

            if (!isShown || animationRoutine != null)
                return;

            UpdateTransform();
        }

        private IEnumerator ShowRoutine()
        {
            // Disable rooms during transition to prevent accidental selection.
            SelectableRooms.ForEach(room => room.EnableInteraction(false));

            if (useAnimation) {
                Vector3 targetPosition, targetScale;
                Quaternion targetRotation;
                if (mode == Mode.Default) {
                    targetPosition = GetPlayerOffsetPosition();
                    targetPosition.y = heightOffset;
                    targetRotation = Quaternion.Euler(Vector3.zero);
                    targetScale = new Vector3(scale, scale, scale);
                }
                else {
                    targetPosition = targetTransform.position;
                    targetRotation = targetTransform.rotation;
                    targetScale = new Vector3(scale, 0.001f, scale);
                }
                yield return StartCoroutine(Transition(
                    transform,
                    labReference.position,
                    targetPosition,
                    transform.rotation,
                    targetRotation,
                    Vector3.one,
                    targetScale,
                    animationDuration));
            }

            // Just to make sure...
            UpdateTransform();

            animationRoutine = null;
            isShown = true;

            // Re-activate rooms after transition.
            SelectableRooms.ForEach(room => room.EnableInteraction(true));
            yield return null;
        }

        private IEnumerator HideRoutine()
        {
            // Disable rooms during transition to prevent accidental selection.
            SelectableRooms.ForEach(room => room.EnableInteraction(false));

            if (useAnimation) {
                yield return StartCoroutine(Transition(
                    transform,
                    transform.position,
                    labReference.position,
                    transform.rotation,
                    Quaternion.Euler(Vector3.zero),
                    transform.localScale,
                    Vector3.one,
                    animationDuration));
            }

            transform.position = labReference.position;
            transform.localScale = Vector3.one;

            animationRoutine = null;

            // Re-activate rooms after transition.
            SelectableRooms.ForEach(room => room.EnableInteraction(true));

            isShown = false;
            gameObject.SetActive(false);
            yield return null;
        }

        private IEnumerator Transition(
            Transform transform,
            Vector3 startPosition,
            Vector3 endPosition,
            Quaternion startRotation,
            Quaternion endRotation,
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
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, Mathf.SmoothStep(0f, 1f, t));
                transform.localScale = Vector3.Lerp(startScale, endScale, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
        }

        private void UpdateTransform()
        {
            if (mode == Mode.Default) {
                // Apply height offset.
                transform.position = new Vector3(transform.position.x, heightOffset, transform.position.z);

                // Apply room and wall scale.
                transform.localScale = new Vector3(scale, scale, scale);
            }
            // 2D flat mode.
            else {
                transform.position = targetTransform.position;
                transform.eulerAngles = targetTransform.eulerAngles;
                transform.localScale = new Vector3(scale, 0.001f, scale);
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
