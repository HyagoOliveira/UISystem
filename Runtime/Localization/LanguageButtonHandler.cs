using ActionCode.LocalizationSystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace ActionCode.UISystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ActionButton))]
    public sealed class LanguageButtonHandler : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private ActionButton button;

        public event Action<Locale> OnSelected;
        public event Action<Locale> OnConfirmed;

        private Locale locale;

        private void OnDisable() => Unbind();

        public void SetLocale(Locale locale)
        {
            this.locale = locale;

            gameObject.name = $"Button_{locale.LocaleName}";
            label.text = LocalizationManager.GetDisplayName(locale);

            Bind();
        }

        private void Bind()
        {
            button.OnClicked += HandleClicked;
            button.OnSelected += HandleSelection;
        }

        private void Unbind()
        {
            button.OnClicked -= HandleClicked;
            button.OnSelected -= HandleSelection;
        }

        private void HandleClicked()
        {
            if (locale) OnConfirmed?.Invoke(locale);
        }

        private void HandleSelection()
        {
            if (locale) OnSelected?.Invoke(locale);
        }
    }
}