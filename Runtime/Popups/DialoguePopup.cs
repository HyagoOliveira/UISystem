using UnityEngine;
using UnityEngine.UIElements;
using ActionCode.AwaitableSystem;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Dialogue Popup with Confirm and Cancel buttons.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DialoguePopup : AbstractPopup
    {
        [Header("Buttons")]
        [SerializeField, Tooltip("The Cancel Button name inside your UI Document.")]
        private string cancelButtonName = "Cancel";
        [SerializeField, Tooltip("The Confirm Button name inside your UI Document.")]
        private string confirmButtonName = "Confirm";

        public Button CancelButton { get; private set; }
        public Button ConfirmButton { get; private set; }

        public override void Focus() => ConfirmButton.Focus();

        protected override void FindReferences()
        {
            base.FindReferences();

            CancelButton = Root.Find<Button>(cancelButtonName);
            ConfirmButton = Root.Find<Button>(confirmButtonName);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            CancelButton.clicked += Cancel;
            ConfirmButton.clicked += Confirm;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            CancelButton.clicked -= Cancel;
            ConfirmButton.clicked -= Confirm;
        }

        protected override void OnFinishShow()
        {
            base.OnFinishShow();
            Root.RegisterCallback<NavigationCancelEvent>(HandleNavigationCancelEvent);
        }

        protected override void OnStartClose()
        {
            base.OnStartClose();
            Root.UnregisterCallback<NavigationCancelEvent>(HandleNavigationCancelEvent);
        }

        private async void HandleNavigationCancelEvent(NavigationCancelEvent _)
        {
            MenuController.SetSendNavigationEvents(false);

            if (!CancelButton.IsFocused())
            {
                CancelButton.Focus();
                await AwaitableUtility.WaitForSecondsRealtimeAsync(0.2f);
            }

            Cancel();
        }
    }
}