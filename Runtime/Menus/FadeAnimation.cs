using PrimeTween;
using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Data container to hold the fades in and fades out animations.
    /// </summary>
    [System.Serializable]
    public struct FadeAnimation
    {
        [field: SerializeField, Tooltip("The fade in (show) animation to play.")]
        public AbstractAnimation In { get; private set; }
        [field: SerializeField, Tooltip("The fade out (hide) animation to play.")]
        public AbstractAnimation Out { get; private set; }

        public readonly void Initialize()
        {
            if (In) In.enabled = false;
            if (Out) Out.enabled = false;
        }

        /// <summary>
        /// Tries to play the fades in (show) animation.
        /// </summary>
        /// <returns>An asynchronous operation.</returns>
        public async readonly Awaitable TryPlayFadeInAnimation()
        {
            if (In) await In.PlayAsync();
        }

        /// <summary>
        /// Tries to play the fades out (hide) animation.
        /// </summary>
        /// <returns>An asynchronous operation.</returns>
        public async readonly Awaitable TryPlayFadeOutAnimation()
        {
            if (Out) await Out.PlayAsync();
        }
    }
}