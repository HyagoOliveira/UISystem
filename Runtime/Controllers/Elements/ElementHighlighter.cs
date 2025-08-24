using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Highlights any element found by <see cref="className"/> when any Pointer (like a Mouse) enters into it.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ElementHighlighter : MonoBehaviour, IDisposable
    {
        [Tooltip("The class name used to find the elements.")]
        public string className = "unity-button";

        private UQueryBuilder<VisualElement> elements;

        public void Initialize(VisualElement root)
        {
            elements = root.Query<VisualElement>(className: className);
            elements.ForEach(e => e.RegisterCallback<PointerEnterEvent>(HandlePointerEnterEvent));
        }

        public void Dispose() => elements.ForEach(e => e.UnregisterCallback<PointerEnterEvent>(HandlePointerEnterEvent));

        private void HandlePointerEnterEvent(PointerEnterEvent evt)
        {
            var target = evt.target as Focusable;
            target?.Focus();
        }
    }
}