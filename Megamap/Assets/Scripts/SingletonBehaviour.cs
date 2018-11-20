using UnityEngine;

namespace Megamap {

    public class SingletonBehaviour : MonoBehaviour {

        private static SingletonBehaviour instance = null;
        public static SingletonBehaviour Instance { get { return instance; } }

        protected virtual void Awake()
        {
            if (instance != null && instance != this) {
                Debug.LogWarning("SingletonBehaviour: Cannot instantiate multiple instances of this GameObject; destroying instance.");
                Destroy(gameObject);
            }
            else {
                instance = this;
            }
        }
    }

}
