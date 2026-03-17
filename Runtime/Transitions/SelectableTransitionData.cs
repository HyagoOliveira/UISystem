using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Color Data container for Selectable UI components.
    /// </summary>
    [CreateAssetMenu(fileName = "SelectableTransitionData", menuName = "ActionCode/UI System/Selectable Transition Data", order = 110)]
    public sealed class SelectableTransitionData : ScriptableObject
    {
        [field: SerializeField, Tooltip("The normal Color of an object.")]
        public Color Normal { get; set; }
        [field: SerializeField, Tooltip("The Color when an object is highlighted.")]
        public Color Highlighted { get; set; }
        [field: SerializeField, Tooltip("The Color when an object is selected.")]
        public Color Selected { get; set; }
        [field: SerializeField, Tooltip("The Color when an object is pressed.")]
        public Color Pressed { get; set; }
        [field: SerializeField, Tooltip("The Color when an object is disabled.")]
        public Color Disabled { get; set; }

        private void Reset() => NormalizeColors(Color.white * 0.8f);

        public void NormalizeColors(Color baseColor)
        {
            Normal = baseColor;
            Highlighted = baseColor * 1.1f;
            Selected = baseColor * 1.2f;
            Pressed = baseColor * 1.3f;
            Disabled = baseColor * 0.2F;
        }
    }
}