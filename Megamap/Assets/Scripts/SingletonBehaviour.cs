using UnityEngine;

namespace Megamap {

    // Let other scripts inherit from SingletonBehaviour by making use of C#'s CRTP.
    // This makes it so the singleton code can be re-used in all scripts that should be a singleton.
    public abstract class SingletonBehaviour<TDerived> : MonoBehaviour
        where TDerived : SingletonBehaviour<TDerived> {

        private static SingletonBehaviour<TDerived> instance = null;
        public static SingletonBehaviour<TDerived> Instance { get { return instance; } }

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
