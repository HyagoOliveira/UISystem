using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Custom <see cref="Selectable"/> implementation with <see cref="OnSelected"/> callback and 
    /// color fade transitions for Background and Label UI components.
    /// </summary>
    /// <remarks>
    /// Use <see cref="OnSelected"/> to get notified when this object is selected.<br/>
    /// Set the <see cref="background"/> and <see cref="label"/> components to fade 
    /// colors when transitioning between states.
    /// </remarks>
    [DisallowMultipleComponent]
    public class ActionSelectable : Selectable, ISelectable
    {
        [Space]
        [Min(0f), Tooltip("The color change duration for the UI Components.")]
        public float fadeDuration = 0.1f;

        [Header("UI Components")]
        [Tooltip("The background for this UI.")]
        public SelectableTarget background = new();
        [Tooltip("The label for this UI.")]
        public SelectableTarget label = new();

        public event Action OnSelected;

        protected override void Reset()
        {
            base.Reset();
            transition = Transition.None;
            SetupBackgroundTarget();
        }

        // Triggered when Mouse hovers over it
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            Select();
            // EventSystem.SetSelectedGameObject() (inside Select()) will trigger OnSelect
        }

        // Triggered when Gamepad/Keyboard navigates or Mouse sets selection
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            HandleSelection();
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (transition != Transition.None)
            {
                base.DoStateTransition(state, instant);
                return;
            }

            if (!IsActive()) return;

            var backgroundColor = GetColor(background, state);
            var labelColor = GetColor(label, state);
            var fadeDuration = instant ? 0f : this.fadeDuration;

            background.FadeTarget(backgroundColor, fadeDuration);
            label.FadeTarget(labelColor, fadeDuration);
        }

        protected virtual void HandleSelection() => OnSelected?.Invoke();

        protected virtual void SetupBackgroundTarget()
        {
            background.target = GetComponentInChildren<Graphic>();
            if (!background.HasTarget()) return;

            background.target.raycastTarget = true;
            background.SetColors(background.target.color);
        }

        // Cannot create this function inside SelectableTarget since SelectionState is a protected enum
        private static Color GetColor(SelectableTarget target, SelectionState state) => state switch
        {
            SelectionState.Normal => target.normalColor,
            SelectionState.Highlighted => target.selectedColor,
            SelectionState.Pressed => target.pressedColor,
            SelectionState.Selected => target.selectedColor,
            SelectionState.Disabled => target.disabledColor,
            _ => target.normalColor
        };
    }
}