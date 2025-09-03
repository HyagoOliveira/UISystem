using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Localized String struct with optional fallback text.
    /// </summary>
    [System.Serializable]
    public struct LocalizedString
    {
        /// <summary>
        /// The Table Id where the entry is located.
        /// </summary>
        public string tableId;

        /// <summary>
        /// The Entry Id to be localized.
        /// </summary>
        public string entryId;

        /// <summary>
        /// The fallback text to be used when an available localization is not found.
        /// </summary>
        public string fallback;

        /// <summary>
        /// A struct used to reference a localized string using a Table Id and an Entry Id, with an optional fallback text.
        /// </summary>
        /// <param name="tableId">The Table Id where the entry is located.</param>
        /// <param name="entryId">The Entry Id to be localized.</param>
        /// <param name="fallback">The fallback text to be used when an available localization is not found.</param>
        public LocalizedString(string tableId, string entryId, string fallback = "")
        {
            this.tableId = tableId;
            this.entryId = entryId;
            this.fallback = fallback;
        }

        public async readonly Awaitable<bool> HasLocalization()
        {
#if UNITY_LOCALIZATION
            var database = UnityEngine.Localization.Settings.LocalizationSettings.StringDatabase;
            var table = await database.GetTableEntryAsync(tableId, entryId).Task;
            return table.Entry != null && !string.IsNullOrEmpty(table.Entry.GetLocalizedString());
#else
            return false;
#endif
        }

        public override readonly string ToString() => $"{tableId}/{entryId} ({fallback})";
    }
}