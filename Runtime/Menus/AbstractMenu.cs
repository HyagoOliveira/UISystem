using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ActionCode.AwaitableSystem;

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
        [SerializeField, Tooltip("The local Button Click Player for this menu.")]
        private ButtonClickAudioPlayer buttonClickPlayer;
        [SerializeField, Tooltip("The local Focus Player for this menu.")]
        private ElementFocusAudioPlayer focusPlayer;

        [Header("Screen Transition")]
        [Tooltip("Whether to activate the first screen when start.")]
        public bool activateFirstScreen = true;
        [Tooltip("The first screen to activated when start.")]
        public AbstractMenuScreen firstScreen;
        [Min(0f), Tooltip("The time (in seconds) between screen transitions.")]
        public float transitionTime = 0.2f;

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
        public AbstractMenuScreen[] Screens { get; private set; }
        public AbstractMenuScreen LastScreen { get; private set; }
        public AbstractMenuScreen CurrentScreen { get; private set; }

        private readonly Stack<AbstractMenuScreen> undoHistory = new();

        protected virtual void Reset() => FindRequiredComponents();
        protected virtual void Awake() => InitializeScreens();
        protected virtual void Start() => TryActivateFirstScreen();
        protected virtual void OnEnable() => SubscribeEvents();
        protected virtual void OnDisable() => UnsubscribeEvents();

        /// <summary>
        /// Quits the Game, even while in Editor mode.
        /// </summary>
        public static void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void OpenFirstScreen() => OpenScreen(firstScreen, undoable: false);
        public void OpenScreen(AbstractMenuScreen screen, bool undoable = true) =>
            _ = OpenScreenAsync(screen, undoable);

        public async Awaitable OpenScreenAsync(AbstractMenuScreen screen, bool undoable = true)
        {
            Time.timeScale = 1f;

            var hasCurrentScreen = CurrentScreen != null;
            if (hasCurrentScreen)
            {
                CurrentScreen.Root.UnregisterCallback<NavigationCancelEvent>(HandleNavigationCancelEvent);
                // Wait so menu elements can execute their final actions.
                await Awaitable.NextFrameAsync();
                DisposeElements();
            }

            await AwaitableUtility.WaitForFramesAsync(10);

            LastScreen = CurrentScreen;
            var applyTransition = CurrentScreen && CurrentScreen.IsEnabled;

            if (applyTransition)
            {
                DeactivateAllScreens();
                await Awaitable.WaitForSecondsAsync(transitionTime);
            }

            if (undoable)
            {
                var hasLastController = LastScreen != null;
                if (hasLastController) undoHistory.Push(LastScreen);
            }

            if (screen == null) return;

            CurrentScreen = screen;
            CurrentScreen.Activate();
            CurrentScreen.SetVisibility(true);
            CurrentScreen.Root.RegisterCallback<NavigationCancelEvent>(HandleNavigationCancelEvent);

            await Awaitable.NextFrameAsync();
            CurrentScreen.Focus();

            InitializeElements();
            OnScreenOpened?.Invoke(CurrentScreen);
        }

        public bool TryOpenLastScreen(out AbstractMenuScreen screen)
        {
            var hasUndoableScreen = undoHistory.TryPop(out screen);
            if (hasUndoableScreen) OpenScreen(screen, undoable: false);
            return hasUndoableScreen;
        }

        protected virtual void SubscribeEvents() { }
        protected virtual void UnsubscribeEvents() { }

        protected virtual void FindFirstScreen() => firstScreen =
            GetComponentInChildren<AbstractMenuScreen>(includeInactive: true);

        protected virtual void InitializeScreens()
        {
            Screens = GetComponentsInChildren<AbstractMenuScreen>(includeInactive: true);
            foreach (var screen in Screens) screen.Initialize(this);
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

        private void TryActivateFirstScreen()
        {
            if (activateFirstScreen) OpenFirstScreen();
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

        private void HandleNavigationCancelEvent(NavigationCancelEvent evt)
        {
            Debug.Log($"OnNavCancelEvent {evt.propagationPhase}");

            if (Popups.IsDisplayingAnyPopup()) return;
            if (!TryOpenLastScreen(out AbstractMenuScreen screen)) return;

            ButtonClickPlayer.PlayCancelSound();
            OnScreenCanceled?.Invoke(screen);
        }
    }
}