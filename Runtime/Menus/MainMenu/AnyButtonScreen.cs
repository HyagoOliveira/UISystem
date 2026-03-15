using PrimeTween;
using UnityEngine;
using ActionCode.InputSystem;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Screen for the Any Button section from a Main Menu.
    /// This is the first screen from the Main Menu and will wait 
    /// for any input to open the <see cref="nextScreen"/>.
    /// </summary>
    /// <remarks>
    /// If set, the <see cref="idleAnimation"/> will be played after the screen is fully opened.
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AnyButtonPressedListener))]
    public sealed class AnyButtonScreen : BaseScreen
    {
        [Space]
        public string nextScreen = "MainMenuOptionsScreen";
        public AbstractAnimation idleAnimation;
        [SerializeField] private AnyButtonPressedListener buttonListener;

        private void Reset()
        {
            buttonListener = GetComponent<AnyButtonPressedListener>();
            buttonListener.disableAfterEvent = false;
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            buttonListener.OnAnyButtonPressed.AddListener(HandleAnyButtonPressed);
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            buttonListener.OnAnyButtonPressed.RemoveListener(HandleAnyButtonPressed);
        }

        public override void FinishOpen()
        {
            base.FinishOpen();
            if (idleAnimation) idleAnimation.Play();
        }

        public override void StartClose()
        {
            base.StartClose();
            if (idleAnimation) idleAnimation.Stop();
        }

        private void HandleAnyButtonPressed()
        {
            if (IsOpening) return;

            buttonListener.enabled = false;

            Menu.Audio.PlaySubmition();
            OpenScreen(nextScreen);
        }
    }
}