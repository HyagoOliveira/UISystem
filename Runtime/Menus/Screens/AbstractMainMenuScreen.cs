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
        [SerializeField] protected string startGameButtonName = "StartGame";
        [SerializeField] protected string optionsButtonName = "Options";
        [SerializeField] protected string quitButtonName = "Quit";

        [Header("Screen Names")]
        [SerializeField, Tooltip("The Start Game Screen name to open when clicking the Start Game button.")]
        protected string startGameScreenName = "StartGameScreen";
        [SerializeField, Tooltip("The Options Screen name to open when clicking the Options button.")]
        protected string optionsScreenName = "OptionsScreen";

        public Button Continue { get; private set; }
        public Button StartGame { get; private set; }
        public Button Options { get; private set; }
        public Button Quit { get; private set; }

        private bool isContinueAvailable;

        public override void Focus()
        {
            var button = isContinueAvailable ? Continue : StartGame;
            button.Focus();
            Continue.SetEnabled(isContinueAvailable);
        }

        public override async Awaitable LoadAnyContent() =>
            isContinueAvailable = await IsContinueAvailable();

        protected override void FindReferences()
        {
            base.FindReferences();

            Continue = Root.Q<Button>(continueButtonName);
            StartGame = Root.Q<Button>(startGameButtonName);
            Options = Root.Q<Button>(optionsButtonName);
            Quit = Root.Q<Button>(quitButtonName);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Continue.clicked += HandleContinueClicked;
            StartGame.clicked += HandleStartGameClicked;
            Options.clicked += HandleOptionsClicked;
            Quit.clicked += HandleQuitClicked;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Continue.clicked -= HandleContinueClicked;
            StartGame.clicked -= HandleStartGameClicked;
            Options.clicked -= HandleOptionsClicked;
            Quit.clicked -= HandleQuitClicked;
        }

        protected abstract void HandleContinueClicked();
        protected abstract Awaitable<bool> IsContinueAvailable();

        protected virtual void HandleStartGameClicked() => _ = Menu.OpenScreenAsync(startGameScreenName, undoable: true);
        protected virtual void HandleOptionsClicked() => _ = Menu.OpenScreenAsync(optionsScreenName, undoable: true);
        protected virtual void HandleQuitClicked() => Popups.ShowQuitGameDialogue();
    }
}