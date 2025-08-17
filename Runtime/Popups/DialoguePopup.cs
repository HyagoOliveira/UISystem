using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using ActionCode.InputSystem;
#if UNITY_LOCALIZATION
using UnityEngine.Localization.Components;
#endif

namespace ActionCode.UISystem
{
    /// <summary>
    /// Component for a Dialogue Popup, with a localized/normal Title and Message 
    /// using optional callbacks to Confirm and Cancel.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DialoguePopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text message;

#if UNITY_LOCALIZATION
        [Header("Localization")]
        [SerializeField] private LocalizeStringEvent localizedTitle;
        [SerializeField] private LocalizeStringEvent localizedMessage;
#endif

        [Header("Commands")]
        [SerializeField] private GameObject confirmCommand;
        [SerializeField] private GameObject cancelCommand;

        [Header("Input")]
        [SerializeField, Tooltip("The Input Asset where the bellow actions are.")]
        private InputActionAsset input;
        [SerializeField, Tooltip("The confirm input action used to confirm the Popup.")]
        private InputActionPopup confirmInput = new(nameof(input), "UI", "Submit");
        [SerializeField, Tooltip("The cancel input action used to cancel the Popup.")]
        private InputActionPopup cancelInput = new(nameof(input), "UI", "Cancel");

        private event Action OnConfirm;
        private event Action OnCancel;

        private InputAction cancelAction;
        private InputAction confirmAction;

        private void Awake() => FindActions();
        private void OnEnable() => SubscribeEvents();
        private void OnDisable() => UnsubscribeEvents();

        /// <summary>
        /// Shows the dialogue with the given message and title, optionally using localization.
        /// </summary>
        /// <remarks>
        /// If a localization table is provided, the <paramref name="message"/> and <paramref name="title"/> 
        /// are resolved using the specified table. Otherwise, they are displayed as plain
        /// text.
        /// </remarks>
        /// <param name="localizationTable">
        /// The name of the localization table to use for resolving the message and title. 
        /// If null or empty, the message and title are displayed as plain text.
        /// </param>
        /// <param name="message">
        /// The message to display in the dialogue box. If <paramref name="localizationTable"/> is provided,  
        /// this should be a localization key; otherwise, it is treated as plain text.
        /// </param>
        /// <param name="title">
        /// The title of the dialogue box. If <paramref name="localizationTable"/> is provided, this should be a
        /// localization key; otherwise, it is treated as plain text. Defaults to an empty string.
        /// </param>
        /// <param name="onConfirm">An optional callback to invoke when the user confirms the dialogue.</param>
        /// <param name="onCancel">An optional callback to invoke when the user cancels the dialogue.</param>
        public void Show(
            string localizationTable,
            string message,
            string title = "",
            Action onConfirm = null,
            Action onCancel = null
        )
        {
            Show();

            var isLocalized = !string.IsNullOrEmpty(localizationTable);
            if (isLocalized) ShowLocalized(localizationTable, message, title);
            else ShowText(message, title);

            SetActions(onConfirm, onCancel);
        }

        /// <summary>
        /// Closes the dialogue.
        /// </summary>
        public void Close()
        {
            Hide();

            OnConfirm = null;
            OnCancel = null;
        }

        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);

        private void FindActions()
        {
            cancelAction = input.FindAction(cancelInput.GetPath());
            confirmAction = input.FindAction(confirmInput.GetPath());
        }

        private void SetActions(Action onConfirm, Action onCancel)
        {
            OnCancel = onCancel;
            OnConfirm = onConfirm;

            var hasCancel = OnCancel != null;
            var hasConfirm = OnConfirm != null;

            cancelCommand.SetActive(true);
            confirmCommand.SetActive(hasConfirm);

            //confirmAction.set

            confirmAction.actionMap.Enable();

            OnCancel += Close;
            OnConfirm += Close;
        }

        private void ShowText(string message, string title)
        {
            this.title.text = title;
            this.message.text = message;
        }

        private void ShowLocalized(string table, string message, string title = "")
        {
#if UNITY_LOCALIZATION
            localizedMessage.StringReference = new(table, message);
            if (!string.IsNullOrEmpty(message)) localizedTitle.StringReference = new(table, title);
#endif
        }

        private void SubscribeEvents()
        {
            cancelAction.performed += HandleCancelPerformed;
            confirmAction.performed += HandleConfirmPerformed;
        }

        private void UnsubscribeEvents()
        {
            cancelAction.performed -= HandleCancelPerformed;
            confirmAction.performed -= HandleConfirmPerformed;
        }

        private void HandleCancelPerformed(InputAction.CallbackContext _) => OnCancel?.Invoke();
        private void HandleConfirmPerformed(InputAction.CallbackContext _) => OnConfirm?.Invoke();
    }
}