using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ActionCode.ScreenFadeSystem;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Controller for UI Toolkit Menu.
    /// <para>
    /// A Menu is a Finite State Machine containing several Screens, 
    /// keeping the data about the Current and Last Screen.<br/>
    /// Only one Screen can be activated at time, navigating between then.<br/>
    /// From an activated Screen, you can go back to the last one using the Cancel input (back button).
    /// </para>
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(ElementHighlighter))]
    [RequireComponent(typeof(ButtonClickAudioPlayer))]
    [RequireComponent(typeof(ElementFocusAudioPlayer))]
    public sealed class MenuController : MonoBehaviour
    {
        [SerializeField, Tooltip("The local Audio Source for this menu.")]
        private AudioSource audioSource;
        [SerializeField, Tooltip("The local Highlighter for this menu.")]
        private ElementHighlighter highlighter;
        [SerializeField, Tooltip("The local Focus Player for this menu.")]
        private ElementFocusAudioPlayer focusPlayer;
        [SerializeField, Tooltip("The local Button Click Player for this menu.")]
        private ButtonClickAudioPlayer buttonClickPlayer;

        [Space]
        [SerializeField, Tooltip("[Optional] The first screen to activated when start. Leave it empty if you wish to do it manually.")]
        private AbstractMenuScreen firstScreen;
        [SerializeField, Tooltip("[Optional] Screen Fader Prefab to use when transitioning between screens.")]
        private AbstractScreenFader faderPrefab;

        /// <summary>
        /// Event fired when the given screen is opened.
        /// </summary>
        public event Action<AbstractMenuScreen> OnScreenOpened;

        /// <summary>
        /// Event fired when the given screen is canceled: the back button is pressed.
        /// </summary>
        public event Action<AbstractMenuScreen> OnScreenCanceled;

        public AudioSource AudioSource => audioSource;
        public ElementHighlighter Highlighter => highlighter;
        public ElementFocusAudioPlayer FocusPlayer => focusPlayer;
        public ButtonClickAudioPlayer ButtonClickPlayer => buttonClickPlayer;

        /// <summary>
        /// The current Screen Fader used when transitioning between screens.
        /// <para>This can be null if <see cref="faderPrefab"/> is not set or it is not available.</para>
        /// </summary>
        public AbstractScreenFader Fader { get; private set; }
        public AbstractMenuScreen FirstScreen => firstScreen;

        /// <summary>
        /// The las screen activated. It can be null.
        /// </summary>
        public AbstractMenuScreen LastScreen { get; private set; }

        /// <summary>
        /// The current activated screen. It can be null.
        /// </summary>
        public AbstractMenuScreen CurrentScreen { get; private set; }

        /// <summary>
        /// All screens available in this menu, indexed by their type.
        /// </summary>
        public Dictionary<Type, AbstractMenuScreen> Screens { get; private set; }

        private readonly Stack<AbstractMenuScreen> undoHistory = new();

        private void Reset()
        {
            audioSource = GetComponent<AudioSource>();
            highlighter = GetComponent<ElementHighlighter>();
            focusPlayer = GetComponent<ElementFocusAudioPlayer>();
            buttonClickPlayer = GetComponent<ButtonClickAudioPlayer>();
            firstScreen = GetComponentInChildren<AbstractMenuScreen>(includeInactive: false);
        }

        private void Awake()
        {
            TryFindFader();
            InitializeScreens();
        }

        private void OnEnable()
        {
            SubscribeEvents();
            TryActivateFirstScreen();
        }

        private void OnDisable() => UnsubscribeEvents();

        /// <summary>
        /// Enables or disables the sending of navigation events globally.
        /// </summary>
        /// <param name="enabled">
        /// Should the EventSystem allow navigation events (move/submit/cancel).
        /// </param>
        public static void SetSendNavigationEvents(bool enabled)
        {
            var eventSystem = UnityEngine.EventSystems.EventSystem.current;
            if (eventSystem) eventSystem.sendNavigationEvents = enabled;
        }

        /// <summary>
        /// Whether this menu has the given screen type available.
        /// </summary>
        /// <typeparam name="T">The menu type.</typeparam>
        /// <returns>True if this menu has the given screen type available.</returns>
        public bool HasScreen<T>() where T : AbstractMenuScreen => Screens.ContainsKey(typeof(T));

        /// <summary>
        /// Opens the <see cref="FirstScreen"/> if available."/>
        /// </summary>
        public void OpenFirstScreen() => _ = OpenScreenAsync(firstScreen, undoable: false);

        /// <summary>
        /// Opens the given screen asynchronously if available. 
        /// Checks whether the menu is available before trying to open it.
        /// </summary>
        /// <param name="name">The name of the menu. Use its type.</param>
        /// <param name="undoable">Whether this screen can be closed using the back button.</param>
        /// <returns>An asynchronously operation.</returns>
        public async Awaitable OpenScreenAsync(string name, bool undoable = true)
        {
            var hasScreen = TryGetMenuScreen(name, out AbstractMenuScreen screen);
            if (hasScreen) await OpenScreenAsync(screen, undoable);
            else Debug.LogError($"Screen {name} is not available.");
        }

        /// <summary>
        /// Opens the generic screen asynchronously if available. 
        /// Checks whether the menu is available before trying to open it.
        /// </summary>
        /// <typeparam name="T">The menu type to open.</typeparam>
        /// <param name="undoable">Whether this screen can be closed using the back button.</param>
        /// <returns>An asynchronously operation.</returns>
        public async Awaitable OpenScreenAsync<T>(bool undoable = true) where T : AbstractMenuScreen
        {
            var type = typeof(T);
            var hasScreen = Screens.TryGetValue(type, out AbstractMenuScreen screen);
            if (hasScreen) await OpenScreenAsync(screen, undoable);
            else Debug.LogError($"Screen {type} is not available.");
        }

        /// <summary>
        /// Opens the given screen asynchronously.
        /// </summary>
        /// <param name="screen">The screen to open.</param>
        /// <param name="undoable">Whether this screen can be closed using the back button.</param>
        /// <returns>An asynchronously operation.</returns>
        public async Awaitable OpenScreenAsync(AbstractMenuScreen screen, bool undoable = true)
        {
            SetSendNavigationEvents(false);

            var hasCurrentScreen = CurrentScreen && CurrentScreen.IsValid();
            if (hasCurrentScreen) await DisposeCurrentScreenAsync();

            LastScreen = CurrentScreen;

            var applyTransition = CurrentScreen && CurrentScreen.IsEnabled;
            if (applyTransition)
            {
                if (Fader && screen.applyFadeOut) await Fader.FadeOutAsync();
                DeactivateAllScreens();
            }

            if (undoable)
            {
                var hasLastController = LastScreen != null;
                if (hasLastController) undoHistory.Push(LastScreen);
            }

            if (screen == null) return;

            if (Fader && screen.applyFadeIn) await Fader.FadeInAsync();

            CurrentScreen = screen;
            CurrentScreen.Activate();
            CurrentScreen.SetVisibility(true);

            await InitializeCurrentScreenAsync();

            SetSendNavigationEvents(true);
            OnScreenOpened?.Invoke(CurrentScreen);
        }

        /// <summary>
        /// Closes the Current Screen if available.
        /// </summary>
        public async void CloseCurrrentScreen()
        {
            if (CurrentScreen == null) return;

            await DisposeCurrentScreenAsync();
            CurrentScreen.Deactivate();

            CurrentScreen = null;
        }

        public bool TryOpenLastScreen(out AbstractMenuScreen screen)
        {
            var hasUndoableScreen = undoHistory.TryPop(out screen);
            if (hasUndoableScreen) _ = OpenScreenAsync(screen, undoable: false);
            return hasUndoableScreen;
        }

        private bool TryGetMenuScreen(string name, out AbstractMenuScreen screen)
        {
            screen = null;
            name = name.ToLower().Trim();

            foreach (var (screenType, screenInstance) in Screens)
            {
                var screenName = screenType.ToString().ToLower().Trim();
                var isScreen = screenName.Contains(name);
                if (isScreen)
                {
                    screen = screenInstance;
                    return true;
                }
            }

            return false;
        }

        private void SubscribeEvents()
        {
            AbstractPopup.OnAnyStartShow += HandleAnyPopupStartShow;
            AbstractPopup.OnAnyFinishClose += HandleAnyPopupFinishClose;
        }

        private void UnsubscribeEvents()
        {
            AbstractPopup.OnAnyStartShow -= HandleAnyPopupStartShow;
            AbstractPopup.OnAnyFinishClose -= HandleAnyPopupFinishClose;
        }

        private void InitializeScreens()
        {
            var screens = GetComponentsInChildren<AbstractMenuScreen>(includeInactive: true);
            Screens = new(screens.Length);

            foreach (var screen in screens)
            {
                screen.Initialize(this);
                Screens.Add(screen.GetType(), screen);
            }
        }

        private void InitializeElements()
        {
            Highlighter.Initialize(CurrentScreen.Root);
            FocusPlayer.Initialize(CurrentScreen.Root);
            ButtonClickPlayer.Initialize(CurrentScreen.Root);
        }

        private void DisposeElements()
        {
            Highlighter.Dispose();
            FocusPlayer.Dispose();
            ButtonClickPlayer.Dispose();
        }

        private void OnCancel()
        {
            if (!TryOpenLastScreen(out AbstractMenuScreen screen)) return;

            ButtonClickPlayer.PlayCancelSound();
            OnScreenCanceled?.Invoke(screen);
        }

        private void TryFindFader()
        {
            if (faderPrefab == null) return;
            Fader = ScreenFadeFactory.Create(faderPrefab);
        }

        private void DeactivateAllScreens()
        {
            foreach (var screen in Screens.Values)
            {
                screen.Deactivate();
            }
        }

        private async void TryActivateFirstScreen()
        {
            if (firstScreen == null) return;

            // Await one frame to let the First Screen components initialize
            await Awaitable.NextFrameAsync();
            OpenFirstScreen();
        }

        private async Awaitable InitializeCurrentScreenAsync()
        {
            CurrentScreen.Root.RegisterCallback<NavigationCancelEvent>(HandleNavigationCancelEvent);

            await Awaitable.NextFrameAsync();
            await CurrentScreen.FocusAsync();

            InitializeElements();
        }

        private async Awaitable DisposeCurrentScreenAsync()
        {
            CurrentScreen.Root.UnregisterCallback<NavigationCancelEvent>(HandleNavigationCancelEvent);
            // Wait so menu elements can execute their final actions.
            await Awaitable.NextFrameAsync();
            DisposeElements();
        }

        private void HandleNavigationCancelEvent(NavigationCancelEvent _) => OnCancel();

        private async void HandleAnyPopupStartShow(AbstractPopup _)
        {
            if (CurrentScreen == null) return;

            await DisposeCurrentScreenAsync();
            CurrentScreen.SetEnabled(false);
        }

        private async void HandleAnyPopupFinishClose(AbstractPopup _)
        {
            if (CurrentScreen == null) return;

            CurrentScreen.SetEnabled(true);

            await CurrentScreen.FocusAsync();
            await InitializeCurrentScreenAsync();
        }
    }
}