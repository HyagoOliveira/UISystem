using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract component for a Pause Menu.
    /// Use any <see cref="PauseScreen"/> component.
    /// </summary>
    public abstract class AbstractPauseMenu : MonoBehaviour// AbstractMenu
    {
        [Space]
        public PauseScreen pauseScreen;

        protected void SubscribeEvents()
        {
            pauseScreen.OnContinueClicked += HandleContinueClicked;
            pauseScreen.OnMainMenuClicked += HandleMainMenuClicked;
            pauseScreen.OnQuitClicked += HandleQuitClicked;
        }

        protected void UnsubscribeEvents()
        {
            pauseScreen.OnContinueClicked -= HandleContinueClicked;
            pauseScreen.OnMainMenuClicked -= HandleMainMenuClicked;
            pauseScreen.OnQuitClicked -= HandleQuitClicked;
        }

        /*protected override async void OnCancel()
        {
            var shouldSelectContinueButton = !pauseScreen.Continue.IsFocused();
            if (shouldSelectContinueButton)
            {
                SetSendNavigationEvents(false);
                pauseScreen.Continue.Focus();
                await AwaitableUtility.WaitForSecondsRealtimeAsync(0.2f);
            }

            HandleContinueClicked();
        }*/

        protected abstract void HandleContinueClicked();
        protected abstract void HandleMainMenuClicked();
        protected virtual void HandleQuitClicked() => Popups.ShowQuitGameDialogue();
    }
}