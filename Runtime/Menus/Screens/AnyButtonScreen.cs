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
    /// It moves to the <see cref="AbstractMainMenuScreen"/> when any key is pressed.
    /// </summary>
    public sealed class AnyButtonScreen : AbstractMenuScreen
    {
        [Header("Transition")]
        [SerializeField, Tooltip("Whether can go back to this screen from Main Menu.")]
        private bool canGoBack;
        [SerializeField, Tooltip("Whether to fade the screen when opening the next screen.")]
        private bool fadeScreen;
        [SerializeField, Tooltip("The next menu screen to open when any key is pressed.")]
        private string nextScreenName = "MainMenuScreen";

        [Header("Animations")]
        [SerializeField, Tooltip("The optional animation to play when idle.")]
        private AbstractAnimator idleAnimator;
        [SerializeField, Tooltip("The optional animation to play when clicked.")]
        private AbstractAnimator clickedAnimator;

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

            _ = Menu.OpenScreenAsync(nextScreenName, canGoBack, fadeScreen);
        }

        private static bool IsValidDevicePress(InputDevice device) => device is not Mouse mouse || mouse.IsInsideGameView();
    }
}