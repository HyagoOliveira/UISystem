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
        public AbstractMenu Menu { get; private set; }

        public virtual void Initialize(AbstractMenu menu) => Menu = menu;
    }
}