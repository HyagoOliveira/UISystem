using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Highlights any element found by <see cref="className"/> when any Pointer (like a Mouse) enters into it.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ElementHighlighter : AbstractElement<VisualElement>
    {
        [Tooltip("The class name used to find the elements.")]
        public string className = "unity-button";

        protected override string GetClassName() => className;
        protected override void RegisterEvent(VisualElement e) => e.RegisterCallback<PointerEnterEvent>(HandlePointerEnterEvent);
        protected override void UnregisterEvent(VisualElement e) => e.UnregisterCallback<PointerEnterEvent>(HandlePointerEnterEvent);

        private void HandlePointerEnterEvent(PointerEnterEvent evt)
        {
            var target = evt.target as Focusable;
            target?.Focus();
        }
    }
}