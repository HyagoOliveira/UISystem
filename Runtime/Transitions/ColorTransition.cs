using UnityEngine;
using UnityEngine.UI;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Transit UI component colors based on the <see cref="SelectionState"/>.
    /// </summary>
    /// <remarks>Use it to fade backgrounds colors.</remarks>
    [DisallowMultipleComponent]
    public sealed class ColorTransition : AbstractTransition
    {
        [SerializeField, Tooltip("The target graphic to change color.")]
        private Graphic target;
        [SerializeField, Min(0), Tooltip("The color change fade duration.")]
        private float fadeDuration = 0.1f;

        private void Reset() => target = GetComponent<Graphic>();

        public override void Transit(SelectionState state, bool instant)
        {
            if (target == null || data == null) return;

            var duration = instant ? 0f : fadeDuration;
            var color = data.GetColor(state);

            target.CrossFadeColor(
                color,
                duration,
                ignoreTimeScale: true,
                useAlpha: true
            );
        }
    }
}