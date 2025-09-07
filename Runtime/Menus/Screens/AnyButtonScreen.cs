using System;
using ActionCode.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityInputSystem = UnityEngine.InputSystem.InputSystem;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Controller for a UI Toolkit "Press Any Button" Screen.
    /// Use the <see cref="OnAnyClicked"/> event when any key is pressed.
    /// </summary>
    public sealed class AnyButtonScreen : AbstractMenuScreen
    {
        [Tooltip("Whether can go back to this screen from Main Menu.")]
        public bool canGoBack;

        [Header("Animations")]
        [SerializeField, Tooltip("The optional animation to play when idle.")]
        private AbstractAnimator idleAnimator;
        [SerializeField, Tooltip("The optional animation to play when clicked.")]
        private AbstractAnimator clickedAnimator;

        /// <summary>
        /// Event fired when any button is pressed.
        /// </summary>
        public event Action OnAnyClicked;

        private IDisposable anyButtonPressListener;

        public override void Activate()
        {
            base.Activate();
            if (idleAnimator) idleAnimator.Play();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeAnyButtonPressEvent();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            UnsubscribeAnyButtonPressEvent();
        }

        private void SubscribeAnyButtonPressEvent() => anyButtonPressListener = UnityInputSystem.onAnyButtonPress.Call(HandleAnyButtonPressed);
        private void UnsubscribeAnyButtonPressEvent() => anyButtonPressListener?.Dispose();

        private void HandleAnyButtonPressed(InputControl input)
        {
            if (IsValidDevicePress(input.device))
            {
                PressButton();
                UnsubscribeAnyButtonPressEvent();
            }
        }

        private async void PressButton()
        {
            Menu.ButtonClickPlayer.PlaySubmitSound();

            if (idleAnimator) idleAnimator.Stop();
            if (clickedAnimator) await clickedAnimator.PlayAsync();

            OnAnyClicked?.Invoke();
        }

        private static bool IsValidDevicePress(InputDevice device) => device is not Mouse mouse || mouse.IsInsideGameView();
    }
}