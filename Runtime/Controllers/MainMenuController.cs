using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Controller for a UI Toolkit Main Menu.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ElementHighlighter))]
    [RequireComponent(typeof(ButtonClickAudioPlayer))]
    [RequireComponent(typeof(ElementFocusAudioPlayer))]
    public sealed class MainMenuController : AbstractController
    {
        [SerializeField, Tooltip("The local Focus Audio Player component.")]
        private ElementFocusAudioPlayer focusPlayer;

        [Header("Button Names")]
        [SerializeField] private string continueButtonName = "Continue";
        [SerializeField] private string startButtonName = "StartGame";
        [SerializeField] private string loadButtonName = "LoadGame";
        [SerializeField] private string optionsButtonName = "Options";
        [SerializeField] private string quitButtonName = "Quit";

        public event Action OnContinueClicked;
        public event Action OnStartClicked;
        public event Action OnLoadClicked;
        public event Action OnOptionsClicked;
        public event Action OnQuitClicked;

        public Button ContinueButton { get; private set; }
        public Button StartButton { get; private set; }
        public Button LoadButton { get; private set; }
        public Button OptionsButton { get; private set; }
        public Button QuitButton { get; private set; }

        public bool IsContinueEnabled { get; set; }

        protected override void Reset()
        {
            base.Reset();
            focusPlayer = GetComponentInParent<ElementFocusAudioPlayer>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CheckContinueButtonAvailability();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ContinueButton.visible = false;
        }

        protected override void FindReferences()
        {
            base.FindReferences();
            ContinueButton = Root.Q<Button>(continueButtonName);
            StartButton = Root.Q<Button>(startButtonName);
            LoadButton = Root.Q<Button>(loadButtonName);
            OptionsButton = Root.Q<Button>(optionsButtonName);
            QuitButton = Root.Q<Button>(quitButtonName);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            ContinueButton.clicked += HandleContinueClicked;
            StartButton.clicked += HandleStartClicked;
            LoadButton.clicked += HandleLoadClicked;
            OptionsButton.clicked += HandleOptionsClicked;
            QuitButton.clicked += HandleQuitClicked;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            ContinueButton.clicked -= HandleContinueClicked;
            StartButton.clicked -= HandleStartClicked;
            LoadButton.clicked -= HandleLoadClicked;
            OptionsButton.clicked -= HandleOptionsClicked;
            QuitButton.clicked -= HandleQuitClicked;
        }

        private void HandleContinueClicked() => OnContinueClicked?.Invoke();
        private void HandleStartClicked() => OnStartClicked?.Invoke();
        private void HandleLoadClicked() => OnLoadClicked?.Invoke();
        private void HandleOptionsClicked() => OnOptionsClicked?.Invoke();
        private void HandleQuitClicked() => OnQuitClicked?.Invoke();

        private void CheckContinueButtonAvailability()
        {
            var firstButton = IsContinueEnabled ? ContinueButton : StartButton;

            ContinueButton.visible = IsContinueEnabled;
            focusPlayer.FocusWithoutSound(firstButton);
        }
    }
}
