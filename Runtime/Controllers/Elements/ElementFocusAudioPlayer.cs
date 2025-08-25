using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Plays a selection sound when any Element found by <see cref="className"/> is focused.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class ElementFocusAudioPlayer : AbstractElement<VisualElement>
    {
        [SerializeField, Tooltip("The local AudioSource component.")]
        private AudioSource source;
        [SerializeField, Tooltip("The Global Menu Data.")]
        private MenuData data;

        [Space]
        [SerializeField, Tooltip("The class name used to find the elements.")]
        private string className = "unity-button";

        private void Reset() => source = GetComponent<AudioSource>();

        public void PlaySelectionSound() => source.PlayOneShot(data.selection);

        public async void FocusWithoutSound(VisualElement element)
        {
            await Awaitable.NextFrameAsync();
            element.Focus();
        }

        protected override string GetClassName() => className;
        protected override void RegisterEvent(VisualElement e) => e.RegisterCallback<FocusEvent>(HandleElementFocused);
        protected override void UnregisterEvent(VisualElement e) => e.UnregisterCallback<FocusEvent>(HandleElementFocused);

        private void HandleElementFocused(FocusEvent _) => PlaySelectionSound();
    }
}