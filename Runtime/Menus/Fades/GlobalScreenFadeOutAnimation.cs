using ActionCode.AnimationSystem;
using System.Threading;
using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Tries to get the first Screen Fader and fades out.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GlobalScreenFadeOutAnimation : AbstractAnimation
    {
        protected override async Awaitable UpdateAnimationAsync(CancellationToken _) => await TryFadeOutAsync();

        public static async Awaitable TryFadeOutAsync()
        {
            var fader = GlobalScreenFadeInAnimation.GetGlobalFader();
            if (fader) await fader.FadeOutAsync();
        }
    }
}
