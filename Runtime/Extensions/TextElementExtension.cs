using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Extensions for <see cref="TextElementExtension"/> instances.
    /// </summary>
    public static class TextElementExtension
    {
        /// <summary>
        /// Updates the localization binding using the given table and entry IDs.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tableId">The localization table id where the <paramref name="entryId"/> is.</param>
        /// <param name="entryId">The localization entry id.</param>
        public static void UpdateLocalization(this TextElement text, string tableId, string entryId)
        {
#if UNITY_LOCALIZATION
            var localization = new UnityEngine.Localization.LocalizedString(tableId, entryId);
            text.SetBinding("text", localization);
#endif
        }

        /// <summary>
        /// Updates the localization binding using the given LocalizedString.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="localization">Localized String struct with optional fallback text.</param>
        /// <returns></returns>
        public static async Awaitable UpdateLocalization(this TextElement text, LocalizedString localization)
        {
            var isValid = await localization.HasLocalization();
            if (isValid) text.UpdateLocalization(localization.tableId, localization.entryId);
            else text.text = localization.fallback;
        }

        /// <summary>
        /// Updates the dynamic localization using the given variable name and value.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="variableName">The dynamic variable name inside the Localization Binding.</param>
        /// <param name="value">The dynamic variable value inside the Localization Binding.</param>
        public static void UpdateDynamicLocalization(this TextElement text, string variableName, string value)
        {
#if UNITY_LOCALIZATION
            var localization = text.GetBinding("text") as UnityEngine.Localization.LocalizedString;
            var variable = localization[variableName] as UnityEngine.Localization.SmartFormat.PersistentVariables.StringVariable;
            variable.Value = value;
#endif
        }

        /// <summary>
        /// <inheritdoc cref="UpdateDynamicLocalization(TextElement, string, string)"/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="variableName"><inheritdoc cref="UpdateDynamicLocalization(TextElement, string, string)" path="/param[@name='variableName']"/></param>
        /// <param name="value"><inheritdoc cref="UpdateDynamicLocalization(TextElement, string, string)" path="/param[@name='value']"/></param>
        public static void UpdateDynamicLocalization(this TextElement text, string variableName, int value)
        {
#if UNITY_LOCALIZATION
            var localization = text.GetBinding("text") as UnityEngine.Localization.LocalizedString;
            var variable = localization[variableName] as UnityEngine.Localization.SmartFormat.PersistentVariables.IntVariable;
            variable.Value = value;
#endif
        }

        /// <summary>
        /// <inheritdoc cref="UpdateDynamicLocalization(TextElement, string, string)"/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="variableName"><inheritdoc cref="UpdateDynamicLocalization(TextElement, string, string)" path="/param[@name='variableName']"/></param>
        /// <param name="value"><inheritdoc cref="UpdateDynamicLocalization(TextElement, string, string)" path="/param[@name='value']"/></param>
        public static void UpdateDynamicLocalization(this TextElement text, string variableName, bool value)
        {
#if UNITY_LOCALIZATION
            var localization = text.GetBinding("text") as UnityEngine.Localization.LocalizedString;
            var variable = localization[variableName] as UnityEngine.Localization.SmartFormat.PersistentVariables.BoolVariable;
            variable.Value = value;
#endif
        }

        /// <summary>
        /// <inheritdoc cref="UpdateDynamicLocalization(TextElement, string, string)"/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="variableName"><inheritdoc cref="UpdateDynamicLocalization(TextElement, string, string)" path="/param[@name='variableName']"/></param>
        /// <param name="value"><inheritdoc cref="UpdateDynamicLocalization(TextElement, string, string)" path="/param[@name='value']"/></param>
        public static void UpdateDynamicLocalization(this TextElement text, string variableName, float value)
        {
#if UNITY_LOCALIZATION
            var localization = text.GetBinding("text") as UnityEngine.Localization.LocalizedString;
            var variable = localization[variableName] as UnityEngine.Localization.SmartFormat.PersistentVariables.FloatVariable;
            variable.Value = value;
#endif
        }

        /// <summary>
        /// <inheritdoc cref="UpdateDynamicLocalization(TextElement, string, string)"/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="variableName"><inheritdoc cref="UpdateDynamicLocalization(TextElement, string, string)" path="/param[@name='variableName']"/></param>
        /// <param name="value"><inheritdoc cref="UpdateDynamicLocalization(TextElement, string, string)" path="/param[@name='value']"/></param>
        /// <param name="format">The Date Time format. Default is abbreviated date (d).</param>
        public static void UpdateDynamicLocalization(this TextElement text, string variableName, System.DateTime value, string format = "d")
        {
#if UNITY_LOCALIZATION
            var localization = text.GetBinding("text") as UnityEngine.Localization.LocalizedString;
            var variable = localization[variableName] as UnityEngine.Localization.SmartFormat.PersistentVariables.StringVariable;

            localization.StringChanged += _ =>
            {
                var code = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.Identifier.Code;
                var info = new System.Globalization.CultureInfo(code);
                variable.Value = value.ToString(format, info);
            };
#endif
        }
    }
}