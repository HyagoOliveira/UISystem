using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Global data container to be used for multiple Menus.
    /// </summary>
    [CreateAssetMenu(fileName = "MenuData", menuName = "ActionCode/UI System/Menu Data", order = 110)]
    public class MenuData : ScriptableObject
    {
        [Header("Screen")]
        [Tooltip("The audio played when a element inside a Screen is selected.")]
        public AudioClip screenSelection;
        [Tooltip("The audio played when a element inside a Screen is submitted.")]
        public AudioClip screenSubmit;
        [Tooltip("The audio played when a Screen is canceled.")]
        public AudioClip screenCancel;
        [Tooltip("The audio played for a Screen error.")]
        public AudioClip screenError;

        [Header("Tabs")]
        [Tooltip("The audio played when a Tab is selected.")]
        public AudioClip tabSelecttion;

        [Header("Modals")]
        [Tooltip("The audio played when a Modal is opened.")]
        public AudioClip modalOpen;
        [Tooltip("The audio played when a Modal is closed.")]
        public AudioClip modalClose;

        // https://flatuicolors.com/
        //[Header("Color Palette")]

    }
}