using UnityEngine;
using UnityEngine.UIElements;
using ActionCode.AwaitableSystem;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract class for a Pause Menu Screen, with buttons: Continue, Main Menu and Quit.
    /// You can further extend.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class AbstractPauseScreen : AbstractMenuScreen
    {
        [Header("Button Names")]
        [SerializeField] private string continueButtonName = "Continue";
        [SerializeField] private string mainMenuButtonName = "MainMenu";
        [SerializeField] private string quitButtonName = "Quit";

        public Button Continue { get; private set; }
        public Button MainMenu { get; private set; }
        public Button Quit { get; private set; }

        public override async Awaitable FocusAsync()
        {
            await base.FocusAsync();
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

            Root.RegisterCallback<NavigationCancelEvent>(HandleNavigationCancelEvent);
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Continue.clicked -= HandleContinueClicked;
            MainMenu.clicked -= HandleMainMenuClicked;
            Quit.clicked -= HandleQuitClicked;

            Root.UnregisterCallback<NavigationCancelEvent>(HandleNavigationCancelEvent);
        }

        protected abstract void ContinueGame();
        protected abstract void GoToMainMenu();

        protected virtual void HandleContinueClicked() => ContinueGame();
        protected virtual void HandleMainMenuClicked() => Popups.ShowQuitLevelDialogue(GoToMainMenu);
        protected virtual void HandleQuitClicked() => Popups.ShowQuitLevelDialogue(Popups.QuitGameAfterCloseAnimation);
        protected virtual void HandleNavigationCancelEvent(NavigationCancelEvent _) => ContinueGameAfterSelectAnimation();

        private async void ContinueGameAfterSelectAnimation()
        {
            var shouldSelectContinueButton = !Continue.IsFocused();
            if (shouldSelectContinueButton)
            {
                MenuController.SetSendNavigationEvents(false);
                Continue.Focus();
                await AwaitableUtility.WaitForSecondsRealtimeAsync(0.2f);
            }

            ContinueGame();
        }
    }
}