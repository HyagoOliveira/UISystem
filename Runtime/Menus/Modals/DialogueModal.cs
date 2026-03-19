using System;
using UnityEngine;
using UnityEngine.UI;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Dialogue Modal Screen with Title, Message and Confirm/Cancel buttons.
    /// </summary>
    [DisallowMultipleComponent]
    public class DialogueModal : BaseScreen
    {
        [Header("Labels")]
        [SerializeField] private Label title;
        [SerializeField] private Label message;

        [Header("Buttons")]
        [SerializeField] private ActionButton confirmButton;
        [SerializeField] private ActionButton cancelButton;

        private event Action OnCanceled;
        private event Action OnConfirmed;

        private void Awake() => SetupButtonNavigations();

        public void Show(
            LocalizedString message,
            LocalizedString title,
            Action onConfirm,
            Action onCancel
        )
        {
            SetActions(onConfirm, onCancel);
            SetLocalization(message, title);
        }

        public void Close()
        {
            Dispose();
            cancelButton.OnCancel(null); // Execute ModalMenu.OnCancel
        }

        internal void SetCancelButtonVisibility(bool isVisible)
        {
            cancelButton.interactable = isVisible;
            cancelButton.gameObject.SetActive(isVisible);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            confirmButton.OnClicked += HandleConfirnButtonClicked;
            cancelButton.OnClicked += HandleCancelButtonClicked;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            confirmButton.OnClicked -= HandleConfirnButtonClicked;
            cancelButton.OnClicked -= HandleCancelButtonClicked;

            Dispose();
        }

        private void Dispose()
        {
            OnCanceled = null;
            OnConfirmed = null;
        }

        private void HandleConfirnButtonClicked()
        {
            OnConfirmed?.Invoke();
            Close();
        }

        private void HandleCancelButtonClicked()
        {
            OnCanceled?.Invoke();
            Close();
        }

        private void SetActions(Action onConfirm, Action onCancel = null)
        {
            OnCanceled = onCancel;
            OnConfirmed = onConfirm;
        }

        private void SetLocalization(LocalizedString locMessage, LocalizedString locTitle)
        {
            locTitle.UpdateLocalization(title);
            locMessage.UpdateLocalization(message);
        }

        private void SetupButtonNavigations()
        {
            SetButtonHorizontalNavigation(confirmButton, cancelButton);
            SetButtonHorizontalNavigation(cancelButton, confirmButton);
        }

        private static void SetButtonHorizontalNavigation(ActionButton button, ActionButton navigateTo)
        {
            var navigation = button.navigation;

            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnLeft = navigateTo;
            navigation.selectOnRight = navigateTo;

            button.navigation = navigation;
        }
    }
}