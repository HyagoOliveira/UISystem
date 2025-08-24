using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Plays a submit sound when any Button found by <see cref="className"/> is clicked.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class ButtonClickAudioPlayer : MonoBehaviour, IDisposable
    {
        [SerializeField, Tooltip("The local AudioSource component.")]
        private AudioSource source;
        [SerializeField, Tooltip("The Global Menu Data.")]
        private MenuData data;

        [Space]
        [Tooltip("The class name used to find the buttons.")]
        public string className = "unity-button";

        private UQueryBuilder<Button> elements;

        private void Reset() => source = GetComponent<AudioSource>();

        public void Initialize(VisualElement root)
        {
            elements = root.Query<Button>(className: className);
            elements.ForEach(b => b.clicked += HandleClickEvent);
        }

        public void Dispose() => elements.ForEach(b => b.clicked -= HandleClickEvent);
        public void PlaySubmitSound() => source.PlayOneShot(data.submit);
        public void PlayCancelSound() => source.PlayOneShot(data.cancel);

        private void HandleClickEvent() => PlaySubmitSound();
    }
}