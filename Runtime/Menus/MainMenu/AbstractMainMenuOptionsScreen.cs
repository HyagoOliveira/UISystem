using UnityEngine;

namespace ActionCode.UISystem
{
    public abstract class AbstractMainMenuOptionsScreen : BaseScreen
    {
        [Header("Buttons")]
        [SerializeField] protected ActionButton continueButton;
        [SerializeField] protected ActionButton startButton;
        [SerializeField] protected ActionButton loadButton;
        [SerializeField] protected ActionButton optionsButton;
        [SerializeField] protected ActionButton exitButton;

        [Header("Screens")]
        public string loadScreen = "LoadScreen";
        public string optionsScreen = "OptionsScreen";

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            continueButton.OnClicked += HandleContinueButtonClicked;
            startButton.OnClicked += HandleStartButtonClicked;
            loadButton.OnClicked += HandleLoadButtonClicked;
            optionsButton.OnClicked += HandleSettingsButtonClicked;
            exitButton.OnClicked += HandleExitButtonClicked;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            continueButton.OnClicked -= HandleContinueButtonClicked;
            startButton.OnClicked -= HandleStartButtonClicked;
            loadButton.OnClicked -= HandleLoadButtonClicked;
            optionsButton.OnClicked -= HandleSettingsButtonClicked;
            exitButton.OnClicked -= HandleExitButtonClicked;
        }

        public override async Awaitable LoadAsync()
        {
            var isContinueAvailable = await IsContinueAvailableAsync();
            continueButton.gameObject.SetActive(isContinueAvailable);
            firstInput = isContinueAvailable ? continueButton.gameObject : startButton.gameObject;
        }

        protected abstract Awaitable<bool> IsContinueAvailableAsync();
        protected abstract void HandleContinueButtonClicked();
        protected abstract void HandleStartButtonClicked();

        protected virtual void HandleLoadButtonClicked() => OpenCloseableScreen(loadScreen);
        protected virtual void HandleSettingsButtonClicked() => OpenCloseableScreen(optionsScreen);

        protected virtual void HandleExitButtonClicked()
        {
        }
    }
}