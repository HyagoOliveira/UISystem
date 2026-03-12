using System;
using UnityEngine.EventSystems;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Interface used on objects able to be clicked (like buttons). 
    /// </summary>
    public interface IClickable : IPointerClickHandler, ISubmitHandler
    {
        /// <summary>
        /// Event fired when this object is clicked.
        /// </summary>
        event Action OnClicked;
    }
}