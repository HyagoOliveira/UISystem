using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Screens are used to display different parts of a Menu, as a 
    /// sub-menu Screen, a Tab Screen or other specific menu section.
    /// Use this component as a base class and implement your own Screens.
    /// </summary>
    /// <remarks>
    /// Screens contains one or multiple elements that can be selected, submitted (clicked) or canceled. 
    /// A local or parented <see cref="AudioHandler"/> component will play the corresponding audio from the <see cref="MenuData"/>.
    /// </remarks>
    [DisallowMultipleComponent]
    public class BaseScreen : MonoBehaviour
    {
        [Tooltip("[Optional] The first input to be selected when this Screen is opened.")]
        public GameObject firstInput;
        [Tooltip("[Optional] The fade in/out animations to play when this Screen is opened/closed.")]
        public FadeAnimation fades;

        /// <summary>
        /// The current Menu for this screen.
        /// </summary>
        public Menu Menu { get; private set; }

        public bool IsOpening { get; private set; }
        public bool IsClosing { get; private set; }

        public virtual void Initialize(Menu menu)
        {
            Menu = menu;
            fades.Initialize();
        }

        protected virtual void OnEnable() => SubscribeEvents();
        protected virtual void OnDisable() => UnsubscribeEvents();

        public bool IsOpened() => gameObject.activeSelf;
        public bool IsClosed() => !IsOpened();

        public string GetIdentifier() => gameObject.name;

        /// <summary>
        /// Opens the screen using the given identifier.
        /// </summary>
        /// <param name="identifier"><inheritdoc cref="Menu.OpenScreenAsync(string, bool)" path="/param[@name='identifier']"/></param>
        public void OpenScreen(string identifier) => _ = Menu.OpenScreenAsync(identifier, undoable: false);

        /// <summary>
        /// Opens a closeable screen using the given identifier.
        /// </summary>
        /// <remarks>
        /// Press the cancel button to go back to the last screen.
        /// </remarks>
        /// <param name="identifier"><inheritdoc cref="Menu.OpenScreenAsync(string, bool)" path="/param[@name='identifier']"/></param>
        public void OpenCloseableScreen(string identifier) => _ = Menu.OpenScreenAsync(identifier, undoable: true);

        /// <summary>
        /// Executed when stating to open this screen, before any fade in animations start to play.
        /// </summary>
        public virtual void StartOpen()
        {
            IsOpening = true;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Executed when finishing to open this screen, after all fade is animations are played 
        /// and the Player has the input.
        /// </summary>
        public virtual void FinishOpen() => IsOpening = false;

        /// <summary>
        /// Executed when starting to close this screen, before any fade out animations start to play.
        /// </summary>
        public virtual void StartClose() => IsClosing = true;

        /// <summary>
        /// Executed when finishing to close this screen, after all fade out animations are played.
        /// </summary>
        public virtual void FinishClose()
        {
            IsClosing = false;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Loads any content from this screen asynchronously.
        /// </summary>
        /// <remarks>
        /// For example, implement this function to load a 
        /// Character model inside a Character Screen.
        /// </remarks>
        /// <returns>An asynchronous operation.</returns>
        public virtual async Awaitable LoadAsync() => await Awaitable.NextFrameAsync();

        protected virtual void SubscribeEvents() { }
        protected virtual void UnsubscribeEvents() { }
    }
}