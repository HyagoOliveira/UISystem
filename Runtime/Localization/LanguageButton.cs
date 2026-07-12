using ActionCode.LocalizationSystem;
using System;
using UnityEngine;
using UnityEngine.Localization;

namespace ActionCode.UISystem
{
    [DisallowMultipleComponent]
    public sealed class LanguageButton : ActionButton
    {
        public event Action<Locale> OnLocaleSelected;
        public event Action<Locale> OnLocaleConfirmed;

        private Locale locale;

        public void SetLocale(Locale locale)
        {
            this.locale = locale;

            gameObject.name = $"LanguageSelectorButton_{locale.LocaleName}";
            Label.Text = LocalizationManager.GetDisplayName(locale);
        }

        protected override void HandleClicked()
        {
            base.HandleClicked();
            OnLocaleConfirmed?.Invoke(locale);
        }

        protected override void HandleSelection()
        {
            base.HandleSelection();
            OnLocaleSelected?.Invoke(locale);
        }
    }
}