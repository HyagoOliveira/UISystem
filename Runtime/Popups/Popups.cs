using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Data holder for all Global Popups.
    /// <para>Use it to access any popup in the game.</para>
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ElementHighlighter))]
    [RequireComponent(typeof(ButtonClickAudioPlayer))]
    [RequireComponent(typeof(ElementFocusAudioPlayer))]
    public sealed class Popups : MonoBehaviour
    {
        [SerializeField, Tooltip("The local Highlighter for all Popups.")]
        private ElementHighlighter highlighter;
        [SerializeField, Tooltip("The local Focus Player for all Popups.")]
        private ElementFocusAudioPlayer focusPlayer;
        [SerializeField, Tooltip("The local Button Click Player for all Popups.")]
        private ButtonClickAudioPlayer buttonClickPlayer;

        [Header("Popups")]
        [SerializeField, Tooltip("The local Dialogue Popup.")]
        private DialoguePopup dialogue;

        public ElementHighlighter Highlighter => highlighter;
        public ElementFocusAudioPlayer FocusPlayer => focusPlayer;
        public ButtonClickAudioPlayer ButtonClickPlayer => buttonClickPlayer;

        /// <summary>
        /// The global Dialogue Popup.
        /// </summary>
        public static DialoguePopup Dialogue => Instance.dialogue;

        public static Popups Instance { get; private set; }

        private void Reset()
        {
            FindElements();
            FindPopups();
        }

        private void Awake() => Instance = this;
        private void OnEnable() => SubscrivePopups();
        private void OnDisable() => UnsubscribePopups();
        private void OnDestroy() => Instance = null;

        public static bool IsDisplayingAny() => Dialogue && Dialogue.isActiveAndEnabled;

        private void FindPopups() => dialogue = GetComponentInChildren<DialoguePopup>(includeInactive: true);

        private void FindElements()
        {
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

        private static void SubscrivePopups()
        {
            AbstractPopup.OnAnyShown += HandleAnyPopupShown;
            AbstractPopup.OnAnyClosed += HandleAnyPopupClosed;
        }

        private static void UnsubscribePopups()
        {
            AbstractPopup.OnAnyShown -= HandleAnyPopupShown;
            AbstractPopup.OnAnyClosed -= HandleAnyPopupClosed;

            Instance.DisposeElements();
        }

        private static void HandleAnyPopupShown(AbstractPopup popup) => Instance.InitializeElements(popup.Root);
        private static void HandleAnyPopupClosed(AbstractPopup _) => Instance.DisposeElements();
    }
}