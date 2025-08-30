using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    public abstract class AbstractPopup : AbstractController
    {
        [Header("Texts")]
        [SerializeField, Tooltip("The Title Label name inside your UI Document.")]
        private string titleName = "Title";
        [SerializeField, Tooltip("The Message Label name inside your UI Document.")]
        private string messageName = "Message";

        public Label Title { get; private set; }
        public Label Message { get; private set; }

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

        public void ShowUsingText(
            string message,
            string title = "",
            Action onConfirm = null,
            Action onCancel = null
        )
        {
            Activate();
            SetTexts(title, message);
            SetActions(onConfirm, onCancel);
            FocusButton();
            ShowAnyPopup();
        }

        public void Close()
        {
            DestroyEvents();
            Deactivate();
            CloseAnyPopup();
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

        protected void SetTexts(string title, string message)
        {
            Title.text = title;
            Message.text = message;
        }

        protected void SetActions(Action onConfirm, Action onCancel)
        {
            OnCanceled = onCancel;
            OnConfirmed = onConfirm;
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

        private void ShowAnyPopup() => OnAnyShown?.Invoke(this);
        private void CloseAnyPopup() => OnAnyClosed?.Invoke(this);
    }
}