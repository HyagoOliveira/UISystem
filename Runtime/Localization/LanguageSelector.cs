using ActionCode.LocalizationSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace ActionCode.UISystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(AudioHandler))]
    public sealed class LanguageSelector : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private AudioHandler audioHandler;
        [SerializeField] private ListController languages;

        [Space]
        [Tooltip("Event fired when the localization is selected.")]
        public UnityEvent OnLanguageConfirmed;

        private readonly List<LanguageButton> buttons = new();

        private void Reset()
        {
            group = GetComponent<CanvasGroup>();
            audioHandler = GetComponent<AudioHandler>();
        }

        private void OnEnable() => PopulateLanguageButtons();
        private void OnDisable() => Dispose();

        private void Dispose()
        {
            foreach (var button in buttons)
            {
                UnbindButton(button);
            }
            buttons.Clear();
        }

        private async void PopulateLanguageButtons()
        {
            SetGroupEnabled(false);
            audioHandler.UnbindElements();

            var locales = await LocalizationManager.GetLocalesAsync();
            var selectedIndex = LocalizationManager.GetLocaleIndex(locales);

            languages.Clear();

            foreach (var locale in locales)
            {
                var item = languages.Add();
                var button = item.GetComponent<LanguageButton>();

                button.SetLocale(locale);

                BindButton(button);
                buttons.Add(button);
            }

            SetGroupEnabled(true);
            languages.Select(selectedIndex);
            audioHandler.BindElements(transform);
        }

        private void SetGroupEnabled(bool isEnabled)
        {
            group.interactable = isEnabled;
            group.alpha = isEnabled ? 1f : 0f;
        }

        private void BindButton(LanguageButton button)
        {
            button.OnLocaleConfirmed += HandleLanguageConfirmed;
            button.OnLocaleSelected += HandleLanguageSelected;
        }

        private void UnbindButton(LanguageButton button)
        {
            button.OnLocaleConfirmed -= HandleLanguageConfirmed;
            button.OnLocaleSelected -= HandleLanguageSelected;
        }

        private void HandleLanguageConfirmed(Locale locale)
        {
            group.interactable = false;
            LocalizationManager.Select(locale);
            OnLanguageConfirmed?.Invoke();
        }

        // GameData.Settings.LanguageCode is update by GameDataLocaleHandler
        private void HandleLanguageSelected(Locale locale) => LocalizationManager.Select(locale);
    }
}