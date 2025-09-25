using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract class for a Pause Menu, using a <see cref="AbstractPauseScreen"/> implementation.
    /// You can further extend.
    /// </summary>
    [RequireComponent(typeof(MenuController))]
    public abstract class AbstractPauseMenu : MonoBehaviour
    {
        [SerializeField] private MenuController menu;

        [Header("Audio")]
        [SerializeField] private AudioClip open;
        [SerializeField] private AudioClip close;

        [Header("Screens")]
        [SerializeField] private AbstractPauseScreen pauseScreen;

        public MenuController Menu => menu;
        public AbstractPauseScreen PauseScreen => pauseScreen;

        protected virtual void Reset()
        {
            menu = GetComponent<MenuController>();
            pauseScreen = GetComponentInChildren<AbstractPauseScreen>(includeInactive: true);
        }

        private void OnEnable() => SubscribeEvents();
        private void OnDisable() => UnsubscribeEvents();

        public void OpenPauseScreen()
        {
            PlayOpenSound();
            _ = Menu.OpenScreenAsync(PauseScreen, undoable: false, fadeScreen: false);
        }

        public void CloseScreen()
        {
            PlayCloseSound();
            Menu.CloseCurrrentScreen();
        }

        protected virtual void SubscribeEvents() { }
        protected virtual void UnsubscribeEvents() { }

        protected void PlayOpenSound() => Menu.AudioSource.PlayOneShot(open);
        protected void PlayCloseSound() => Menu.AudioSource.PlayOneShot(close);
    }
}