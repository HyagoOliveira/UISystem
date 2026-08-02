using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Color Data container for Selectable UI components.
    /// </summary>
    [CreateAssetMenu(fileName = "ColorTransitionData", menuName = "ActionCode/UI System/Color Transition Data", order = 110)]
    public sealed class ColorTransitionData : ScriptableObject
    {
        [Tooltip("The normal Color of an object.")]
        public Color Normal;
        [Tooltip("The Color when an object is highlighted.")]
        public Color Highlighted;
        [Tooltip("The Color when an object is selected.")]
        public Color Selected;
        [Tooltip("The Color when an object is pressed.")]
        public Color Pressed;
        [Tooltip("The Color when an object is disabled.")]
        public Color Disabled;


        private void Reset() => SetColors(Color.white);

        public void SetColors(Color baseColor)
        {
            Normal = baseColor;
            Highlighted = baseColor;
            Selected = baseColor;
            Pressed = baseColor;
            Disabled = baseColor;
        }

        public Color GetColor(SelectionState state) => state switch
        {
            SelectionState.Normal => Normal,
            SelectionState.Highlighted => Highlighted,
            SelectionState.Pressed => Pressed,
            SelectionState.Selected => Selected,
            SelectionState.Disabled => Disabled,
            _ => Normal
        };
    }
}