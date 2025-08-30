using UnityEngine;
using UnityEngine.UIElements;

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

        protected override void FindButtons()
        {
            CancelButton = Root.Find<Button>(cancelButtonName);
            ConfirmButton = Root.Find<Button>(confirmButtonName);
        }

        protected override void FocusButton() => ConfirmButton.Focus();

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
    }
}