using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Scale animator for a Visual Elements.
    /// <para>
    /// Use the <see cref="scaleCurve"/> curve to animate the Visual Element Transform Scale.
    /// </para>
    /// </summary>
    public sealed class ScaleAnimator : AbstractAnimator
    {
        [Space]
        [SerializeField, Tooltip("The curve driving the scale animation.")]
        private AnimationCurve scaleCurve;

        public void SetScale(float scale) => Element.transform.scale = Vector3.one * scale;
        public override float GetDuration() => GetDuration(scaleCurve);

        protected override void UpdateAnimation()
        {
            CurrentTime += Time.deltaTime * Speed;
            SetScale(scaleCurve.Evaluate(CurrentTime));
            CheckStopCondition();
        }

        private void CheckStopCondition()
        {
            if (HasCurveFinished(scaleCurve, CurrentTime)) Stop();
        }
    }
}