using UnityEngine;

namespace Megamap {

    public class ChangeVisibility : MonoBehaviour {

        public static void SetVisible(GameObject go, bool visible)
        {
            if (go == null)
                return;

            var renderers = go.GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers) {
                r.enabled = visible;
            }
        }

        public void SetVisible(bool visible)
        {
            SetVisible(gameObject, visible);
        }

    }

}

