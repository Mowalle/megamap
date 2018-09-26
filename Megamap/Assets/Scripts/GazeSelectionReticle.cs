// This code is based on Unity's VR Sample Assets.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class GazeSelectionReticle : MonoBehaviour {

        public event Action OnSelectionComplete;

        [SerializeField] private float selectionDuration = 2f;
        [SerializeField] private bool hideOnStart = true;
        [SerializeField] private Image selectionReticle;

        private bool isSelectionRadialActive;
        private bool isRadialFilled;

        public float SelectionDuration { get { return selectionDuration; } }

        private Coroutine waitRoutine;
        private Coroutine selectionFillRoutine;

        private void OnEnable()
        {
            OnSelectionComplete += HandleOnSelectionComplete;
        }

        private void OnDisable()
        {
            OnSelectionComplete -= HandleOnSelectionComplete;
        }

        private void Start()
        {
            // Setup the radial to have no fill at the start and hide if necessary.
            selectionReticle.fillAmount = 0f;
            if (hideOnStart) {
                Hide();
            }
            else {
                Show();
            }
        }

        public void Show()
        {
            selectionReticle.gameObject.SetActive(true);
            isSelectionRadialActive = true;
        }

        public void Hide()
        {
            selectionReticle.gameObject.SetActive(false);
            isSelectionRadialActive = false;

            // This effectively resets the radial for when it's shown again.
            selectionReticle.fillAmount = 0f;
        }

        public IEnumerator WaitForSelectionRadialToFill()
        {
            // Set the radial to not filled in order to wait for it.
            isRadialFilled = false;

            // Make sure the radial is visible and usable.
            Show();

            // Check every frame if the radial is filled.
            while (!isRadialFilled) {
                yield return null;
            }

            // Once its been used make the radial invisible.
            Hide();
        }

        private IEnumerator FillSelectionRadial()
        {
            // At the start of the coroutine, the bar is not filled
            isRadialFilled = false;

            // Create a timer and reset the fill amount.
            float timer = 0f;
            selectionReticle.fillAmount = 0f;

            // This loop is executed once per frame until the timer exceeds the duration.
            while (timer < selectionDuration) {
                // The image's fill amount requires a value from 0 to 1, so we normalise the time.
                selectionReticle.fillAmount = timer / selectionDuration;

                // Increase the timer by the time between frames and wait for the next frame.
                timer += Time.deltaTime;
                yield return null;
            }

            // When the loop is finished set the fill amount to be full.
            selectionReticle.fillAmount = 1f;

            // Turn off the radial so it can only be used once.
            //isSelectionRadialActive = false;

            // The radial is now filled, so the coroutine waiting for it can continue.
            isRadialFilled = true;

            // If there is anything subscribed to OnSelectionComplete, call it.
            if (OnSelectionComplete != null) {
                OnSelectionComplete();
            }
        }

        public void StartFilling()
        {
            if (waitRoutine == null) {
                waitRoutine = StartCoroutine(WaitForSelectionRadialToFill());
            }

            // If the radial is active start filling it.
            if (isSelectionRadialActive) {
                selectionFillRoutine = StartCoroutine(FillSelectionRadial());
            }
        }

        public void StopFilling()
        {
            // If the radial is active stop filling it and reset it's amount.
            if (isSelectionRadialActive) {

                if (waitRoutine != null) {
                    StopCoroutine(waitRoutine);
                    waitRoutine = null;
                    Hide();
                }

                if (selectionFillRoutine != null) {
                    StopCoroutine(selectionFillRoutine);
                }

                selectionReticle.fillAmount = 0f;
            }
        }

        private void HandleOnSelectionComplete()
        {
            waitRoutine = null;
        }
    }

}
