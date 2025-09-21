using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Plays a submit sound when any Button found by the class names is clicked.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class ButtonClickAudioPlayer : AbstractElement<Button>
    {
        [SerializeField, Tooltip("The local AudioSource component.")]
        private AudioSource source;
        [SerializeField, Tooltip("The Global Menu Data.")]
        private MenuData data;

        public MenuData Data => data;

        private void Reset() => source = GetComponent<AudioSource>();

        public void PlaySubmitSound() => source.PlayOneShot(Data.submit);
        public void PlayCancelSound() => source.PlayOneShot(Data.cancel);

        public async Awaitable WaitSubmitSoundAsync() => await Awaitable.WaitForSecondsAsync(Data.submit.length);
        public async Awaitable WaitCancelSoundAsync() => await Awaitable.WaitForSecondsAsync(Data.cancel.length);

        protected override void RegisterEvent(Button b) => b.clicked += HandleClickEvent;
        protected override void UnregisterEvent(Button b) => b.clicked -= HandleClickEvent;

        private void HandleClickEvent() => PlaySubmitSound();
    }
}