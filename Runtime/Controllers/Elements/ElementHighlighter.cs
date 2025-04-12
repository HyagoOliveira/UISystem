using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Highlights any element found by <see cref="className"/> when any Pointer (like a Mouse) enters into it.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class ElementHighlighter : AbstractController
    {
        [Tooltip("The class name used to find the elements.")]
        public string className = "unity-button";

        private UQueryBuilder<VisualElement> elements;

        protected override void FindReferences()
        {
            base.FindReferences();
            elements = Root.Query<VisualElement>(className: className);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            elements.ForEach(e => e.RegisterCallback<PointerEnterEvent>(HandlePointerEnterEvent));
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            elements.ForEach(e => e.UnregisterCallback<PointerEnterEvent>(HandlePointerEnterEvent));
        }

        private void HandlePointerEnterEvent(PointerEnterEvent evt)
        {
            var target = evt.target as Focusable;
            target?.Focus();
        }
    }
}
