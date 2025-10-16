using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Confirmation Popup with a single Confirm button.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ConfirmationPopup : AbstractPopup
    {
        [Header("Buttons")]
        [SerializeField, Tooltip("The Confirm Button name inside your UI Document.")]
        private string confirmButtonName = "Confirm";

        public Button ConfirmButton { get; private set; }

        protected override void FindReferences()
        {
            base.FindReferences();
            ConfirmButton = Root.Find<Button>(confirmButtonName);
        }

        protected override void FocusButton() => ConfirmButton.Focus();

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            ConfirmButton.clicked += Confirm;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
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

        private void HandleNavigationCancelEvent(NavigationCancelEvent _) => Confirm();
    }
}