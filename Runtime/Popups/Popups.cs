using UnityEngine;
using UnityEngine.UIElements;

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

        [Header("Popups")]
        [SerializeField, Tooltip("The local Dialogue Popup.")]
        private DialoguePopup dialogue;

        public MenuData Data => data;
        public AudioSource Audio => source;
        public ElementHighlighter Highlighter => highlighter;
        public ElementFocusAudioPlayer FocusPlayer => focusPlayer;
        public ButtonClickAudioPlayer ButtonClickPlayer => buttonClickPlayer;

        public AbstractPopup Current { get; private set; }

        /// <summary>
        /// The global Dialogue Popup.
        /// </summary>
        public static DialoguePopup Dialogue => Instance.dialogue;

        private static Popups Instance { get; set; }

        private void Reset()
        {
            FindComponents();
            FindPopups();
        }

        private void Awake() => Instance = this;
        private void OnEnable() => SubscriveEvents();
        private void OnDisable() => UnsubscribeEvents();
        private void OnDestroy()
        {
            Instance = null;
            DisposeElements();
        }

        private void FindPopups() => dialogue = GetComponentInChildren<DialoguePopup>(includeInactive: true);

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