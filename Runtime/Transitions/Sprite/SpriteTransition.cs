using UnityEngine;
using UnityEngine.UI;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Transit UI component sprites based on the <see cref="SelectionState"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SpriteTransition : AbstractTransition
    {
        [Tooltip("The data used to transit the UI state.")]
        [SerializeField] private SpriteTransitionData data;
        [SerializeField, Tooltip("The target image to change its sprite.")]
        private Image target;

        private void Reset() => target = GetComponent<Image>();

        public override void Transit(SelectionState state, bool instant) =>
            target.sprite = data.GetSprite(state);
    }
}