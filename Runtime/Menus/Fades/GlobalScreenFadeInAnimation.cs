using ActionCode.AnimationSystem;
using ActionCode.ScreenFadeSystem;
using System.Threading;
using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Tries to get the first Screen Fader and fades in.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GlobalScreenFadeInAnimation : AbstractAnimation
    {
        protected override async Awaitable UpdateAnimationAsync(CancellationToken _)
        {
            var fader = GetGlobalFader();
            if (fader) await fader.FadeInAsync();
        }

        internal static AbstractScreenFader GetGlobalFader()
        {
            var hasFader = ScreenFadeFactory.TryGetFirst(out var fader);
            return hasFader ? fader : FindAnyObjectByType<AbstractScreenFader>();
        }
    }
}
