using UnityEngine;
using System;
using System.Collections.Generic;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract controller for generic Menu.
    /// <para>
    /// A Menu consists of several Screens that can be activated one by time.
    /// </para>
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(MenuAudioHandler))]
    public abstract class AbstractMenu : MonoBehaviour
    {
        [SerializeField, Tooltip("The Audio Handler for this menu.")]
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        private MenuAudioHandler audio;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

        [Header("Screen Transition")]
        [Tooltip("Whether to activate the first screen when start.")]
        public bool activateFirstScreen = true;
        [Tooltip("The first screen to activated when start.")]
        public AbstractController firstScreen;
        [Min(0f), Tooltip("The time (in seconds) between screen transitions.")]
        public float transitionTime = 0.2f;

        /// <summary>
        /// Event fired when the given screen is opened.
        /// </summary>
        public event Action<AbstractController> OnScreenOpened;

        public MenuAudioHandler Audio => audio;
        public AbstractController LastScreen { get; private set; }
        public AbstractController CurrentScreen { get; private set; }

        private readonly Stack<AbstractController> undoHistory = new();

        protected virtual void Reset()
        {
            audio = GetComponent<MenuAudioHandler>();
            FindFirstScreen();
        }

        protected virtual void Start() => TryActivateFirstScreen();

        protected abstract AbstractController[] GetScreens();

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

        public void OpenScreen(AbstractController controller, bool undoable = true) =>
            _ = OpenScreenAsync(controller, undoable);

        public async Awaitable OpenScreenAsync(AbstractController controller, bool undoable = true)
        {
            Time.timeScale = 1f;

            LastScreen = CurrentScreen;
            var applyTransition = CurrentScreen && CurrentScreen.IsEnabled;

            if (applyTransition)
            {
                if (undoable) await Audio.PlayAndWaitSubmitSound();

                DeactivateAllScreens();

                await Awaitable.WaitForSecondsAsync(transitionTime);
            }

            if (undoable)
            {
                var hasLastController = LastScreen != null;
                if (hasLastController) undoHistory.Push(LastScreen);
            }

            CurrentScreen = controller;
            CurrentScreen.Activate();
            CurrentScreen.SetVisibility(true);

            OnScreenOpened?.Invoke(CurrentScreen);
        }

        public void OpenFirstScreen() => OpenScreen(firstScreen, undoable: false);

        public bool TryOpenLastScreen()
        {
            var hasUndoableController = undoHistory.TryPop(out var controller);
            if (hasUndoableController) OpenScreen(controller, undoable: false);
            return hasUndoableController;
        }

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
    }
}