using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract controller for generic Menus.
    /// A Menu consists of several Controllers that can be activated one by time.
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

        [Header("Transition")]
        [Tooltip("Whether to activate the first controller when start.")]
        public bool activateFirstController = true;
        [Tooltip("The first controller to enable when start.")]
        public AbstractController firstController;
        [Min(0f), Tooltip("The time (in seconds) between transitions.")]
        public float transitionTime = 0.2f;

        public event Action<AbstractController> OnScreenActivated;

        public MenuAudioHandler Audio => audio;
        public AbstractController LastScreen { get; private set; }
        public AbstractController CurrentScreen { get; private set; }

        private readonly Stack<AbstractController> undoHistory = new();

        protected virtual void Reset()
        {
            audio = GetComponent<MenuAudioHandler>();
            FindFirstController();
        }

        protected virtual void Start() => TryActivateFirstController();

        protected abstract AbstractController[] GetScreens();

        /// <summary>
        /// Quits the Game even in Editor mode.
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
            StartCoroutine(OpenScreenRoutine(controller, undoable));

        public IEnumerator OpenScreenRoutine(AbstractController controller, bool undoable)
        {
            LastScreen = CurrentScreen;
            var applyTransition = CurrentScreen && CurrentScreen.IsEnabled;

            if (applyTransition)
            {
                if (undoable) yield return Audio.PlayAndWaitSubmitSound();

                DeactivateAllScreens();

                yield return new WaitForSecondsRealtime(transitionTime);
            }

            if (undoable)
            {
                var hasLastController = LastScreen != null;
                if (hasLastController) undoHistory.Push(LastScreen);
            }

            CurrentScreen = controller;
            CurrentScreen.Activate();
            CurrentScreen.SetVisibility(true);

            OnScreenActivated?.Invoke(CurrentScreen);
        }

        public void OpenFirstScreen() => OpenScreen(firstController, undoable: false);

        public bool TryOpenLastScreen()
        {
            var hasUndoableController = undoHistory.TryPop(out var controller);
            if (hasUndoableController) OpenScreen(controller, undoable: false);
            return hasUndoableController;
        }

        protected virtual void FindFirstController() =>
            firstController = GetComponentInChildren<AbstractController>(includeInactive: true);

        protected void DeactivateAllScreens()
        {
            foreach (var screen in GetScreens())
            {
                screen.Deactivate();
            }
        }

        private void TryActivateFirstController()
        {
            if (activateFirstController) OpenFirstScreen();
        }
    }
}