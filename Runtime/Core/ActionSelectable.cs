using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Custom <see cref="Selectable"/> implementation with <see cref="OnSelected"/> callback 
    /// and Transitions components.
    /// </summary>
    /// <remarks>
    /// Use <see cref="OnSelected"/> to get notified when this object is selected.<br/>
    /// Add any component implementing <see cref="AbstractTransition"/> into this GameObject 
    /// hierarchy to get notify when this Selectable state changes.
    /// </remarks>
    [DisallowMultipleComponent]
    public class ActionSelectable : Selectable, ISelectable
    {
        [field: Space]
        [field: SerializeField, Tooltip("The Label component for this UI.")]
        public Label Label { get; private set; }
        [field: SerializeField] public AbstractTransition[] Transitions { get; set; } = new AbstractTransition[0];

        public event Action OnSelected;

        /// <summary>
        /// Event fired when this object is unselected.
        /// </summary>
        public event Action OnUnselected;

        protected override void Reset()
        {
            base.Reset();

            transition = Transition.None;
            Label = GetComponentInChildren<Label>();
            targetGraphic = GetComponentInChildren<Graphic>();
            Transitions = GetComponentsInChildren<AbstractTransition>();

            CheckForTargetGraphic();
        }

        public bool IsAvailable() => IsActive() && IsInteractable();

        // Triggered when Mouse hovers over it
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (IsAvailable()) Select();
            // EventSystem.SetSelectedGameObject will trigger OnSelect
        }

        // Triggered when Gamepad/Keyboard navigates or Mouse sets selection
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            if (IsAvailable()) HandleSelection();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            if (IsAvailable()) HandleUnselection();
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (transition != Transition.None)
            {
                base.DoStateTransition(state, instant);
                return;
            }

            if (!IsActive()) return;

            var selectionState = GetSelectionState(state);

            foreach (var transition in Transitions)
            {
                transition.Transit(selectionState, instant);
            }
        }

        protected virtual void HandleSelection() => OnSelected?.Invoke();
        protected virtual void HandleUnselection() => OnUnselected?.Invoke();

        public static void FadeColor(Graphic target, Color color, float duration) => target.CrossFadeColor(
            color,
            duration,
            ignoreTimeScale: true,
            useAlpha: true
        );

        private void CheckForTargetGraphic()
        {
            if (targetGraphic != null) return;
            Debug.LogWarning($"{gameObject.name} must contain any implementation of Graphic component in order to work.");
        }

        // Converts UnityUI.SelectionState into ActionCode.UISystem.SelectionState
        // since Unity's SelectionState is a protected enum
        private static UISystem.SelectionState GetSelectionState(SelectionState state) => state switch
        {
            SelectionState.Normal => UISystem.SelectionState.Normal,
            SelectionState.Highlighted => UISystem.SelectionState.Highlighted,
            SelectionState.Pressed => UISystem.SelectionState.Pressed,
            SelectionState.Selected => UISystem.SelectionState.Selected,
            SelectionState.Disabled => UISystem.SelectionState.Disabled,
            _ => UISystem.SelectionState.Normal,
        };
    }
}