using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Label component for Selectable UIs.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Label : AbstractTransition
    {
        [SerializeField, Tooltip("The local Text component.")]
        private TMP_Text target;
        [SerializeField, Tooltip("[Optional] The local Localization component.")]
        private LocalizeStringEvent localization;

        [Space]
        [SerializeField, Tooltip("Whether to enable the local Target Auto Size.")]
        private bool useAutoSize = true;

        /// <summary>
        /// The label text.
        /// </summary>
        public string Text
        {
            get => Target.text;
            set => Target.text = value;
        }

        /// <summary>
        /// The local Text component.
        /// </summary>
        public TMP_Text Target
        {
            get => target;
            set => target = value;
        }

        private void Reset() => Setup();
        private void Start() => TrySetupTargetAutosize();

        private void TrySetupTargetAutosize()
        {
            if (!useAutoSize) return;

            // Settings this values only in runtime to avoid
            // Prefabs getting values changes in Editor
            target.enableAutoSizing = true;
            target.fontSizeMax = target.fontSize;
            // Maybe add min/max font size into a LabelData SO
        }

        private void Setup()
        {
            target = GetComponent<TMP_Text>();
            localization = GetComponent<LocalizeStringEvent>();

            if (target == null) return;

            target.color = Color.white;
            target.raycastTarget = false;
        }

        public override void Transit(SelectionState state, bool _)
        {
            if (data) target.color = data.GetColor(state);
        }

        /// <summary>
        /// Updates the local Localization component using the given table and name key.
        /// </summary>
        /// <param name="table">The name of the Localized table.</param>
        /// <param name="key">The name of the Localized entry inside table.</param>
        public void UpdateLocalization(string table, string key) =>
            localization.StringReference.SetReference(table, key);

        public void UpdateLocalization(UnityEngine.Localization.LocalizedString reference) =>
            localization.StringReference = reference;

        /// <summary>
        /// Clears the local Localization component, seting the label text to empty.
        /// </summary>
        public void ClearLocalization()
        {
            localization.StringReference = new UnityEngine.Localization.LocalizedString();
            localization.OnUpdateString?.Invoke(string.Empty); // Clear the Text string
        }
    }
}