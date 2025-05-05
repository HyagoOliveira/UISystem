using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Opacity animator for Visual Elements.
    /// Use the <see cref="animation"/> curve to animate the opacity.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class OpacityAnimator : AbstractAnimator
    {
        [SerializeField] private string elementName;
#pragma warning disable UNT0013 // Remove invalid SerializeField attribute
        [SerializeField] public AnimationCurve animation;
#pragma warning restore UNT0013 // Remove invalid SerializeField attribute

        /// <summary>
        /// The Visual Element been animated.
        /// </summary>
        public VisualElement Element { get; private set; }

        public void SetOpacity(float opacity) => Element.style.opacity = opacity;

        protected override void FindReferences() => Element = Find<VisualElement>(elementName);

        protected override void UpdateAnimation()
        {
            var opacity = animation.Evaluate(Time.timeSinceLevelLoad);
            SetOpacity(opacity);
        }
    }
}