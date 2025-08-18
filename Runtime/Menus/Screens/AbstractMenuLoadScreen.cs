using System;
using UnityEngine;

namespace ActionCode.UISystem
{
    public abstract class AbstractMenuLoadScreen : AbstractMenuScreen
    {
        /// <summary>
        /// Event fired when data is confirmed the to be loaded.
        /// </summary>
        public event Action OnDataLoadConfirmed;

        /// <summary>
        /// Loads the data from the last slot.
        /// </summary>
        /// <returns></returns>
        public abstract Awaitable LoadFromLastSlotAsync();

        public abstract void ResetGameData();

        protected void ConfirmDataLoad() => OnDataLoadConfirmed?.Invoke();
    }
}