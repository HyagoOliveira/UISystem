using UnityEngine;
using System.Threading;
using ActionCode.AnimationSystem;
using ActionCode.ScreenFadeSystem;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Tries to get the first Screen Fader and fades in.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GlobalScreenFadeInAnimation : AbstractAnimation
    {
        protected override async Awaitable PlayAsync(CancellationToken token)
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
