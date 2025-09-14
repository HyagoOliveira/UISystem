using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Opacity animator for a Visual Elements.
    /// <para>
    /// Use the <see cref="opacityCurve"/> curve to animate the Visual Element opacity.
    /// </para>
    /// </summary>
    public sealed class OpacityAnimator : AbstractAnimator
    {
        [Space]
        [SerializeField, Tooltip("The curve driving the opacity animation.")]
        private AnimationCurve opacityCurve;

        public void SetOpacity(float opacity) => Element.style.opacity = opacity;
        public override float GetDuration() => GetDuration(opacityCurve);

        protected override void UpdateAnimation()
        {
            var opacity = opacityCurve.Evaluate(CurrentTime);

            SetOpacity(opacity);
            UpdateCurrentTime();
            CheckStopCondition();
        }

        private void CheckStopCondition()
        {
            if (HasCurveFinished(opacityCurve, CurrentTime)) Stop();
        }
    }
}