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
    }
}