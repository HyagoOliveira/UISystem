using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Selects the first element found by <see cref="elementName"/>.
    /// Use it to select the first option for a menu.
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1)]
    public sealed class ElementFirstSelector : AbstractController
    {
        [Tooltip("The name used to find the element.")]
        public string elementName;

        public VisualElement Element { get; private set; }

        protected override void FindReferences()
        {
            base.FindReferences();
            Element = Root.Q(elementName);
            Element?.Focus();
        }
    }
}
