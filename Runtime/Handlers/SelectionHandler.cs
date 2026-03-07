using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Handler for a selectable element.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SelectionHandler : MonoBehaviour, ISelectable
    {
        public event Action OnSelected;

        public void OnPointerEnter(PointerEventData _) => HandleSelection();

        private void HandleSelection()
        {
            var eventSystem = EventSystem.current;
            if (eventSystem) eventSystem.SetSelectedGameObject(gameObject);
            OnSelected?.Invoke();
        }
    }
}