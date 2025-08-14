using UnityEngine;
using UnityEngine.InputSystem;
using ActionCode.InputSystem;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Handler for Menu Audio.
    /// Plays the cancel Audio Effect when the UI Cancel input is performed.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class MenuAudioHandler : MonoBehaviour
    {
        [SerializeField, Tooltip("The local Menu component")]
        private AbstractMenu menu;
        [SerializeField, Tooltip("The local AudioSource component.")]
        private AudioSource source;
        [SerializeField, Tooltip("The Global Menu Data.")]
        private MenuData menuData;

        [Header("Input")]
        [SerializeField] private InputActionAsset input;
        [SerializeField] private InputActionPopup cancel = new(nameof(input), "UI");

        private InputAction cancelAction;

        private void Reset() => FindDependencies();
        private void Awake() => cancelAction = input.FindAction(cancel.GetPath());
        private void OnEnable() => cancelAction.performed += HandleCancelPerformed;
        private void OnDisable() => cancelAction.performed -= HandleCancelPerformed;

        public void PlaySubmitSound() => source.PlayOneShot(menuData.submit);
        public void PlayCancelSound() => source.PlayOneShot(menuData.cancel);

        public async Awaitable PlayAndWaitSubmitSound()
        {
            PlaySubmitSound();
            await Awaitable.WaitForSecondsAsync(menuData.submit.length);
        }

        private void FindDependencies()
        {
            source = GetComponent<AudioSource>();
            menu = GetComponent<AbstractMenu>();
        }

        private void HandleCancelPerformed(InputAction.CallbackContext _)
        {
            if (menu.TryOpenLastScreen()) PlayCancelSound();
        }
    }
}
