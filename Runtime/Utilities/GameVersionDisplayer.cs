using TMPro;
using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Displays the Project Version.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GameVersionDisplayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;

        private void Reset() => label = GetComponentInChildren<TMP_Text>();
        private void Start() => label.text = $"v {Application.version}";
    }
}