using UnityEngine;

namespace ActionCode.UISystem
{
    [DisallowMultipleComponent]
    public sealed class MainOptionsScreen : BaseScreen
    {
        [Space]
        public string startScreen = "LoadScreen";
        public string optionsMenu = "OptionsMenu";

        [Header("Buttons")]
        [SerializeField] private ActionButton startButton;
        [SerializeField] private ActionButton optionsButton;
        [SerializeField] private ActionButton exitButton;

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
        private void HandleOptionsButtonClicked() => Menu.OpenMenu(optionsMenu);
        private void HandleExitButtonClicked() => ModalMenu.ShowQuitGameDialogue();
    }
}