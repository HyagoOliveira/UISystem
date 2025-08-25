using System;
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
        /// <summary>
        /// Event fired when any button is pressed.
        /// </summary>
        public event Action OnAnyClicked;

        private IDisposable anyButtonPressListener;

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            anyButtonPressListener = UnityInputSystem.onAnyButtonPress.CallOnce(HandleAnyButtonPressed);
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            anyButtonPressListener?.Dispose();
        }

        private async void HandleAnyButtonPressed(InputControl _)
        {
            await Menu.ButtonClickPlayer.PlaySubmitSoundAndWaitAsync();
            OnAnyClicked?.Invoke();
        }
    }
}