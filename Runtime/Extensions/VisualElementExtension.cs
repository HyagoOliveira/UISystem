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
    }
}