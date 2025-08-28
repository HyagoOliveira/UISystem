using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Data holder for the Popups used in the game.
    /// <para>Use to find the in game popups.</para>
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Popups : MonoBehaviour
    {
        [SerializeField] private CanvasDialoguePopup dislogue;

        public static CanvasDialoguePopup Dialogue { get; private set; }

        private void Awake() => Dialogue = dislogue;
        private void OnDisable() => Dialogue = null;

        public static bool IsDisplayingAnyPopup() => Dialogue && Dialogue.isActiveAndEnabled;
    }
}