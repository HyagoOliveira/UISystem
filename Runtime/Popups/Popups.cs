using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ActionCode.AwaitableSystem;
using ActionCode.ScreenFadeSystem;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Data holder for all Global Popups.
    /// <para>Use it to access any popup in the game.</para>
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(ElementHighlighter))]
    [RequireComponent(typeof(ButtonClickAudioPlayer))]
    [RequireComponent(typeof(ElementFocusAudioPlayer))]
    public sealed class Popups : MonoBehaviour
    {
        [SerializeField, Tooltip("The local AudioSource component.")]
        private AudioSource source;
        [SerializeField, Tooltip("The Global Menu Data.")]
        private MenuData data;

        [Header("Elements")]
        [SerializeField, Tooltip("The local Highlighter for all Popups.")]
        private ElementHighlighter highlighter;
        [SerializeField, Tooltip("The local Focus Player for all Popups.")]
        private ElementFocusAudioPlayer focusPlayer;
        [SerializeField, Tooltip("The local Button Click Player for all Popups.")]
        private ButtonClickAudioPlayer buttonClickPlayer;

        public MenuData Data => data;
        public AudioSource Audio => source;
        public ElementHighlighter Highlighter => highlighter;
        public ElementFocusAudioPlayer FocusPlayer => focusPlayer;
        public ButtonClickAudioPlayer ButtonClickPlayer => buttonClickPlayer;

        public AbstractPopup Current { get; private set; }

        /// <summary>
        /// The global Dialogue Popup.
        /// </summary>
        public static DialoguePopup Dialogue => GetPopup<DialoguePopup>();

        /// <summary>
        /// The global Confirmation Popup.
        /// </summary>
        public static ConfirmationPopup Confirmation => GetPopup<ConfirmationPopup>();

        /// <summary>
        /// Whether the application is quitting.
        /// </summary>
        public static bool IsQuitting { get; private set; }

        /// <summary>
        /// Whether a Scene loading process is happening.
        /// </summary>
        public static bool IsLoadingScene { get; set; }

        private static Popups Instance { get; set; }

        private readonly Dictionary<Type, AbstractPopup> popups = new();

        private void Reset() => FindComponents();

        private void Awake()
        {
            Instance = this;
            FindPopups();
        }

        private void OnEnable() => SubscriveEvents();
        private void OnDisable() => UnsubscribeEvents();

        private void OnDestroy()
        {
            DisposeElements();
            Instance = null;
        }

        /// <summary>
        /// Gets a child popup. No safe check is done.
        /// </summary>
        /// <typeparam name="T">The type of popup.</typeparam>
        /// <returns>The popup instance if its a children.</returns>
        public static T GetPopup<T>() where T : AbstractPopup => Instance.popups[typeof(T)] as T;

        /// <summary>
        /// Tries to get a child popup.
        /// </summary>
        /// <typeparam name="T">The type of popup.</typeparam>
        /// <param name="popup">The popup instance if its a children</param>
        /// <returns>Whether the popup was found.</returns>
        public static bool TryGetPopup<T>(out AbstractPopup popup) where T : AbstractPopup =>
            Instance.popups.TryGetValue(typeof(T), out popup);

        /// <summary>
        /// Whether the game can have navigation (not quitting or loading a scene).
        /// </summary>
        /// <returns></returns>
        public static bool CanHaveNavigation() => !IsQuitting && !IsLoadingScene;

        /// <summary>
        /// Shows the Quit Game Dialogue Popup using localization if available.
        /// Quits the game when confirmed.
        /// </summary>
        public static void ShowQuitGameDialogue()
        {
            Dialogue.Show(
                message: new LocalizedString("Popups", "are_you_sure", "Are you sure?"),
                title: new LocalizedString("Popups", "quit_title", "Quitting the game"),
                onConfirm: QuitGameAfterCloseAnimation
            );
        }

        /// <summary>
        /// Shows the Quit Level Dialogue Popup using localization if available.
        /// Executes the given Confirmation action when confirmed.
        /// </summary>
        /// <param name="onConfirm">The action to execute when the Dialogue is confirmed.</param>
        public static void ShowQuitLevelDialogue(Action onConfirm)
        {
            Dialogue.Show(
                message: new LocalizedString("Popups", "are_you_sure_level", "Are you sure?\nAll unsaved progress will be lost."),
                title: new LocalizedString("Popups", "quit_title_level", "Quitting the Level"),
                onConfirm
            );
        }

        /// <summary>
        /// Quits the Game, even while in Editor mode.
        /// <para>Shows a Quit Browser Confirmation Popup if in Web GL.</para>
        /// </summary>
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
                Confirmation.Show(
                    message: new LocalizedString("Popups", "webgl_quit_message", "You must close your browser manually!"),
                    title: new LocalizedString("Popups", "webgl_quit_title", "Quitting the Browser")
                );
            }
            else Application.Quit();
        }

        /// <summary>
        /// Quits the Game, even while in Editor mode, after the current Dialogue Close Animation.
        /// <para>Shows a Quit Browser Confirmation Popup if in WebGL.</para>
        /// </summary>
        public static async void QuitGameAfterCloseAnimation()
        {
            IsQuitting = true;
            Time.timeScale = 1f;

            var time = Dialogue.GetCloseAnimationTime() + 0.1f;
            await AwaitableUtility.WaitForSecondsRealtimeAsync(time);

            var hasAvailableFader = ScreenFadeFactory.TryGetFirst(out AbstractScreenFader fader);
            if (hasAvailableFader) await fader.FadeOutAsync();

            QuitGame();
        }

        private void FindPopups()
        {
            popups.Clear();
            var childPopups = GetComponentsInChildren<AbstractPopup>(includeInactive: true);

            foreach (var popup in childPopups)
            {
                var type = popup.GetType();
                popups.Add(type, popup);
            }
        }

        private void FindComponents()
        {
            source = GetComponent<AudioSource>();
            highlighter = GetComponent<ElementHighlighter>();
            focusPlayer = GetComponent<ElementFocusAudioPlayer>();
            buttonClickPlayer = GetComponent<ButtonClickAudioPlayer>();
        }

        private void InitializeElements(VisualElement root)
        {
            Highlighter.Initialize(root);
            FocusPlayer.Initialize(root);
            ButtonClickPlayer.Initialize(root);
        }

        private void DisposeElements()
        {
            Highlighter.Dispose();
            FocusPlayer.Dispose();
            ButtonClickPlayer.Dispose();
        }

        private void SubscriveEvents()
        {
            AbstractPopup.OnAnyStartShow += HandleAnyPopupStartShow;
            AbstractPopup.OnAnyFinishShow += HandleAnyPopupFinishShow;
            AbstractPopup.OnAnyStartClose += HandleAnyPopupStartClose;
        }

        private void UnsubscribeEvents()
        {
            AbstractPopup.OnAnyStartShow -= HandleAnyPopupStartShow;
            AbstractPopup.OnAnyFinishShow -= HandleAnyPopupFinishShow;
            AbstractPopup.OnAnyStartClose -= HandleAnyPopupStartClose;
        }

        private void HandleAnyPopupStartShow(AbstractPopup _) => Audio.PlayOneShot(Data.openPopup);

        private void HandleAnyPopupFinishShow(AbstractPopup popup)
        {
            Current = popup;
            InitializeElements(Current.Root);
        }

        private void HandleAnyPopupStartClose(AbstractPopup _)
        {
            Audio.PlayOneShot(Data.closePopup);
            Current = null;
            DisposeElements();
        }
    }
}