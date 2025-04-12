using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Plays a submit sound when any Button found by <see cref="className"/> is clicked.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class ButtonClickAudioPlayer : AbstractController
    {
        [SerializeField, Tooltip("The local AudioSource component.")]
        private AudioSource source;
        [SerializeField, Tooltip("The Global Menu Data.")]
        private MenuData data;

        [Space]
        [Tooltip("The class name used to find the buttons.")]
        public string className = "unity-button";

        private UQueryBuilder<Button> elements;

        protected override void Reset()
        {
            base.Reset();
            source = GetComponent<AudioSource>();
        }

        protected override void FindReferences()
        {
            base.FindReferences();
            elements = Root.Query<Button>(className: className);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            elements.ForEach(b => b.clicked += HandleClickEvent);
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            elements.ForEach(b => b.clicked -= HandleClickEvent);
        }

        public void PlaySubmitSound() => source.PlayOneShot(data.submit);
        public void PlayCancelSound() => source.PlayOneShot(data.cancel);

        private void HandleClickEvent() => PlaySubmitSound();
    }
}
