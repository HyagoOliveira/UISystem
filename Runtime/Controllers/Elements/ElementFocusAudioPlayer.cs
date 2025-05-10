using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Plays a selection sound when any Element found by <see cref="ClassName"/> is focused.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class ElementFocusAudioPlayer : AbstractController
    {
        [SerializeField, Tooltip("The Global Menu Data.")]
        private MenuData data;
        [SerializeField, Tooltip("The local AudioSource component.")]
        private AudioSource source;

        [Space]
        [SerializeField, Tooltip("The class name used to find the elements.")]
        private string className = "unity-button";

        /// <summary>
        /// The class name used to find the elements.
        /// </summary>
        public string ClassName
        {
            get => className;
            internal set => className = value;
        }

        private UQueryBuilder<VisualElement> elements;

        protected override void Reset()
        {
            base.Reset();
            source = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Updates a new Class Name so the new elements play a selection sound when focused.
        /// </summary>
        /// <param name="className">The new class name.</param>
        public void UpdateClassName(string className)
        {
            enabled = false;
            ClassName = className;
            enabled = true;
        }

        /// <summary>
        /// Plays the Selection sound from <see cref="data"/>.
        /// </summary>
        public void PlaySelectionSound() => source.PlayOneShot(data.selection);

        /// <summary>
        /// Focus the given element without play the selection sound.
        /// </summary>
        /// <param name="element">The element to focus.</param>
        public async void FocusWithoutSound(VisualElement element)
        {
            enabled = false;

            await Awaitable.NextFrameAsync();
            element.Focus();

            enabled = true;
        }

        protected override void FindReferences()
        {
            base.FindReferences();
            elements = Root.Query<VisualElement>(className: ClassName);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            elements.ForEach(e => e.RegisterCallback<FocusEvent>(HandleElementFocused));
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            elements.ForEach(e => e.UnregisterCallback<FocusEvent>(HandleElementFocused));
        }

        private void HandleElementFocused(FocusEvent _) => PlaySelectionSound();
    }
}
