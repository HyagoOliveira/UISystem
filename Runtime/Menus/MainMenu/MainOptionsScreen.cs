using UnityEngine;

namespace ActionCode.UISystem
{
    [DisallowMultipleComponent]
    public sealed class MainOptionsScreen : BaseScreen
    {
        [Header("Buttons")]
        [SerializeField] private ActionButton startButton;
        [SerializeField] private ActionButton optionsButton;
        [SerializeField] private ActionButton exitButton;

        [Header("Screens")]
        public string startScreen = "LoadScreen";
        public string optionsScreen = "OptionsScreen";

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            startButton.OnClicked += HandleStartButtonClicked;
            optionsButton.OnClicked += HandleOptionsButtonClicked;
            exitButton.OnClicked += HandleExitButtonClicked;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            startButton.OnClicked -= HandleStartButtonClicked;
            optionsButton.OnClicked -= HandleOptionsButtonClicked;
            exitButton.OnClicked -= HandleExitButtonClicked;
        }

        private void HandleStartButtonClicked() => OpenCloseableScreen(startScreen);
        private void HandleOptionsButtonClicked() => OpenCloseableScreen(optionsScreen);
        private void HandleExitButtonClicked() => ModalMenu.ShowQuitGameDialogue();
    }
}