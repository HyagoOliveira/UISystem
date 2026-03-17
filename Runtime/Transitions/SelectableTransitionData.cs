using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Color Data container for Selectable UI components.
    /// </summary>
    [CreateAssetMenu(fileName = "SelectableTransitionData", menuName = "ActionCode/UI System/Selectable Transition Data", order = 110)]
    public sealed class SelectableTransitionData : ScriptableObject
    {
        [Header("Colors")]
        [Tooltip("The normal Color of an object.")]
        public Color NormalColor;
        [Tooltip("The Color when an object is highlighted.")]
        public Color HighlightedColor;
        [Tooltip("The Color when an object is selected.")]
        public Color SelectedColor;
        [Tooltip("The Color when an object is pressed.")]
        public Color PressedColor;
        [Tooltip("The Color when an object is disabled.")]
        public Color DisabledColor;

        private void Reset() => SetColors(Color.white);

        public void SetColors(Color baseColor)
        {
            NormalColor = baseColor;
            HighlightedColor = baseColor;
            SelectedColor = baseColor;
            PressedColor = baseColor;
            DisabledColor = baseColor;
        }

        public Color GetColor(SelectionState state) => state switch
        {
            SelectionState.Normal => NormalColor,
            SelectionState.Highlighted => HighlightedColor,
            SelectionState.Pressed => PressedColor,
            SelectionState.Selected => SelectedColor,
            SelectionState.Disabled => DisabledColor,
            _ => NormalColor
        };
    }
}