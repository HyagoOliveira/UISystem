using ActionCode.LocalizationSystem;
using System;
using UnityEngine;

namespace ActionCode.UISystem
{
    [DisallowMultipleComponent]
    public abstract class AbstractActionDataButton<T> : ActionButton where T : class
    {
        public T Data { get; private set; }

        public event Action<T> OnDataClicked;
        public event Action<T> OnDataSelected;

        public void SetData(T data) => Data = data;

        /// <summary>
        /// <inheritdoc cref="LocalizedStringExtension.UpdateDynamicLocalization(UnityEngine.Localization.LocalizedString, string, string)"/>
        /// </summary>
        /// <param name="variableName">
        /// <inheritdoc cref="LocalizedStringExtension.UpdateDynamicLocalization(UnityEngine.Localization.LocalizedString, string, string)" path="/param[@name='variableName']"/>
        /// </param>
        /// <param name="value">
        /// <inheritdoc cref="LocalizedStringExtension.UpdateDynamicLocalization(UnityEngine.Localization.LocalizedString, string, string)" path="/param[@name='value']"/>
        /// </param>
        public void UpdateDynamicLocalization(string variableName, string value) =>
            Label.Localization.StringReference.UpdateDynamicLocalization(variableName, value);

        protected override void HandleClicked()
        {
            base.HandleClicked();
            OnDataClicked?.Invoke(Data);
        }

        protected override void HandleSelection()
        {
            base.HandleSelection();
            OnDataSelected?.Invoke(Data);
        }
    }
}