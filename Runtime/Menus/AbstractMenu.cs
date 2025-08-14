using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ActionCode.InputSystem;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract controller for generic Menu.
    /// <para>
    /// A Menu consists of several Screens that can be activated (one by time),
    /// navigating between then.
    /// </para>
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(AudioSource))]
    public abstract class AbstractMenu : MonoBehaviour
    {
        [SerializeField, Tooltip("The local AudioSource for this menu.")]
        private AudioSource audioSource;
        [SerializeField, Tooltip("The Global Menu Data.")]
        private MenuData menuData;

        [Header("Screen Transition")]
        [Tooltip("Whether to activate the first screen when start.")]
        public bool activateFirstScreen = true;
        [Tooltip("The first screen to activated when start.")]
        public AbstractController firstScreen;
        [Min(0f), Tooltip("The time (in seconds) between screen transitions.")]
        public float transitionTime = 0.2f;

        [Header("Input")]
        [SerializeField] private InputActionAsset input;
        [SerializeField] private InputActionPopup cancel = new(nameof(input), "UI", "Cancel");

        /// <summary>
        /// Event fired when the given screen is opened.
        /// </summary>
        public event Action<AbstractController> OnScreenOpened;

        /// <summary>
        /// Event fired when the given screen is canceled: the back button is pressed.
        /// </summary>
        public event Action<AbstractController> OnScreenCanceled;

        public MenuData Data => menuData;
        public AudioSource Audio => audioSource;
        public AbstractController LastScreen { get; private set; }
        public AbstractController CurrentScreen { get; private set; }

        private InputAction cancelAction;
        private readonly Stack<AbstractController> undoHistory = new();

        protected virtual void Reset()
        {
            audioSource = GetComponent<AudioSource>();
            FindFirstScreen();
        }

        protected virtual void Awake() => cancelAction = input.FindAction(cancel.GetPath());
        protected virtual void Start() => TryActivateFirstScreen();
        protected virtual void OnEnable() => SubscribeEvents();
        protected virtual void OnDisable() => UnsubscribeEvents();

        public void PlaySubmitSound() => Audio.PlayOneShot(Data.submit);
        public void PlayCancelSound() => Audio.PlayOneShot(Data.cancel);

        public async Awaitable PlaySubmitSoundAndWaitAsync()
        {
            PlaySubmitSound();
            await Awaitable.WaitForSecondsAsync(menuData.submit.length);
        }

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

        public void OpenScreen(AbstractController screen, bool undoable = true) =>
            _ = OpenScreenAsync(screen, undoable);

        public async Awaitable OpenScreenAsync(AbstractController screen, bool undoable = true)
        {
            Time.timeScale = 1f;

            LastScreen = CurrentScreen;
            var applyTransition = CurrentScreen && CurrentScreen.IsEnabled;

            if (applyTransition)
            {
                if (undoable) await PlaySubmitSoundAndWaitAsync();

                DeactivateAllScreens();
                await Awaitable.WaitForSecondsAsync(transitionTime);
            }

            if (undoable)
            {
                var hasLastController = LastScreen != null;
                if (hasLastController) undoHistory.Push(LastScreen);
            }

            CurrentScreen = screen;
            CurrentScreen.Activate();
            CurrentScreen.SetVisibility(true);

            OnScreenOpened?.Invoke(CurrentScreen);
        }

        public bool TryOpenLastScreen(out AbstractController screen)
        {
            var hasUndoableScreen = undoHistory.TryPop(out screen);
            if (hasUndoableScreen) OpenScreen(screen, undoable: false);
            return hasUndoableScreen;
        }

        protected abstract AbstractController[] GetScreens();

        protected virtual void SubscribeEvents() => cancelAction.performed += HandleCancelPerformed;
        protected virtual void UnsubscribeEvents() => cancelAction.performed -= HandleCancelPerformed;

        protected virtual void FindFirstScreen() => firstScreen =
            GetComponentInChildren<AbstractController>(includeInactive: true);

        protected void DeactivateAllScreens()
        {
            foreach (var screen in GetScreens())
            {
                screen.Deactivate();
            }
        }

        private void TryActivateFirstScreen()
        {
            if (activateFirstScreen) OpenFirstScreen();
        }

        private void HandleCancelPerformed(InputAction.CallbackContext _)
        {
            if (!TryOpenLastScreen(out AbstractController screen)) return;

            PlayCancelSound();
            OnScreenCanceled?.Invoke(screen);
        }
    }
}