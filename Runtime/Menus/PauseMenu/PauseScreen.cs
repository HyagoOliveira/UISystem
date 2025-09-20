using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Base class for a Pause Menu Screen.
    /// Use it with a <see cref="AbstractPauseMenu"/> implementation.
    /// </summary>
    [DisallowMultipleComponent]
    public class PauseScreen : AbstractMenuScreen
    {
        [Header("Button Names")]
        [SerializeField] private string continueButtonName = "Continue";
        [SerializeField] private string mainMenuButtonName = "MainMenu";
        [SerializeField] private string quitButtonName = "Quit";

        public Button Continue { get; private set; }
        public Button MainMenu { get; private set; }
        public Button Quit { get; private set; }

        public event Action OnContinueClicked;
        public event Action OnMainMenuClicked;
        public event Action OnQuitClicked;

        public override void Focus()
        {
            base.Focus();
            Continue.Focus();
        }

        protected override void FindReferences()
        {
            base.FindReferences();

            Continue = Find<Button>(continueButtonName);
            MainMenu = Find<Button>(mainMenuButtonName);
            Quit = Find<Button>(quitButtonName);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Continue.clicked += HandleContinueClicked;
            MainMenu.clicked += HandleMainMenuClicked;
            Quit.clicked += HandleQuitClicked;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Continue.clicked -= HandleContinueClicked;
            MainMenu.clicked -= HandleMainMenuClicked;
            Quit.clicked -= HandleQuitClicked;
        }

        private void HandleContinueClicked() => OnContinueClicked?.Invoke();
        private void HandleMainMenuClicked() => OnMainMenuClicked?.Invoke();
        private void HandleQuitClicked() => OnQuitClicked?.Invoke();
    }
}