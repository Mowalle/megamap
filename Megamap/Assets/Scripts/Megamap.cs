using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Megamap {

    public class Megamap : MonoBehaviour {

        [Range(0.01f, 1f)] public float scale = 1f;
        [Range(0.1f, 1.5f)] public float heightOffset = 0f;

        public Transform LabReference { get { return labReference; } set { labReference = value; } }
        [SerializeField] private Transform labReference = null;

        public bool placeAtPlayer = true;

        public bool UseAnimation { get { return useAnimation; } set { useAnimation = value; } }
        [SerializeField] private bool useAnimation = true;
        public float animationDuration = 1.5f;

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

            if (placeAtPlayer)
                transform.position = GetPlayerOffsetPosition();
        }

        public void Show()
        {
            if (isShown || animationRoutine != null)
                return;

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

            ApplyCondition();
        }

        private IEnumerator ShowRoutine()
        {
            // Disable rooms during transition to prevent accidental selection.
            SelectableRooms.ForEach(room => room.EnableInteraction(false));

            if (useAnimation) {
                var targetPosition = placeAtPlayer ? GetPlayerOffsetPosition() : transform.position;
                targetPosition.y = heightOffset;
                yield return StartCoroutine(Transition(
                    transform,
                    labReference.position,
                    targetPosition,
                    Vector3.one,
                    new Vector3(scale, scale, scale),
                    animationDuration));
            }

            // Just to make sure...
            ApplyCondition();

            animationRoutine = null;
            isShown = true;

            // Re-activate rooms after transition.
            SelectableRooms.ForEach(room => room.EnableInteraction(true));
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
                    new Vector3(scale, scale, scale),
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

        private void ApplyCondition()
        {
            // Apply room and wall scale.
            transform.localScale = new Vector3(scale, scale, scale);

            // Apply height offset.
            transform.position = new Vector3(transform.position.x, heightOffset, transform.position.z);
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
