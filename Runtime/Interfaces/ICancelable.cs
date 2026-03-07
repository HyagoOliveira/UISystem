using System;
using UnityEngine.EventSystems;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Interface used on objects able to be canceled.
    /// </summary>
    public interface ICancelable : ICancelHandler
    {
        /// <summary>
        /// Event fired when this object is canceled.
        /// </summary>
        event Action OnCanceled;
    }
}