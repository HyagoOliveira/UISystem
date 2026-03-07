using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Handler for a submittable button.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SelectionHandler))]
    public sealed class ButtonHandler : MonoBehaviour, ISubmitable
    {
        public event Action OnSubmitted;

        public void OnSubmit(BaseEventData _) => HandleSubmition();

        public void OnPointerClick(PointerEventData evt)
        {
            var canSubmit = evt.button == PointerEventData.InputButton.Left;
            if (canSubmit) HandleSubmition();
        }

        private void HandleSubmition() => OnSubmitted?.Invoke();
    }
}
