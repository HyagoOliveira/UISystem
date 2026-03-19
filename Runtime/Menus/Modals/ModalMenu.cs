using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Menu for multiple Modal Screens.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ModalMenu : Menu
    {
        [SerializeField] private DialogueModal dialogue;

        public static bool HasAnyOpened { get; private set; }
        private static ModalMenu Instance { get; set; }
        private static Menu SourceMenu { get; set; }
        private static GameObject LastSelectedGameObject { get; set; }

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
            SourceMenu = null;
            HasAnyOpened = false;
            LastSelectedGameObject = null;
        }

        // Called by any ActionButton Cancel event
        public override async void OnCancel(BaseEventData _) => await CloseAsync();

        public static bool TryGetModal<T>(out T modal) where T : DialogueModal
        {
            modal = GetModal<T>();
            return modal != null;
        }

        public static T GetModal<T>() where T : DialogueModal
        {
            foreach (var screen in Instance.Screens.Values)
            {
                if (screen is T modal) return modal;
            }

            return null;
        }

        /// <summary>
        /// Shows a localized Confirmation Modal with a confirmed callback.
        /// </summary>
        /// <param name="message">The localized message with optional fallback text.</param>
        /// <param name="title">The localized title with optional fallback text.</param>
        /// <param name="onConfirm">An optional action to execute when the Modal is confirmed.</param>
        public static void ShowConfirmation(
            LocalizedString message,
            LocalizedString title,
            Action onConfirm = null
        ) => ShowDialogue(message, title, onConfirm, onCancel: null, showCancelButton: false);

        /// <summary>
        /// Shows a localized Dialogue Modal with a confirmed/canceled callbacks.
        /// </summary>
        /// <param name="message"><inheritdoc cref="ShowConfirmation(LocalizedString, LocalizedString, Action)" path="/param[@name='message']"/></param>
        /// <param name="title"><inheritdoc cref="ShowConfirmation(LocalizedString, LocalizedString, Action)" path="/param[@name='title']"/></param>
        /// <param name="onConfirm"><inheritdoc cref="ShowConfirmation(LocalizedString, LocalizedString, Action)" path="/param[@name='onConfirm']"/></param>
        /// <param name="onCancel">An optional action to execute when the Modal is canceled.</param>
        public static void ShowDialogue(
            LocalizedString message,
            LocalizedString title,
            Action onConfirm = null,
            Action onCancel = null
        ) => ShowDialogue(message, title, onConfirm, onCancel, showCancelButton: true);

        /// <summary>
        /// Shows the Quit Level Dialogue Popup using localization if available.
        /// Executes the given Confirmation action when confirmed.
        /// </summary>
        /// <param name="onConfirm">The action to execute when the Dialogue is confirmed.</param>
        public static void ShowQuitLevelDialogue(Action onConfirm) => ShowDialogue(
            message: new LocalizedString("Modals", "are_you_sure_level", "Are you sure?\nAll unsaved progress will be lost."),
            title: new LocalizedString("Modals", "quit_title_level", "Quitting the Level"),
            onConfirm
        );

        /// <summary>
        /// Shows the Quit Game Dialogue Modal.
        /// </summary>
        /// <remarks>
        /// Quits the game when confirmed.
        /// </remarks>
        public static void ShowQuitGameDialogue() => ShowDialogue(
            message: new LocalizedString("Modals", "are_you_sure", "Are you sure?"),
            title: new LocalizedString("Modals", "quit_title", "Quitting the game"),
            onConfirm: QuitGameAfterCloseAnimation
        );

        public static async Awaitable CloseAsync()
        {
            Instance.DisableInput();
            Instance.Audio.PlayModalClose();
            await Instance.CloseCurrentScreenAsync();
            HasAnyOpened = false;

            if (SourceMenu)
            {
                SourceMenu.EnableInput();
                SourceMenu = null;
            }

            if (LastSelectedGameObject)
            {
                EventManager.TrySetSelectedGameObject(LastSelectedGameObject);
                LastSelectedGameObject = null;
            }
        }

        public static async void QuitGameAfterCloseAnimation()
        {
            //IsQuitting = true;
            await CloseAsync();
            QuitGame();
        }

        /// <summary>
        /// Quits the Game, even while in Editor mode.
        /// </summary>
        /// <remarks>
        /// Shows a Quit Browser Confirmation Popup if in WebGL.
        /// </remarks>
        public static void QuitGame()
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                ShowConfirmation(
                    message: new LocalizedString("Popups", "webgl_quit_message", "You must close your browser manually!"),
                    title: new LocalizedString("Popups", "webgl_quit_title", "Quitting the Browser")
                );
            }
            else
            {
                Time.timeScale = 1f;
                Application.Quit();
            }
        }

        private static void ShowDialogue(
            LocalizedString message,
            LocalizedString title,
            Action onConfirm,
            Action onCancel,
            bool showCancelButton
        )
        {
            var modal = Instance.dialogue;

            modal.Show(message, title, onConfirm, onCancel);
            modal.SetCancelButtonVisibility(showCancelButton);

            LastSelectedGameObject = EventManager.GetSelectedGameObject();

            SourceMenu = LastOpenedMenu;
            if (SourceMenu) SourceMenu.DisableInput();

            HasAnyOpened = true;
            _ = Instance.OpenScreenAsync(modal);
        }
    }
}