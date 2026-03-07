using System;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Interface used on objects able to be submitted (like buttons). 
    /// </summary>
    public interface ISubmitable
    {
        /// <summary>
        /// Event fired when this object is submitted.
        /// </summary>
        event Action OnSubmitted;
    }
}