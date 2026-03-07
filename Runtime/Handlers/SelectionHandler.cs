using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Handler for a selectable element.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SelectionHandler : MonoBehaviour, ISelectable, ICancelable
    {
        public event Action OnSelected;
        public event Action OnCanceled;

        // Triggered when Mouse hovers it
        public void OnPointerEnter(PointerEventData _)
        {
            var eventSystem = EventSystem.current;
            var canSelectGO = eventSystem && eventSystem.currentSelectedGameObject != gameObject;
            if (canSelectGO) eventSystem.SetSelectedGameObject(gameObject);

            // OnSelect will be triggered by EventSystem when SetSelectedGameObject is called
        }

        // Triggered when Gamepad/Keyboard navigates or Mouse sets selection
        public void OnSelect(BaseEventData _) => HandleSelection();

        // Triggered when the cancel button (typically the Escape key or Gamepad East Button) is pressed
        public void OnCancel(BaseEventData eventData) => HandleCancelation();

        private void HandleSelection() => OnSelected?.Invoke();
        private void HandleCancelation() => OnCanceled?.Invoke();
    }
}