using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract controller for a UI Toolkit Main Menu Screen, with the main 
    /// options like Continue, Start Game, Load Game, Options and Quit.
    /// <para>
    /// You can extend this class and implement custom behaviors.
    /// </para>
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class AbstractMainMenuScreen : AbstractMenuScreen
    {
        [Header("Button Names")]
        [SerializeField] protected string continueButtonName = "Continue";
        [SerializeField] protected string newGameButtonName = "NewGame";
        [SerializeField] protected string loadButtonName = "LoadGame";
        [SerializeField] protected string optionsButtonName = "Options";
        [SerializeField] protected string quitButtonName = "Quit";

        [Header("Screen Names")]
        [SerializeField, Tooltip("The Load Game Screen name to open when clicking the Load Game button.")]
        protected string loadGameScreenName = "LoadGameScreen";
        [SerializeField, Tooltip("The Options Screen name to open when clicking the Options button.")]
        protected string optionsScreenName = "OptionsScreen";

        public Button Continue { get; private set; }
        public Button NewGame { get; private set; }
        public Button LoadGame { get; private set; }
        public Button Options { get; private set; }
        public Button Quit { get; private set; }

        private bool isContinueAvailable;

        public override void Focus()
        {
            var button = isContinueAvailable ? Continue : NewGame;

            button.Focus();
            Continue.SetEnabled(isContinueAvailable);
        }

        public override async Awaitable LoadAnyContent() =>
            isContinueAvailable = await IsContinueAvailable();

        protected override void FindReferences()
        {
            base.FindReferences();

            Continue = Root.Q<Button>(continueButtonName);
            NewGame = Root.Q<Button>(newGameButtonName);
            LoadGame = Root.Q<Button>(loadButtonName);
            Options = Root.Q<Button>(optionsButtonName);
            Quit = Root.Q<Button>(quitButtonName);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Continue.clicked += HandleContinueClicked;
            NewGame.clicked += HandleNewGameClicked;
            LoadGame.clicked += HandleLoadClicked;
            Options.clicked += HandleOptionsClicked;
            Quit.clicked += HandleQuitClicked;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Continue.clicked -= HandleContinueClicked;
            NewGame.clicked -= HandleNewGameClicked;
            LoadGame.clicked -= HandleLoadClicked;
            Options.clicked -= HandleOptionsClicked;
            Quit.clicked -= HandleQuitClicked;
        }

        protected abstract Awaitable<bool> IsContinueAvailable();
        protected abstract void HandleContinueClicked();
        protected abstract void HandleNewGameClicked();

        protected virtual void HandleLoadClicked() => _ = Menu.OpenScreenAsync(loadGameScreenName, undoable: true);
        protected virtual void HandleOptionsClicked() => _ = Menu.OpenScreenAsync(optionsScreenName, undoable: true);
        protected virtual void HandleQuitClicked() => Popups.ShowQuitGameDialogue();
    }
}