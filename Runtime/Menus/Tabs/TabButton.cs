using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Button component for a Tab.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TabButton : MonoBehaviour,
        IPointerClickHandler,
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField, Tooltip("All UI transitions")]
        private AbstractTransition[] transitions = new AbstractTransition[0];

        public event Action<uint> OnSwitched;

        public uint Index { get; private set; }
        public bool IsSelected { get; private set; }

        private void Reset() => transitions = GetComponentsInChildren<AbstractTransition>();

        internal void Initialize(uint index) => Index = index;

        internal void Select()
        {
            IsSelected = true;
            DoStateTransition(SelectionState.Selected);
        }

        internal void Unselect()
        {
            IsSelected = false;
            DoStateTransition(SelectionState.Normal);
        }

        public bool IsAvailable() => isActiveAndEnabled;

        // Triggered when Mouse clicks on it
        public void OnPointerClick(PointerEventData eventData)
        {
            var isLeftClick = eventData.button == PointerEventData.InputButton.Left;
            if (isLeftClick) Switch();
        }

        // Triggered when Mouse enter hovering over it
        public void OnPointerEnter(PointerEventData _) => DoStateTransition(SelectionState.Highlighted);

        // Triggered when Mouse exit hovering over it
        public void OnPointerExit(PointerEventData _) => UpdateSelectionTransition();

        public void OnPointerDown(PointerEventData eventData)
        {
            var isLeftClick = eventData.button == PointerEventData.InputButton.Left;
            if (isLeftClick) DoStateTransition(SelectionState.Pressed);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            var isLeftClick = eventData.button == PointerEventData.InputButton.Left;
            if (isLeftClick) UpdateSelectionTransition();
        }

        private void UpdateSelectionTransition()
        {
            var state = IsSelected ? SelectionState.Selected : SelectionState.Normal;
            DoStateTransition(state);
        }

        private void Switch()
        {
            if (!IsAvailable()) return;

            Select();
            OnSwitched?.Invoke(Index);
        }

        private void DoStateTransition(SelectionState state)
        {
            if (!IsAvailable()) return;

            foreach (var transition in transitions)
            {
                transition.Transit(state, instant: false);
            }
        }
    }
}