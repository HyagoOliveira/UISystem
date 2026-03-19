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
    /// From an opened Screen, you can go back to the last one using the Cancel input (back button).<br/><br/>
    /// 
    /// Each Screen contains one or multiple elements that can be selected, submitted (clicked) or canceled. 
    /// The local <see cref="AudioHandler"/> component will play the corresponding audio from the <see cref="MenuData"/>.
    /// </remarks>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(AudioHandler))]
    [RequireComponent(typeof(UnityEngine.UI.GraphicRaycaster))] // Necessary to detect mouse inputs
    public class Menu : MonoBehaviour, IDisposable, ICancelable
    {
        [SerializeField, Tooltip("The local Canvas Group component.")]
        private CanvasGroup canvasGroup;
        [SerializeField, Tooltip("The local UI Audio Handler.")]
        private AudioHandler audioHandler;

        [Space]
        [Tooltip("[Optional] The first screen to activated when start. Leave it empty if you wish to do it manually.")]
        public BaseScreen firstScreen;
        [Tooltip("[Optional] The menu global fade in/out animations to play when any Screen is opened/closed.")]
        public FadeAnimation globalFades;

        #region Events
        /// <summary>
        /// Event fired when the given screen is opened.
        /// </summary>
        public event Action<BaseScreen> OnScreenOpened;

        /// <summary>
        /// Event fired when the given screen is closed.
        /// </summary>
        public event Action<BaseScreen> OnScreenClosed;

        /// <summary>
        /// Event fired when any screen is canceled, normally pressing the back button.
        /// </summary>
        public event Action OnCanceled;
        #endregion

        #region Properties
        /// <summary>
        /// Whether this Menu is active.
        /// </summary>
        public bool IsActive => gameObject.activeInHierarchy;

        /// <summary>
        /// Whether this Menu is opening any screen.
        /// </summary>
        public bool IsOpening { get; private set; }

        /// <summary>
        /// Whether this Menu is fully opened with a current screen.
        /// </summary>
        public bool IsOpened { get; private set; }

        /// <summary>
        /// The local UI Audio Handler.
        /// </summary>
        public AudioHandler Audio => audioHandler;

        /// <summary>
        /// The last activated screen. It can be null if no screen has been navigated yet.
        /// </summary>
        public BaseScreen LastScreen { get; private set; }

        /// <summary>
        /// The current activated screen. It can be null if no screen has been opened yet.
        /// </summary>
        public BaseScreen CurrentScreen { get; private set; }

        /// <summary>
        /// All screens available in this menu, indexed by their identifiers.
        /// </summary>
        public Dictionary<string, BaseScreen> Screens { get; private set; }
        #endregion

        private readonly Stack<BaseScreen> undoHistory = new();

        protected virtual void Reset()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            audioHandler = GetComponent<AudioHandler>();
            firstScreen = GetComponentInChildren<BaseScreen>(includeInactive: false);
        }

        protected virtual void Awake() => globalFades.Initialize();

        protected virtual void OnEnable()
        {
            InitializeScreens();
            TryOpenFirstScreen();
        }

        protected virtual void OnDisable() => Dispose();

        #region Activation
        public void Activate() => SetActive(true);
        public void Deactivate() => SetActive(false);

        /// <summary>
        /// Activates or deactivates this Menu, according to the give param.
        /// </summary>
        /// <param name="isActivated">Whether to activate this menu.</param>
        public void SetActive(bool isActivated) => gameObject.SetActive(isActivated);
        #endregion

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
        public async Awaitable OpenScreenAsync<T>(T screen, bool undoable = false) where T : BaseScreen
            => await OpenScreenAsync(screen.GetIdentifier(), undoable);

        /// <summary>
        /// Opens the given screen asynchronously using the given screen identifier. 
        /// </summary>
        /// <param name="identifier">The screen identifier.</param>
        /// <param name="undoable"><inheritdoc cref="OpenScreenAsync{T}(T, bool)" path="/param[@name='undoable']"/></param>
        /// <returns><inheritdoc cref="OpenFirstScreenAsync"/></returns>
        public virtual async Awaitable OpenScreenAsync(string identifier, bool undoable = false)
        {
            SetOpening(true);
            if (!IsActive) Activate();

            var hasScreen = Screens.TryGetValue(identifier, out var screen);
            if (!hasScreen)
            {
                Debug.LogError($"No screen {identifier} found for menu {gameObject.name}.");
                SetOpening(false);
                return;
            }

            // Disable the entire menu input while opening Screen
            SetInputEnable(false);
            Audio.UnbindElements();

            if (CurrentScreen)
            {
                CurrentScreen.StartClose();
                await CurrentScreen.fades.TryPlayFadeOutAnimation();
                await globalFades.TryPlayFadeOutAnimation();

                CurrentScreen.FinishClose();
                OnScreenClosed?.Invoke(CurrentScreen);

                if (undoable) undoHistory.Push(CurrentScreen);
            }

            CloseOpenedScreens();

            LastScreen = CurrentScreen;
            CurrentScreen = screen;

            CurrentScreen.StartOpen();
            await CurrentScreen.LoadAsync();
            await globalFades.TryPlayFadeInAnimation();
            await CurrentScreen.fades.TryPlayFadeInAnimation();

            // EventSystem may not be loaded yet
            await EventManager.WaitUntilEventSystemIsReadyAsync();

            // Selecting first, binding audio latter to avoid triggering events
            EventManager.TrySetSelectedGameObject(CurrentScreen.firstInput);

            Audio.BindElements(CurrentScreen.transform);
            OnScreenOpened?.Invoke(CurrentScreen);

            // Re-enable Menu input
            SetInputEnable(true);

            CurrentScreen.FinishOpen();
            SetOpening(false);
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
        public void SetInputEnable(bool isEnabled)
        {
            canvasGroup.blocksRaycasts = isEnabled;
            EventManager.TrySendNavigationEvents(isEnabled);
        }

        private void SetOpening(bool isOpening)
        {
            IsOpened = !isOpening;
            IsOpening = isOpening;
        }

        private void CloseOpenedScreens()
        {
            foreach (var screen in Screens.Values)
            {
                if (screen.IsOpened()) screen.FinishClose();
            }
        }
        #endregion

        #region Initialization
        protected virtual void InitializeScreens()
        {
            var screens = GetComponentsInChildren<BaseScreen>(includeInactive: true);
            Screens = new(screens.Length);

            foreach (var screen in screens)
            {
                screen.Initialize(this);
                Screens.Add(screen.GetIdentifier(), screen);
            }
        }

        private async void TryOpenFirstScreen()
        {
            if (firstScreen == null) return;

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
            EventManager.TrySetSelectedGameObject(null);
        }
        #endregion

        public void OnCancel(UnityEngine.EventSystems.BaseEventData _)
        {
            var wasLastScreenOpened = TryOpenLastScreen();
            if (wasLastScreenOpened) Audio.PlayCancellation();

            OnCanceled?.Invoke();
        }
    }
}