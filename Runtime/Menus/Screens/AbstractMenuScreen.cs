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
        [Header("Fades")]
        [Tooltip("Whether to apply the fade in animation (screen appears).")]
        public bool applyFadeIn;
        [Tooltip("Whether to apply the fade out animation (screen disappears).")]
        public bool applyFadeOut;

        /// <summary>
        /// The current Menu for this screen.
        /// </summary>
        public MenuController Menu { get; private set; }

        /// <summary>
        /// Initializes this screen with the given Menu.
        /// </summary>
        /// <param name="menu">The menu this screen belongs to.</param>
        public virtual void Initialize(MenuController menu) => Menu = menu;
    }
}