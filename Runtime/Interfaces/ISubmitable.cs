using System;
using UnityEngine.EventSystems;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Interface used on objects able to be submitted (like buttons). 
    /// </summary>
    public interface ISubmitable : IPointerClickHandler, ISubmitHandler
    {
        /// <summary>
        /// Event fired when this object is submitted.
        /// </summary>
        event Action OnSubmitted;
    }
}