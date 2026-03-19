using System;
using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Audio handler for UI elements.
    /// </summary>
    /// <remarks>
    /// Plays a selection and submition sound for every <see cref="ISelectable"/> 
    /// and <see cref="IClickable"/> implementation, respectably.
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioHandler : MonoBehaviour
    {
        [SerializeField, Tooltip("The local Audio Source.")]
        private AudioSource audioSource;
        [SerializeField, Tooltip("The Global Data for Menus.")]
        private MenuData data;

        /// <summary>
        /// The local Audio Source.
        /// </summary>
        public AudioSource Audio => audioSource;

        /// <summary>
        /// The Global Data for Menus.
        /// </summary>
        public MenuData Data
        {
            get => data;
            set => data = value;
        }

        private IClickable[] clickables = Array.Empty<IClickable>();
        private ISelectable[] selectables = Array.Empty<ISelectable>();

        private void Reset()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 0f; // 2D Audio
        }

        public void PlaySelection() => Play(data.selection);
        public void PlaySubmition() => Play(data.submition);
        public void PlayCancellation() => Play(data.cancellation);
        public void PlayTabSelection() => Play(data.tabSelection);
        public void PlayModalOpen() => Play(data.modalOpen);
        public void PlayModalClose() => Play(data.modalClose);

        public void Play(AudioClip clip)
        {
            Audio.Stop();
            Audio.PlayOneShot(clip);
        }

        public void BindElements(Transform container)
        {
            FindElements(container);
            SubscribeElements();
        }

        public void UnbindElements()
        {
            UnsubscribeScreenElements();
            ClearElements();
        }

        public float GetSubmitionTime() => data.submition.length;

        private void FindElements(Transform container)
        {
            clickables = container.GetComponentsInChildren<IClickable>(includeInactive: true);
            selectables = container.GetComponentsInChildren<ISelectable>(includeInactive: true);
        }

        private void ClearElements()
        {
            clickables = Array.Empty<IClickable>();
            selectables = Array.Empty<ISelectable>();
        }

        private void SubscribeElements()
        {
            foreach (var clickable in clickables)
            {
                clickable.OnClicked += HandleClicked;
            }

            foreach (var selectable in selectables)
            {
                selectable.OnSelected += HandleSelected;
            }
        }

        private void UnsubscribeScreenElements()
        {
            foreach (var clickable in clickables)
            {
                clickable.OnClicked -= HandleClicked;
            }

            foreach (var selectable in selectables)
            {
                selectable.OnSelected -= HandleSelected;
            }
        }

        private void HandleSelected() => PlaySelection();
        private void HandleClicked() => PlaySubmition();
    }
}