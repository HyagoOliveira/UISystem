using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Field for a Toggle.
    /// </summary>
    /// <remarks>
    /// Player can navigate between values using the gamepad/keyboard left/right or submit buttons.<br/>
    /// </remarks>
    [DisallowMultipleComponent]
    public sealed class ToggleField : AbstractField<bool>, IClickable
    {
        [Space]
        [Tooltip("The toggle initial value.")]
        public bool initialValue;

        [Header("DISPLAYS")]
        [SerializeField] private GameObject enabledDisplay;
        [SerializeField] private GameObject disabledDisplay;

        public event Action OnClicked;

        private AudioHandler audioHandler;

        protected override void Awake()
        {
            base.Awake();
            if (IsRunning())
            {
                audioHandler = GetComponentInParent<AudioHandler>();
                if (initialValue) SetValueWithoutNotify(initialValue);
            }
        }

        /// <summary>
        /// Toggles the value.
        /// </summary>
        public void Toggle() => Value = !Value;

        protected override void ChangeValue(bool value)
        {
            UpdateDisplay(value);
            base.ChangeValue(value);
        }

        public override void SetValueWithoutNotify(bool value)
        {
            UpdateDisplay(value);
            base.SetValueWithoutNotify(value);
        }

        private void UpdateDisplay(bool isEnabled)
        {
            enabledDisplay.SetActive(isEnabled);
            disabledDisplay.SetActive(!isEnabled);
        }

        #region INTERFACES
        // Triggered when Mouse clicks on it
        public void OnPointerClick(PointerEventData evt)
        {
            var isLeftClick = evt.button == PointerEventData.InputButton.Left;
            if (isLeftClick) Press();
        }

        // Triggered when Gamepad/Keyboard submits or Mouse clicks on it
        public void OnSubmit(BaseEventData _) => Press();

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);

            switch (eventData.moveDir)
            {
                case MoveDirection.Right:
                case MoveDirection.Left:
                    Toggle();
                    if (audioHandler) audioHandler.PlaySubmition();
                    break;
            }
        }

        private void Press()
        {
            if (!IsAvailable()) return;

            Toggle();
            OnClicked?.Invoke();
        }
        #endregion
    }
}
