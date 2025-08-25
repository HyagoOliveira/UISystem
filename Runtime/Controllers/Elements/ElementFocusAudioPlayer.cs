using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Plays a selection sound when any Element found by <see cref="className"/> is focused.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class ElementFocusAudioPlayer : MonoBehaviour, IDisposable
    {
        [SerializeField, Tooltip("The local AudioSource component.")]
        private AudioSource source;
        [SerializeField, Tooltip("The Global Menu Data.")]
        private MenuData data;

        [Space]
        [SerializeField, Tooltip("The class name used to find the elements.")]
        private string className = "unity-button";

        private UQueryBuilder<VisualElement> elements;

        private void Reset() => source = GetComponent<AudioSource>();

        public void Initialize(VisualElement root)
        {
            elements = root.Query<VisualElement>(className: className);
            elements.ForEach(e => e.RegisterCallback<FocusEvent>(HandleElementFocused));
        }

        public void Dispose() => elements.ForEach(e => e.UnregisterCallback<FocusEvent>(HandleElementFocused));
        public void PlaySelectionSound() => source.PlayOneShot(data.selection);

        public async void FocusWithoutSound(VisualElement element)
        {
            await Awaitable.NextFrameAsync();
            element.Focus();
        }

        private void HandleElementFocused(FocusEvent _) => PlaySelectionSound();
    }
}
