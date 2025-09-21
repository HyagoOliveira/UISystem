using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Highlights any element found by the class names when any Pointer (like a Mouse) enters into it.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ElementHighlighter : AbstractElement<VisualElement>
    {
        protected override string[] GetQueryClasses() => new[] { "unity-button", "unity-base-slider" };
        protected override void RegisterEvent(VisualElement e) => e.RegisterCallback<PointerEnterEvent>(HandlePointerEnterEvent);
        protected override void UnregisterEvent(VisualElement e) => e.UnregisterCallback<PointerEnterEvent>(HandlePointerEnterEvent);

        private void HandlePointerEnterEvent(PointerEnterEvent evt)
        {
            var target = evt.target as Focusable;
            target?.Focus();
        }
    }
}