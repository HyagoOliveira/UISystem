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
        [Tooltip("The audio played when select a Menu Element (buttons, list item etc)")]
        public AudioClip selection;
        [Tooltip("The audio played when submit a Menu Element (buttons, list item etc).")]
        public AudioClip submit;
        [Tooltip("The audio played when cancel a Menu Element (buttons, list item etc).")]
        public AudioClip cancel;

        [Header("Taps")]
        [Tooltip("The audio played when select a Tab.")]
        public AudioClip selectTab;

        [Header("Popus")]
        [Tooltip("The audio played when opening a Popup.")]
        public AudioClip openPopup;
        [Tooltip("The audio played when closing a Popup.")]
        public AudioClip closePopup;
    }
}