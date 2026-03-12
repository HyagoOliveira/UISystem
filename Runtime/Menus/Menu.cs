using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Controller for a Generic Game Menu.
    /// <para>
    /// Create your own menu and reference this component or implement a new menu class inheriting from here.
    /// Navigate through the Screens using the OpenScreenAsync functions.
    /// </para>
    /// </summary>
    /// <remarks>
    /// A Menu is a Finite State Machine containing several Screens, tracking the Current and Last one.<br/>
    /// Only one Screen can be activated at time. 
    /// From an opened Screen, you can go back to the last one using the Cancel input (back button).
    /// </remarks>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(AudioHandler))]
    public class Menu : MonoBehaviour, IDisposable, ICancelable
    {
        [SerializeField, Tooltip("The local Canvas Group component.")]
        private CanvasGroup canvasGroup;
        [SerializeField, Tooltip("The local UI Audio Handler.")]
        private AudioHandler audioHandler;

        [Space]
        [Tooltip("[Optional] The first screen to activated when start. Leave it empty if you wish to do it manually.")]
        public Screen firstScreen;
        [Tooltip("[Optional] The fade in/out animations to play when any Screen is opened/closed.")]
        public FadeAnimation fades;

        #region Events
        /// <summary>
        /// Event fired when the given screen is opened.
        /// </summary>
        public event Action<Screen> OnScreenOpened;

        /// <summary>
        /// Event fired when the given screen is closed.
        /// </summary>
        public event Action<Screen> OnScreenClosed;

        /// <summary>
        /// Event fired when any screen is canceled, normally pressing the back button.
        /// </summary>
        public event Action OnCanceled;
        #endregion

        #region Properties
        /// <summary>
        /// The local UI Audio Handler.
        /// </summary>
        public AudioHandler Audio => audioHandler;

        /// <summary>
        /// The last activated screen. It can be null if no screen has been navigated yet.
        /// </summary>
        public Screen LastScreen { get; private set; }

        /// <summary>
        /// The current activated screen. It can be null if no screen has been opened yet.
        /// </summary>
        public Screen CurrentScreen { get; private set; }

        /// <summary>
        /// All screens available in this menu, indexed by their identifiers.
        /// </summary>
        public Dictionary<string, Screen> Screens { get; private set; }
        #endregion

        private readonly Stack<Screen> undoHistory = new();

        protected virtual void Reset()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            audioHandler = GetComponent<AudioHandler>();
            firstScreen = GetComponentInChildren<Screen>(includeInactive: false);
        }

        protected virtual void Awake()
        {
            fades.Initialize();
            InitializeScreens();
        }

        protected virtual void OnEnable() => TryOpenFirstScreen();
        protected virtual void OnDisable() => Dispose();

        #region Open Screen
        /// <summary>
        /// Opens the <see cref="firstScreen"/> asynchronously if available.
        /// </summary>
        public void OpenFirstScreen() => _ = OpenScreenAsync(firstScreen, undoable: false);

        /// <summary>
        /// Opens the <see cref="firstScreen"/> if available.
        /// </summary>
        /// <returns>An asynchronously operation.</returns>
        public async Awaitable OpenFirstScreenAsync() => await OpenScreenAsync(firstScreen, undoable: false);

        /// <summary>
        /// Opens the given screen instance asynchronously. 
        /// </summary>
        /// <typeparam name="T">The screen type to open.</typeparam>
        /// <param name="screen">The screen instance to open.</param>
        /// <param name="undoable">Whether this screen can be closed using the back button.</param>
        /// <returns><inheritdoc cref="OpenFirstScreenAsync"/></returns>
        public async Awaitable OpenScreenAsync<T>(T screen, bool undoable = false) where T : Screen
            => await OpenScreenAsync(screen.GetIdentifier(), undoable);

        /// <summary>
        /// Opens the given screen asynchronously using the given screen identifier. 
        /// </summary>
        /// <param name="identifier">The screen identifier.</param>
        /// <param name="undoable"><inheritdoc cref="OpenScreenAsync{T}(T, bool)" path="/param[@name='undoable']"/> </param>
        /// <returns><inheritdoc cref="OpenFirstScreenAsync"/></returns>
        public async Awaitable OpenScreenAsync(string identifier, bool undoable = false)
        {
            var hasScreen = Screens.TryGetValue(identifier, out var screen);
            if (!hasScreen)
            {
                Debug.LogError($"No screen {identifier} found for menu {gameObject.name}.");
                return;
            }

            // Disable the entire menu input while opening Screen
            SetInputEnable(false);
            Audio.UnbindElements();

            if (CurrentScreen)
            {
                await CurrentScreen.fades.TryPlayFadeOutAnimation();
                await fades.TryPlayFadeOutAnimation();

                CurrentScreen.Close();
                OnScreenClosed?.Invoke(CurrentScreen);

                if (undoable) undoHistory.Push(CurrentScreen);
            }

            CloseOpenedScreens();

            LastScreen = CurrentScreen;
            CurrentScreen = screen;

            CurrentScreen.Open();
            await CurrentScreen.LoadAsync();
            await fades.TryPlayFadeInAnimation();
            await CurrentScreen.fades.TryPlayFadeInAnimation();

            // EventSystem may not be loaded yet
            await EventManager.WaitUntilEventSystemIsReadyAsync();

            // Selecting first, binding audio latter to avoid triggering events
            EventManager.TrySetSelectedGameObject(CurrentScreen.firstInput);

            Audio.BindElements(CurrentScreen.transform);
            OnScreenOpened?.Invoke(CurrentScreen);

            // Re-enable Menu input
            SetInputEnable(true);
        }

        /// <summary>
        /// Tries to open asynchronously the last screen using the undo history.
        /// </summary>
        /// <returns>Weather the last screen was opened.</returns>
        public bool TryOpenLastScreen()
        {
            var hasUndoableScreen = undoHistory.TryPop(out var screen);
            if (hasUndoableScreen) _ = OpenScreenAsync(screen, undoable: false);
            return hasUndoableScreen;
        }

        /// <summary>
        /// Sets this entire menu input.
        /// </summary>
        /// <param name="isEnabled">Whether the input is enabled.</param>
        public void SetInputEnable(bool isEnabled) => canvasGroup.blocksRaycasts = isEnabled;

        private void CloseOpenedScreens()
        {
            foreach (var screen in Screens.Values)
            {
                if (screen.IsOpenned()) screen.Close();
            }
        }
        #endregion

        #region Initialization
        protected virtual void InitializeScreens()
        {
            var screens = GetComponentsInChildren<Screen>(includeInactive: true);
            Screens = new(screens.Length);

            foreach (var screen in screens)
            {
                screen.Initialize(this);
                Screens.Add(screen.GetIdentifier(), screen);
            }
        }

        protected virtual async void TryOpenFirstScreen()
        {
            if (firstScreen == null || firstScreen.IsOpenned()) return;

            // Await one frame to let the First Screen components initialize
            await Awaitable.NextFrameAsync();
            await OpenFirstScreenAsync();
        }

        public virtual void Dispose()
        {
            Screens.Clear();
            undoHistory.Clear();
            LastScreen = null;
            CurrentScreen = null;
        }
        #endregion

        public void OnCancel(UnityEngine.EventSystems.BaseEventData _)
        {
            var wasLastScreenOpened = TryOpenLastScreen();
            if (wasLastScreenOpened) Audio.PlayCancelation();

            OnCanceled?.Invoke();
        }
    }
}