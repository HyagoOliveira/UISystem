using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract controller for Main Menus.
    /// Handlers the Main Menu navigation between the controllers 
    /// Any Button, Main Menu, Load Game and Options.
    /// </summary>
    public abstract class AbstractMainMenu : AbstractMenu
    {
        [Header("Screens")]
        [SerializeField] protected AnyButtonScreen anyButton;
        [SerializeField] protected MainMenuScreen mainMenu;
        [SerializeField] protected AbstractController loadGame;

        protected override void Reset()
        {
            base.Reset();

            anyButton = GetComponentInChildren<AnyButtonScreen>(includeInactive: true);
            mainMenu = GetComponentInChildren<MainMenuScreen>(includeInactive: true);
        }

        private void OnEnable() => SubscribeEvents();
        private void OnDisable() => UnsubscribeEvents();

        protected abstract void StartGame();
        protected abstract void ContinueGame();

        protected override AbstractController[] GetScreens() =>
            new AbstractController[] { anyButton, mainMenu, loadGame };

        protected virtual void SubscribeEvents()
        {
            anyButton.OnAnyClicked += HandleAnyButtonClicked;

            mainMenu.OnContinueClicked += HandleContinueClicked;
            mainMenu.OnStartClicked += HandleStartClicked;
            mainMenu.OnLoadClicked += HandleLoadClicked;
            mainMenu.OnOptionsClicked += HandleOptionsClicked;
            mainMenu.OnQuitClicked += HandleQuitClicked;
        }

        protected virtual void UnsubscribeEvents()
        {
            anyButton.OnAnyClicked -= HandleAnyButtonClicked;

            mainMenu.OnContinueClicked -= HandleContinueClicked;
            mainMenu.OnStartClicked -= HandleStartClicked;
            mainMenu.OnLoadClicked -= HandleLoadClicked;
            mainMenu.OnOptionsClicked -= HandleOptionsClicked;
            mainMenu.OnQuitClicked -= HandleQuitClicked;
        }

        private void HandleContinueClicked()
        {
            DeactivateAllScreens();
            ContinueGame();
        }

        private void HandleStartClicked()
        {
            DeactivateAllScreens();
            StartGame();
        }

        private async void HandleAnyButtonClicked()
        {
            await PlaySubmitSoundAndWaitAsync();
            OpenScreen(mainMenu, undoable: false);
        }

        private void HandleLoadClicked() => OpenScreen(loadGame);
        private void HandleOptionsClicked() { } //TODO
        private void HandleQuitClicked() => QuitGame();
    }
}
