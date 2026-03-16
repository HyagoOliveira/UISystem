using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Custom <see cref="Selectable"/> implementation with <see cref="OnSelected"/> callback and 
    /// color fade transitions for Background and Label components.
    /// </summary>
    /// <remarks>
    /// Use <see cref="OnSelected"/> to get notified when this object is selected.<br/>
    /// </remarks>
    [DisallowMultipleComponent]
    public class ActionSelectable : Selectable, ISelectable
    {
        [field: Header("UI Components")]
        [field: SerializeField, Tooltip("The background for this UI.")]
        public Graphic Background { get; private set; }
        [field: SerializeField, Tooltip("The Label component for this UI.")]
        public Label Label { get; private set; }

        [field: Space]
        [field: SerializeField, Min(0), Tooltip("The color change duration for the UI Components.")]
        public float FadeDuration { get; set; } = 0.1f;
        [field: SerializeField]
        public SelectableColorData Colors { get; set; }

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

            SetupBackground();
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

            var fadeDuration = instant ? 0f : FadeDuration;

            if (Label) FadeColor(Label.target, GetColor(Label.Colors, state), fadeDuration);
            if (Background && Colors) FadeColor(Background, GetColor(Colors, state), fadeDuration);
        }

        protected virtual void HandleSelection() => OnSelected?.Invoke();
        protected virtual void HandleUnselection() => OnUnselected?.Invoke();

        private void SetupBackground()
        {
            Background = GetComponentInChildren<Graphic>();
            if (Background == null) return;

            Background.color = Color.white;
            Background.raycastTarget = true;
        }

        public static void FadeColor(Graphic target, Color color, float duration) => target.CrossFadeColor(
            color,
            duration,
            ignoreTimeScale: true,
            useAlpha: true
        );

        // Cannot create this function inside SelectableColorData since SelectionState is a protected enum
        private static Color GetColor(SelectableColorData target, SelectionState state) => state switch
        {
            SelectionState.Normal => target.Normal,
            SelectionState.Highlighted => target.Highlighted,
            SelectionState.Pressed => target.Pressed,
            SelectionState.Selected => target.Selected,
            SelectionState.Disabled => target.Disabled,
            _ => target.Normal
        };
    }
}