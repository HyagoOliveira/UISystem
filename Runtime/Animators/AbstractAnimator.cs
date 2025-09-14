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
        [SerializeField, Tooltip("The Visual Element been animated.")]
        private string elementName;
        [SerializeField, Min(0f), Tooltip("The animation speed.")]
        private float speed = 1f;
        [SerializeField, Tooltip("Whether to play the animation on the Start function.")]
        private bool playOnStart = true;

        /// <summary>
        /// Whether the animation is currently playing.
        /// </summary>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// The animation current time.
        /// </summary>
        public float CurrentTime { get; protected set; }

        /// <summary>
        /// The Visual Element been animated.
        /// </summary>
        public VisualElement Element { get; protected set; }

        /// <summary>
        /// The animation speed.
        /// </summary>
        public float Speed
        {
            get => speed;
            set => speed = Mathf.Max(0f, value);
        }

        /// <summary>
        /// The timeScale-independent interval in seconds from the last frame to the current one.
        /// </summary>
        public static float DeltaTime => Time.unscaledDeltaTime;

        protected virtual void Start() => CheckPlayOnStart();

        public async Awaitable PlayAsync()
        {
            if (IsPlaying)
            {
                Stop();
                await Awaitable.NextFrameAsync();
            }

            ResetTime();
            IsPlaying = true;

            try
            {
                while (IsPlaying)
                {
                    UpdateAnimation();
                    await Awaitable.NextFrameAsync();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void Play() => _ = PlayAsync();
        public void ResetTime() => CurrentTime = 0f;

        public void Stop()
        {
            ResetTime();
            IsPlaying = false;
        }

        public abstract float GetDuration();

        public static bool HasCurveFinished(AnimationCurve curve, float currentTime)
        {
            var isLoop = curve.postWrapMode is WrapMode.Loop or WrapMode.PingPong;
            if (isLoop) return false;
            return currentTime >= GetDuration(curve);
        }

        public static float GetDuration(AnimationCurve curve) => curve.keys.Length > 0 ? curve.keys[^1].time : 0f;

        protected void UpdateCurrentTime() => CurrentTime += DeltaTime * Speed;

        protected abstract void UpdateAnimation();
        protected override void FindReferences() => Element = Find<VisualElement>(elementName);

        private void CheckPlayOnStart()
        {
            if (playOnStart) Play();
        }
    }
}