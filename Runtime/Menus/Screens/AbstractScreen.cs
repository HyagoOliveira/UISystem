using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract UI Screen.
    /// </summary>
    /// <remarks>
    /// UI Screens are used to display different parts of a Menu, 
    /// as a sub-menu Screen, a Tab Screen or other specific menu section.
    /// </remarks>
	public abstract class AbstractScreen : MonoBehaviour
    {
        /// <summary>
        /// The current Menu for this screen.
        /// </summary>
        public Menu Menu { get; private set; }

        public virtual void Initialize(Menu menu) => Menu = menu;

        protected virtual void OnEnable() => SubscribeEvents();
        protected virtual void OnDisable() => UnsubscribeEvents();

        public virtual void Open() => gameObject.SetActive(true);
        public virtual void Close() => gameObject.SetActive(false);

        protected virtual void SubscribeEvents() { }
        protected virtual void UnsubscribeEvents() { }
    }
}