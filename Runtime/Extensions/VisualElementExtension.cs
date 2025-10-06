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

        /// <summary>
        /// Whether the element is currently focused.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>True of false, whether the element is currently focused.</returns>
        public static bool IsFocused(this VisualElement element) =>
            element.panel?.focusController?.focusedElement == element;

        /// <summary>
        /// In Unity's UI Toolkit, a disabled button cannot gain focus.
        /// To achieve a similar effect, Style Class manipulation is necessary.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="isFocused">Whether the button should be focused.</param>
        public static void FakeDisabledFocus(this Button button, bool isFocused)
        {
            // Class .unity-button:focus:disabled does not work
            const string unityButtonClass = "unity-button";
            const string buttonDisabledFocus = "button-disabled-focus";

            button.EnableInClassList(unityButtonClass, !isFocused);
            if (isFocused) button.AddToClassList(buttonDisabledFocus);
            else button.RemoveFromClassList(buttonDisabledFocus);
        }

        /// <summary>
        /// Gets the given element background color.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>A Color.</returns>
        public static Color GetBackgroundColor(this VisualElement element) => element.resolvedStyle.backgroundColor;
    }
}