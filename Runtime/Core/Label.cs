using TMPro;
using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Label component for Selectable UIs.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Label : AbstractTransition
    {
        public TMP_Text target;

        /// <summary>
        /// The label text.
        /// </summary>
        public string Text
        {
            get => target.text;
            set => target.text = value;
        }

        private void Reset() => Setup();

        private void Setup()
        {
            target = GetComponent<TMP_Text>();
            if (target == null) return;

            target.color = Color.white;
            target.raycastTarget = false;
        }

        public override void Transit(SelectionState state, bool _)
        {
            if (Data) target.color = Data.GetColor(state);
        }
    }
}