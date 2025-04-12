using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Global data container to be used for multiple Menus.
    /// </summary>
    [CreateAssetMenu(fileName = "MenuData", menuName = "ActionCode/UI System/Menu Data", order = 110)]
    public class MenuData : ScriptableObject
    {
        [Tooltip("The audio played when selected.")]
        public AudioClip selection;
        [Tooltip("The audio played when submitted.")]
        public AudioClip submit;
        [Tooltip("The audio played when canceled.")]
        public AudioClip cancel;

        public float GetSubmitTime() => submit.length;
    }
}