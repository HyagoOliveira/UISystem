using UnityEngine;
using UnityEngine.UI;

namespace ActionCode.UISystem
{
    public abstract class AbstractMainMenuOptionsScreen : Screen
    {
        [Header("Buttons")]
        [SerializeField] protected Button continueButton;
        [SerializeField] protected Button startButton;
        [SerializeField] protected Button loadButton;
        [SerializeField] protected Button settingsButton;
        [SerializeField] protected Button exitButton;

        [Header("Screens")]
        public string loadScreenIdentifier = "LoadScreen";
        public string settingsScreenIdentifier = "SettingsScreen";

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            continueButton.onClick.AddListener(HandleContinueButtonClicked);
            startButton.onClick.AddListener(HandleStartButtonClicked);
            loadButton.onClick.AddListener(HandleLoadButtonClicked);
            settingsButton.onClick.AddListener(HandleSettingsButtonClicked);
            exitButton.onClick.AddListener(HandleExitButtonClicked);
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            continueButton.onClick.RemoveListener(HandleContinueButtonClicked);
            startButton.onClick.RemoveListener(HandleStartButtonClicked);
            loadButton.onClick.RemoveListener(HandleLoadButtonClicked);
            settingsButton.onClick.RemoveListener(HandleSettingsButtonClicked);
            exitButton.onClick.RemoveListener(HandleExitButtonClicked);
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

        protected virtual void HandleLoadButtonClicked() => OpenCloseableScreen(loadScreenIdentifier);
        protected virtual void HandleSettingsButtonClicked() => OpenCloseableScreen(settingsScreenIdentifier);

        protected virtual void HandleExitButtonClicked()
        {
        }
    }
}