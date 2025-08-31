using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Global data container to be used for multiple Menus.
    /// </summary>
    [CreateAssetMenu(fileName = "MenuData", menuName = "ActionCode/UI System/Menu Data", order = 110)]
    public class MenuData : ScriptableObject
    {
        [Header("Menus")]
        [Tooltip("The audio played when selected.")]
        public AudioClip selection;
        [Tooltip("The audio played when submitted.")]
        public AudioClip submit;
        [Tooltip("The audio played when canceled.")]
        public AudioClip cancel;

        [Header("Popus")]
        [Tooltip("The audio played when opening a Popup.")]
        public AudioClip openPopup;
        [Tooltip("The audio played when closing a Popup.")]
        public AudioClip closePopup;

        public float GetSubmitTime() => submit.length;
    }
}