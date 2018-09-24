using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Megamap {

    public class GazeTarget : MonoBehaviour {

        public event Action OnOver;
        public event Action OnOut;
        public event Action OnSelection;

        protected bool isOver_;

        public bool IsOver
        {
            get { return isOver_; }
        }

        public void Over()
        {
            isOver_ = true;

            if (OnOver != null) {
                OnOver();
            }
        }

        public void Out()
        {
            isOver_ = false;

            if (OnOut != null) {
                OnOut();
            }
        }

        public void Select()
        {
            if (OnSelection != null) {
                OnSelection();
            }
        }
    }

}
