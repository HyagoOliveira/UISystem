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
        [SerializeField] protected AbstractMenuLoadScreen loadMenu;
        [SerializeField] protected TabScreen optionsMenu;

        protected override void Reset()
        {
            base.Reset();

            anyButton = GetComponentInChildren<AnyButtonScreen>(includeInactive: true);
            mainMenu = GetComponentInChildren<MainMenuScreen>(includeInactive: true);
            loadMenu = GetComponentInChildren<AbstractMenuLoadScreen>(includeInactive: true);
            optionsMenu = GetComponentInChildren<TabScreen>(includeInactive: true);
        }

        protected abstract void LoadGameScene();

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            anyButton.OnAnyClicked += HandleAnyButtonClicked;

            mainMenu.OnContinueClicked += HandleContinueClicked;
            mainMenu.OnNewGameClicked += HandleNewGameClicked;
            mainMenu.OnLoadClicked += HandleLoadClicked;
            mainMenu.OnOptionsClicked += HandleOptionsClicked;
            mainMenu.OnQuitClicked += HandleQuitClicked;

            if (loadMenu) loadMenu.OnDataLoadConfirmed += HandleDataLoadConfirmed;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            anyButton.OnAnyClicked -= HandleAnyButtonClicked;

            mainMenu.OnContinueClicked -= HandleContinueClicked;
            mainMenu.OnNewGameClicked -= HandleNewGameClicked;
            mainMenu.OnLoadClicked -= HandleLoadClicked;
            mainMenu.OnOptionsClicked -= HandleOptionsClicked;
            mainMenu.OnQuitClicked -= HandleQuitClicked;

            if (loadMenu) loadMenu.OnDataLoadConfirmed -= HandleDataLoadConfirmed;
        }

        private async void HandleContinueClicked()
        {
            DeactivateAllScreens();
            if (loadMenu) await loadMenu.LoadFromLastSlotAsync();
            LoadGameScene();
        }

        private void HandleNewGameClicked()
        {
            DeactivateAllScreens();
            if (loadMenu) loadMenu.ResetGameData();
            LoadGameScene();
        }

        private void HandleDataLoadConfirmed()
        {
            DeactivateAllScreens();
            LoadGameScene();
        }

        private void HandleAnyButtonClicked() => OpenScreen(mainMenu, undoable: anyButton.canGoBack);
        private void HandleLoadClicked() => OpenScreen(loadMenu);
        private void HandleOptionsClicked() => OpenScreen(optionsMenu);
        private void HandleQuitClicked() => ShowQuitGameDialogue();
    }
}
