using UnityEngine;
using UnityEngine.EventSystems;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Manager for EventSystem.
    /// </summary>
    public static class EventManager
    {
        /// <summary>
        /// Tries to set the current selected GameObject in the Event System if Event System is available.
        /// </summary>
        /// <param name="instance">The GameObject instance to set.</param>
        public static void TrySetSelectedGameObject(GameObject instance)
        {
            if (EventSystem.current) EventSystem.current.SetSelectedGameObject(instance);
        }

        /// <summary>
        /// Tries to set the send navigation events in the Event System if Event System is available.
        /// </summary>
        /// <param name="sendNavigationEvents">
        /// <inheritdoc cref="EventSystem.sendNavigationEvents"/>
        /// </param>
        public static void TrySendNavigationEvents(bool sendNavigationEvents)
        {
            if (EventSystem.current) EventSystem.current.sendNavigationEvents = sendNavigationEvents;
        }

        /// <summary>
        /// Waits until Event System is available or timeout is reached.
        /// Useful when opening a Screen in the first frame since Event System may not be loaded yet.
        /// </summary>
        /// <returns>An asynchronous operation.</returns>
        public static async Awaitable WaitUntilEventSystemIsReadyAsync()
        {
            const float timeout = 5f;

            var currentTime = 0f;
            while (EventSystem.current == null || currentTime > timeout)
            {
                await Awaitable.NextFrameAsync();
                currentTime += Time.deltaTime;
            }
        }
    }
}
