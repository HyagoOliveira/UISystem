using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract controller for a generic Menu Screen.
    /// <para>
    /// Menu Screens are used to display different parts of a Menu, as a sub-menu or a specific section.
    /// </para>
    /// </summary>
    public abstract class AbstractMenuScreen : AbstractController
    {
        [Header("Optional Fade Animations")]
        [SerializeField, Tooltip("Optional animation to play when screen appears.")]
        private AbstractAnimator fadeInAnimator;
        [SerializeField, Tooltip("Optional animation to play when screen disappears.")]
        private AbstractAnimator fadeOutAnimator;

        /// <summary>
        /// The current Menu for this screen.
        /// </summary>
        public AbstractMenu Menu { get; private set; }

        public virtual void Initialize(AbstractMenu menu) => Menu = menu;

        /// <summary>
        /// Fades the menu screen in (the screen content will appear).
        /// </summary>
        /// <returns></returns>
        public async Awaitable FadeInAsync()
        {
            if (fadeInAnimator) await fadeInAnimator.PlayAsync();
        }

        /// <summary>
        /// Fades the menu screen out (the screen content will disappear).
        /// </summary>
        /// <returns></returns>
        public async Awaitable FadeOutAsync()
        {
            if (fadeOutAnimator) await fadeOutAnimator.PlayAsync();
        }
    }
}