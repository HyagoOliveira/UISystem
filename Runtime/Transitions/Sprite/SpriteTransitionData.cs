using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Sprite Data container for Selectable UI components.
    /// </summary>
    [CreateAssetMenu(fileName = "SpriteTransitionData", menuName = "ActionCode/UI System/Sprite Transition Data", order = 110)]
    public sealed class SpriteTransitionData : ScriptableObject
    {
        [Tooltip("The normal Sprite of an object.")]
        public Sprite Normal;
        [Tooltip("The Sprite when an object is highlighted.")]
        public Sprite Highlighted;
        [Tooltip("The Sprite when an object is selected.")]
        public Sprite Selected;
        [Tooltip("The Sprite when an object is pressed.")]
        public Sprite Pressed;
        [Tooltip("The Sprite when an object is disabled.")]
        public Sprite Disabled;

        public Sprite GetSprite(SelectionState state) => state switch
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