using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract animator component for a Visual Elements.
    /// <para>
    /// UI Toolkit doesn't have a proper Animation solution for now.
    /// Use any of its implementation to create Visual Elements animations.
    /// </para>
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public abstract class AbstractAnimator : AbstractController
    {
        [SerializeField] private string elementName;
        [SerializeField, Min(0f)] private float speed = 1f;
        [SerializeField] private bool playOnStart = true;

        public bool IsPlaying { get; private set; }
        public float CurrentTime { get; protected set; }

        /// <summary>
        /// The Visual Element been animated.
        /// </summary>
        public VisualElement Element { get; protected set; }

        public float Speed
        {
            get => speed;
            set => speed = Mathf.Max(0f, value);
        }

        protected virtual void Start() => CheckPlayOnStart();

        public async Awaitable PlayAsync()
        {
            ResetTime();
            IsPlaying = true;

            while (IsPlaying)
            {
                UpdateAnimation();
                await Awaitable.NextFrameAsync();
            }
        }

        public void Play() => _ = PlayAsync();
        public void ResetTime() => CurrentTime = 0f;

        public void Stop()
        {
            ResetTime();
            IsPlaying = false;
        }

        public static bool HasCurveFinished(AnimationCurve curve, float currentTime)
        {
            var isLoop = curve.postWrapMode is WrapMode.Loop or WrapMode.PingPong;
            if (isLoop || curve.keys.Length == 0) return false;

            var lastTime = curve.keys[^1].time;
            return currentTime >= lastTime;
        }

        protected abstract void UpdateAnimation();
        protected override void FindReferences() => Element = Find<VisualElement>(elementName);

        private void CheckPlayOnStart()
        {
            if (playOnStart) Play();
        }
    }
}