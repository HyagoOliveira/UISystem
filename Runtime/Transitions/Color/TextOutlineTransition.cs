using TMPro;
using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Set the TextMeshPro Outline color based on the <see cref="SelectionState"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TextOutlineTransition : AbstractTransition
    {
        [Tooltip("The data used to transit the UI state.")]
        [SerializeField] private ColorTransitionData data;
        [SerializeField, Tooltip("The target graphic to change color.")]
        private TMP_Text target;
        [SerializeField, Min(0f)] private float outlineWidth = 0.2f;

        private void Reset() => target = GetComponent<TMP_Text>();

        public override void Transit(SelectionState state, bool _)
        {
            if (!Application.isPlaying) return;
            if (target == null || data == null || target.fontSharedMaterial == null) return;

            target.fontSharedMaterial.EnableKeyword(ShaderUtilities.Keyword_Outline);
            target.outlineWidth = outlineWidth;
            target.outlineColor = data.GetColor(state);
            target.UpdateMeshPadding();
        }
    }
}