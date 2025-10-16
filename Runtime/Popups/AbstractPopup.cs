using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract controller for a UI Toolkit Popup document.
    /// </summary>
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

        /// <summary>
        /// Global event fired when any Popup is starting to shown, before executing the show animation.
        /// <para>The given param is the popup instance.</para>
        /// </summary>
        public static event Action<AbstractPopup> OnAnyStartShow;

        /// <summary>
        /// Global event fired when any Popup is finish to shown, after executing the show animation.
        /// <para>The given param is the popup instance.</para>
        /// </summary>
        public static event Action<AbstractPopup> OnAnyFinishShow;

        /// <summary>
        /// Global event fired when any Popup is starting to close (by confirming or canceling), before executing the close animation.
        /// <para>The given param is the popup instance.</para>
        /// </summary>
        public static event Action<AbstractPopup> OnAnyStartClose;

        /// <summary>
        /// Global event fired when any Popup is finished to close (by confirming or canceling), after executing the close animation.
        /// <para>The given param is the popup instance.</para>
        /// </summary>
        public static event Action<AbstractPopup> OnAnyFinishClose;

        /// <summary>
        /// Event fired when canceling this popup.
        /// </summary>
        public event Action OnCanceled;

        /// <summary>
        /// Event fired when confirming this popup.
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

        private void OnDestroy()
        {
            OnAnyStartShow = null;
            OnAnyFinishClose = null;
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
        /// If empty, it will use the <see cref="Show(string, string, Action, Action)"/> function to show simple text.
        /// </param>
        /// <param name="messageId">The popup localized message id.</param>
        /// <param name="titleId">The popup localized tile id.</param>
        /// <param name="onConfirm"><inheritdoc cref="Show(string, string, Action, Action)" path="/param[@name='onConfirm']"/></param>
        /// <param name="onCancel"><inheritdoc cref="Show(string, string, Action, Action)" path="/param[@name='onCancel']"/></param>
        public void Show(
            string tableId,
            string messageId,
            string titleId = "",
            Action onConfirm = null,
            Action onCancel = null
        )
        {
            Activate();
            SetTexts(tableId, titleId, tableId, messageId);
            ShowAsync(onConfirm, onCancel);
        }

        /// <summary>
        /// Shows the localized popup using the given parameters.
        /// <para>
        /// Requires the Unity Localization package to show the correct localization.
        /// Otherwise it will show the fallback text.
        /// </para>
        /// </summary>
        /// <param name="message">The popup localized message with optional fallback text.</param>
        /// <param name="title">The popup localized title  with optional fallback text.</param>
        /// <param name="onConfirm"><inheritdoc cref="Show(string, string, Action, Action)" path="/param[@name='onConfirm']"/></param>
        /// <param name="onCancel"><inheritdoc cref="Show(string, string, Action, Action)" path="/param[@name='onCancel']"/></param>
        public void Show(
            LocalizedString message,
            LocalizedString title,
            Action onConfirm = null,
            Action onCancel = null
        )
        {
            Activate();
            SetTexts(title, message);
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

        public float GetShowAnimationTime() => showAnimation ? showAnimation.GetDuration() : 0.1f;
        public float GetCloseAnimationTime() => closeAnimation ? closeAnimation.GetDuration() : 0.1f;

        protected override void FindReferences()
        {
            base.FindReferences();

            Title = Root.Find<Label>(titleName);
            Message = Root.Find<Label>(messageName);
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

        protected virtual void OnFinishShow() { }
        protected virtual void OnStartClose() { }
        protected virtual void DestroyEvents() => SetActions(null, null);

        protected void SetTexts(string title, string message)
        {
            Title.text = title;
            Message.text = message;
        }

        protected void SetTexts(
            string titleTableId, string titleId,
            string messageTableId, string messageId)
        {
            Title.UpdateLocalization(titleTableId, titleId);
            Message.UpdateLocalization(messageTableId, messageId);
        }

        protected async void SetTexts(LocalizedString title, LocalizedString message)
        {
            await Title.UpdateLocalization(title);
            await Message.UpdateLocalization(message);
        }

        protected void SetActions(Action onConfirm, Action onCancel)
        {
            OnCanceled = onCancel;
            OnConfirmed = onConfirm;
        }

        protected async void ShowAsync(Action onConfirm, Action onCancel)
        {
            OnAnyStartShow?.Invoke(this);
            MenuController.SetSendNavigationEvents(false);

            if (showAnimation) await showAnimation.PlayAsync();

            SetActions(onConfirm, onCancel);
            Focus();

            MenuController.SetSendNavigationEvents(true);
            OnAnyFinishShow?.Invoke(this);
            OnFinishShow();
        }

        private async void CloseAsync()
        {
            OnStartClose();
            OnAnyStartClose?.Invoke(this);
            MenuController.SetSendNavigationEvents(false);

            if (closeAnimation) await closeAnimation.PlayAsync();
            Deactivate();

            MenuController.SetSendNavigationEvents(Popups.CanHaveNavigation());
            OnAnyFinishClose?.Invoke(this);
        }
    }
}