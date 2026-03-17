using TMPro;
using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Label component for Selectable UIs.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Label : MonoBehaviour
    {
        public TMP_Text target;
        public SelectableTransitionData colors;

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
    }
}