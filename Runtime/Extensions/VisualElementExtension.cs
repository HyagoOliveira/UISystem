using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Extensions for <see cref="VisualElement"/> instances.
    /// </summary>
    public static class VisualElementExtension
    {
        /// <summary>
        /// Query for the given element by its name.<br/>
        /// Shows an error if element is not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="name">The element name to find.</param>
        /// <returns>A <see cref="VisualElement"/> or <c>null</c> if none is found.</returns>
        public static T Find<T>(this VisualElement root, string name) where T : VisualElement
        {
            var element = root.Q<T>(name);
            if (element == null) Debug.LogError($"{name} not found on {root.name}.");
            return element;
        }

        /// <summary>
        /// Sets whether the element should be displayed in the layout.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="enabled">Whether the element should be displayed in the layout.</param>
        public static void SetDisplayEnabled(this VisualElement element, bool enabled) =>
            element.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;

        /// <summary>
        /// Sets whether the element can be selected.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="enabled">Whether the element can be selected.</param>
        public static void SetSelectableEnabled(this VisualElement element, bool enabled) =>
            element.pickingMode = enabled ? PickingMode.Position : PickingMode.Ignore;
    }
}