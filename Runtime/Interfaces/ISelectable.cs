using System;
using UnityEngine.EventSystems;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Interface used on objects able to be selected. 
    /// </summary>
    public interface ISelectable : IPointerEnterHandler, ISelectHandler
    {
        /// <summary>
        /// Event fired when this object is selected.
        /// </summary>
        event Action OnSelected;
    }
}