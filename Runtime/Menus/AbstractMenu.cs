using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ActionCode.ScreenFadeSystem;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract controller for generic Menu.
    /// <para>
    /// A Menu is a Finite State Machine containing several Screens, 
    /// keeping the data about the Current and Last.<br/>
    /// Only one Screen can be activated at time, navigating between then.<br/>
    /// From an activated Screen, you can go back to the last one using the <see cref="cancel"/> input.
    /// </para>
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(ElementHighlighter))]
    [RequireComponent(typeof(ButtonClickAudioPlayer))]
    [RequireComponent(typeof(ElementFocusAudioPlayer))]
    public abstract class AbstractMenu : MonoBehaviour
    {
        [SerializeField, Tooltip("The local Highlighter for this menu.")]
        private ElementHighlighter highlighter;
        [SerializeField, Tooltip("The local Focus Player for this menu.")]
        private ElementFocusAudioPlayer focusPlayer;
        [SerializeField, Tooltip("The local Button Click Player for this menu.")]
        private ButtonClickAudioPlayer buttonClickPlayer;

        [Header("Screen Transition")]
        [Tooltip("Whether to activate the first screen when start.")]
        public bool activateFirstScreen = true;
        [Tooltip("The first screen to activated when start.")]
        public AbstractMenuScreen firstScreen;
        [SerializeField, Tooltip("[Optional] Screen Fader Prefab to use when transitioning screens.")]
        private AbstractScreenFader faderPrefab;

        /// <summary>
        /// Event fired when the given screen is opened.
        /// </summary>
        public event Action<AbstractMenuScreen> OnScreenOpened;

        /// <summary>
        /// Event fired when the given screen is canceled: the back button is pressed.
        /// </summary>
        public event Action<AbstractMenuScreen> OnScreenCanceled;

        public ElementHighlighter Highlighter => highlighter;
        public ElementFocusAudioPlayer FocusPlayer => focusPlayer;
        public ButtonClickAudioPlayer ButtonClickPlayer => buttonClickPlayer;
        public AbstractScreenFader Fader { get; private set; }
        public AbstractMenuScreen[] Screens { get; private set; }
        public AbstractMenuScreen LastScreen { get; private set; }
        public AbstractMenuScreen CurrentScreen { get; private set; }

        private readonly Stack<AbstractMenuScreen> undoHistory = new();

        protected virtual void Reset() => FindRequiredComponents();

        protected virtual void Awake()
        {
            TryFindFader();
            InitializeScreens();
        }

        protected virtual void OnEnable()
        {
            SubscribeEvents();
            TryActivateFirstScreen();
        }

        protected virtual void OnDisable() => UnsubscribeEvents();

        /// <summary>
        /// Shows the Quit Game Dialogue Popup using localization if available.
        /// Quits the game when confirmed.
        /// </summary>
        public static void ShowQuitGameDialogue()
        {
            Popups.Dialogue.Show(
                message: new LocalizedString("Popups", "are_you_sure", "Are you sure?"),
                title: new LocalizedString("Popups", "quit_title", "Quitting the game"),
                onConfirm: QuitGameAfterDialogueCloseAnimation
            );
        }

        /// <summary>
        /// Quits the Game, even while in Editor mode.
        /// <para>Shows a Quit Browser Confirmation Popup if in WebGL.</para>
        /// </summary>
        public static void QuitGame()
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                Popups.Confirmation.Show(
                    message: new LocalizedString("Popups", "webgl_quit_message", "You must close your browser manually!"),
                    title: new LocalizedString("Popups", "webgl_quit_title", "Quitting the Browser")
                );
            }
            else Application.Quit();
        }

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

        public void OpenFirstScreen() => OpenScreen(firstScreen, undoable: false);
        public void OpenScreen(AbstractMenuScreen screen, bool undoable = true) =>
            _ = OpenScreenAsync(screen, undoable);

        public async Awaitable OpenScreenAsync(AbstractMenuScreen screen, bool undoable = true)
        {
            Time.timeScale = 1f;
            SetSendNavigationEvents(false);

            var hasCurrentScreen = CurrentScreen != null;
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

        public bool TryOpenLastScreen(out AbstractMenuScreen screen)
        {
            var hasUndoableScreen = undoHistory.TryPop(out screen);
            if (hasUndoableScreen) OpenScreen(screen, undoable: false);
            return hasUndoableScreen;
        }

        protected virtual void SubscribeEvents()
        {
            AbstractPopup.OnAnyStartShow += HandleAnyPopupStartShow;
            AbstractPopup.OnAnyFinishClose += HandleAnyPopupFinishClose;
        }

        protected virtual void UnsubscribeEvents()
        {
            AbstractPopup.OnAnyStartShow -= HandleAnyPopupStartShow;
            AbstractPopup.OnAnyFinishClose -= HandleAnyPopupFinishClose;
        }

        protected virtual void FindFirstScreen() => firstScreen =
            GetComponentInChildren<AbstractMenuScreen>(includeInactive: true);

        protected virtual void InitializeScreens()
        {
            Screens = GetComponentsInChildren<AbstractMenuScreen>(includeInactive: true);
            foreach (var screen in Screens) screen.Initialize(this);
        }

        protected virtual void OnCancel()
        {
            if (!TryOpenLastScreen(out AbstractMenuScreen screen)) return;

            ButtonClickPlayer.PlayCancelSound();
            OnScreenCanceled?.Invoke(screen);
        }

        private void TryFindFader()
        {
            if (faderPrefab) Fader = ScreenFadeFactory.Create(faderPrefab);
        }

        private void FindRequiredComponents()
        {
            highlighter = GetComponent<ElementHighlighter>();
            focusPlayer = GetComponent<ElementFocusAudioPlayer>();
            buttonClickPlayer = GetComponent<ButtonClickAudioPlayer>();
            FindFirstScreen();
        }

        protected void DeactivateAllScreens()
        {
            foreach (var screen in Screens)
            {
                screen.Deactivate();
            }
        }

        private async void TryActivateFirstScreen()
        {
            if (!activateFirstScreen) return;

            // Await one frame to let the First Screen components initialize
            await Awaitable.NextFrameAsync();
            OpenFirstScreen();
        }

        private async Awaitable InitializeCurrentScreenAsync()
        {
            CurrentScreen.Root.RegisterCallback<NavigationCancelEvent>(HandleNavigationCancelEvent);

            await Awaitable.NextFrameAsync();

            CurrentScreen.Focus();
            InitializeElements();
        }

        private async Awaitable DisposeCurrentScreenAsync()
        {
            CurrentScreen.Root.UnregisterCallback<NavigationCancelEvent>(HandleNavigationCancelEvent);
            // Wait so menu elements can execute their final actions.
            await Awaitable.NextFrameAsync();
            DisposeElements();
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
            CurrentScreen.Focus();

            await InitializeCurrentScreenAsync();
        }

        private static async void QuitGameAfterDialogueCloseAnimation()
        {
            Time.timeScale = 1f;

            var time = Popups.Dialogue.GetCloseAnimationTime() + 0.1f;
            await Awaitable.WaitForSecondsAsync(time);

            var hasAvailableFader = ScreenFadeFactory.TryGetFirst(out AbstractScreenFader fader);
            if (hasAvailableFader) await fader.FadeOutAsync();

            QuitGame();
        }
    }
}