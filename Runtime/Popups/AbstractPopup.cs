using System;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_LOCALIZATION
#endif

namespace ActionCode.UISystem
{
    public abstract class AbstractPopup : AbstractController
    {
        [Header("Animations")]
        [SerializeField] private AbstractAnimator showAnimation;
        [SerializeField] private AbstractAnimator closeAnimation;

        [Header("Texts")]
        [SerializeField, Tooltip("The Title Label name inside your UI Document.")]
        private string titleName = "Title";
        [SerializeField, Tooltip("The Message Label name inside your UI Document.")]
        private string messageName = "Message";

        /// <summary>
        /// The Title Label inside your UI Document.
        /// </summary>
        public Label Title { get; private set; }

        /// <summary>
        /// Title Message Label inside your UI Document.
        /// </summary>
        public Label Message { get; private set; }

        internal ButtonClickAudioPlayer ButtonClickPlayer { get; set; }

        /// <summary>
        /// Global event fired when any Popup is shown.
        /// <para>The given param is the popup instance.</para>
        /// </summary>
        public static event Action<AbstractPopup> OnAnyShown;

        /// <summary>
        /// Global event fired when any Popup is closed, by confirming or canceling it.
        /// <para>The given param is the popup instance.</para>
        /// </summary>
        public static event Action<AbstractPopup> OnAnyClosed;

        /// <summary>
        /// Event fired when canceling the popup.
        /// </summary>
        public event Action OnCanceled;

        /// <summary>
        /// Event fired when confirming the popup.
        /// </summary>
        public event Action OnConfirmed;

        /// <summary>
        /// The default sorting order for all popups.
        /// </summary>
        public const float SORTING_ORDER = 10F;

        protected override void Reset()
        {
            base.Reset();
            Document.sortingOrder = SORTING_ORDER;
        }

        /// <summary>
        /// Shows the popup using the given parameters.
        /// </summary>
        /// <param name="message">The popup message using simple text.</param>
        /// <param name="title">An optional popup title using simple text.</param>
        /// <param name="onConfirm">An optional action to execute when popup is confirmed.</param>
        /// <param name="onCancel">An optional action to execute when popup is canceled.</param>
        public void Show(
            string message,
            string title = "",
            Action onConfirm = null,
            Action onCancel = null
        )
        {
            Activate();
            SetTexts(title, message);
            ShowAsync(onConfirm, onCancel);
        }

        /// <summary>
        /// Shows the localized popup using the given parameters.
        /// <para>Requires the Unity Localization package.</para>
        /// </summary>
        /// <param name="tableId">
        /// The table to find the localizations. 
        /// If empty, it will use the <see cref="Show(string, string, Action, Action)"/> function.
        /// </param>
        /// <param name="messageId">The popup localized message id.</param>
        /// <param name="titleId">The popup localized tile id.</param>
        /// <param name="onConfirm">An optional action to execute when popup is confirmed.</param>
        /// <param name="onCancel">An optional action to execute when popup is canceled.</param>
        public void Show(
            string tableId,
            string messageId,
            string titleId = "",
            Action onConfirm = null,
            Action onCancel = null
        )
        {
            var hasInvalidTableId = string.IsNullOrEmpty(tableId);
            if (hasInvalidTableId)
            {
                Show(messageId, titleId, onConfirm, onCancel);
                return;
            }

            Activate();
            SetTexts(tableId, titleId, messageId);
            ShowAsync(onConfirm, onCancel);
        }

        /// <summary>
        /// Closes the popup.
        /// </summary>
        public void Close()
        {
            DestroyEvents();
            CloseAsync();
        }

        protected abstract void FocusButton();
        protected abstract void FindButtons();

        protected override void FindReferences()
        {
            base.FindReferences();

            Title = Root.Find<Label>(titleName);
            Message = Root.Find<Label>(messageName);

            FindButtons();
        }

        protected virtual void Confirm()
        {
            OnConfirmed?.Invoke();
            Close();
        }

        protected virtual void Cancel()
        {
            OnCanceled?.Invoke();
            Close();
        }

        protected virtual void DestroyEvents() => SetActions(null, null);

        private void SetTexts(string title, string message)
        {
            Title.text = title;
            Message.text = message;
        }

        private void SetTexts(string tableId, string titleId, string messageId)
        {
            Title.UpdateLocalization(tableId, titleId);
            Message.UpdateLocalization(tableId, messageId);
        }

        private void SetActions(Action onConfirm, Action onCancel)
        {
            OnCanceled = onCancel;
            OnConfirmed = onConfirm;
        }

        private async void ShowAsync(Action onConfirm, Action onCancel)
        {
            AbstractMenu.SetSendNavigationEvents(false);

            if (showAnimation) await showAnimation.PlayAsync();

            SetActions(onConfirm, onCancel);
            FocusButton();
            ShowAnyPopup();

            AbstractMenu.SetSendNavigationEvents(true);
        }

        private async void CloseAsync()
        {
            CloseAnyPopup();
            AbstractMenu.SetSendNavigationEvents(false);

            if (ButtonClickPlayer)
            {
                await ButtonClickPlayer.WaitSubmitSoundAsync();
                ButtonClickPlayer.PlayCancelSound();
            }

            if (closeAnimation) await closeAnimation.PlayAsync();
            Deactivate();

            AbstractMenu.SetSendNavigationEvents(true);
        }

        private void ShowAnyPopup() => OnAnyShown?.Invoke(this);
        private void CloseAnyPopup() => OnAnyClosed?.Invoke(this);
    }
}