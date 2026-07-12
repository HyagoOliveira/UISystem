using UnityEngine;
using UnityEngine.UI;

namespace ActionCode.UISystem
{
    [DisallowMultipleComponent]
    public sealed class LayoutEnabler : MonoBehaviour
    {
        private void OnEnable() => SetEnabled(true);

        public void SetEnabled(bool enabled)
        {
            var layouts = GetComponentsInChildren<LayoutGroup>(includeInactive: true);
            foreach (var layout in layouts)
            {
                layout.enabled = enabled;
            }
        }
    }
}