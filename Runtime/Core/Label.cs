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
        public TMP_Text target;
        public LocalizeSpriteEvent localization;

        /// <summary>
        /// The label text.
        /// </summary>
        public string Text
        {
            get => target.text;
            set => target.text = value;
        }

        private void Reset() => Setup();
        private void Start() => SetupTarget();

        private void SetupTarget()
        {
            // Settings this values only in runtime to avoid
            // Prefabs getting values changes in Editor
            target.enableAutoSizing = true;
            target.fontSizeMax = target.fontSize;
            // Maybe add min/max font size into a LabelData SO
        }

        private void Setup()
        {
            localization = GetComponent<LocalizeSpriteEvent>();
            target = GetComponent<TMP_Text>();
            if (target == null) return;

            target.color = Color.white;
            target.raycastTarget = false;
        }

        public override void Transit(SelectionState state, bool _)
        {
            if (data) target.color = data.GetColor(state);
        }
    }
}