using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Controller for a Generic Game Menu.
    /// <para>
    /// Create your own menu component and reference this component to navigate 
    /// through the Screens using the <see cref="OpenScreenAsync(string, bool)"/> function.
    /// </para>
    /// </summary>
    /// <remarks>
    /// A Menu is a Finite State Machine containing several Screens, keeping track the Current and Last one.<br/>
    /// Only one Screen can be activated at time<br/>
    /// From an opened Screen, you can go back to the last one using the Cancel input (back button).
    /// </remarks>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class Menu : MonoBehaviour
    {
        [SerializeField, Tooltip("The local Audio Source.")]
        private AudioSource audioSource;
        [SerializeField, Tooltip("The local Canvas Group.")]
        private CanvasGroup canvasGroup;

        [Space]
        [Tooltip("The Global Data for this Menu.")]
        public MenuData data;

        [Space]
        [Tooltip("[Optional] The first screen to activated when start. Leave it empty if you wish to do it manually.")]
        public Screen firstScreen;

        #region Events
        /// <summary>
        /// Event fired when the given screen is opened.
        /// </summary>
        public event Action<Screen> OnScreenOpened;

        /// <summary>
        /// Event fired when the given screen is closed.
        /// </summary>
        public event Action<Screen> OnScreenClosed;

        /// <summary>
        /// Event fired when the given screen is canceled: the back button is pressed.
        /// </summary>
        public event Action<Screen> OnScreenCanceled;
        #endregion

        #region Properties
        /// <summary>
        /// The local Audio Source for this menu.
        /// </summary>
        public AudioSource Audio => audioSource;

        /// <summary>
        /// The last activated screen. It can be null if no screen has been navigated yet.
        /// </summary>
        public Screen LastScreen { get; private set; }

        /// <summary>
        /// The current activated screen. It can be null if no screen has been opened yet.
        /// </summary>
        public Screen CurrentScreen { get; private set; }

        /// <summary>
        /// All screens available in this menu, indexed by their identifiers.
        /// </summary>
        public Dictionary<string, Screen> Screens { get; private set; }
        #endregion

        private ISelectable[] selectables = Array.Empty<ISelectable>();
        private ISubmitable[] submitables = Array.Empty<ISubmitable>();
        private readonly Stack<Screen> undoHistory = new();

        private void Reset()
        {
            audioSource = GetComponent<AudioSource>();
            canvasGroup = GetComponent<CanvasGroup>();
            firstScreen = GetComponentInChildren<Screen>(includeInactive: false);
        }

        private void Awake() => InitializeScreens();
        private void OnEnable() => TryOpenFirstScreen();
        private void OnDisable() => UnsubscribeScreenElements();

        public static async Awaitable SetSelectedGameObjectAsync(GameObject instance)
        {
            while (EventSystem.current == null) await Awaitable.NextFrameAsync();
            EventSystem.current.SetSelectedGameObject(instance);
        }

        public void PlayAudio(AudioClip clip)
        {
            Audio.Stop();
            Audio.PlayOneShot(clip);
        }

        #region Open Screen
        /// <summary>
        /// Opens the <see cref="firstScreen"/> asynchronously if available.
        /// </summary>
        public void OpenFirstScreen() => _ = OpenScreenAsync(firstScreen, undoable: false);

        /// <summary>
        /// Opens the <see cref="firstScreen"/> if available.
        /// </summary>
        /// <returns>An asynchronously operation.</returns>
        public async Awaitable OpenFirstScreenAsync() => await OpenScreenAsync(firstScreen, undoable: false);

        /// <summary>
        /// Opens the given screen instance asynchronously. 
        /// </summary>
        /// <typeparam name="T">The screen type to open.</typeparam>
        /// <param name="screen">The screen instance to open.</param>
        /// <param name="undoable">Whether this screen can be closed using the back button.</param>
        /// <returns><inheritdoc cref="OpenFirstScreenAsync"/></returns>
        public async Awaitable OpenScreenAsync<T>(T screen, bool undoable = true) where T : Screen
            => await OpenScreenAsync(screen.GetIdentifier(), undoable);

        /// <summary>
        /// Opens the given screen asynchronously using the given screen identifier. 
        /// </summary>
        /// <param name="identifier">The screen identifier.</param>
        /// <param name="undoable"><inheritdoc cref="OpenScreenAsync{T}(T, bool)" path="/param[@name='undoable']"/> </param>
        /// <returns><inheritdoc cref="OpenFirstScreenAsync"/></returns>
        public async Awaitable OpenScreenAsync(string identifier, bool undoable = true)
        {
            var hasScreen = Screens.TryGetValue(identifier, out var screen);
            if (!hasScreen)
            {
                Debug.LogError($"No screen {identifier} found for menu {gameObject.name}.");
                return;
            }

            canvasGroup.blocksRaycasts = false;

            if (CurrentScreen)
            {
                //TODO await CurrentScreen FadeOutAsync
                await Awaitable.WaitForSecondsAsync(0.1f);
                CurrentScreen.Close();
                OnScreenClosed?.Invoke(CurrentScreen);
            }

            UnsubscribeScreenElements();
            CloseOpenedScreens();

            if (undoable && LastScreen) undoHistory.Push(LastScreen);

            LastScreen = CurrentScreen;
            CurrentScreen = screen;

            CurrentScreen.Open();
            //TODO await CurrentScreen FadeInAsync
            await Awaitable.WaitForSecondsAsync(0.1f);

            FindScreenElements();
            SubscribeScreenElements();

            OnScreenOpened?.Invoke(CurrentScreen);
            await SetSelectedGameObjectAsync(CurrentScreen.firstInput);

            canvasGroup.blocksRaycasts = true;
        }

        /// <summary>
        /// Tries to open asynchronously the last screen using the undo history.
        /// </summary>
        /// <returns><inheritdoc cref="OpenFirstScreenAsync"/></returns>
        public async Awaitable TryOpenLastScreen()
        {
            var hasUndoableScreen = undoHistory.TryPop(out var screen);
            if (hasUndoableScreen) await OpenScreenAsync(screen, undoable: false);
        }

        private void CloseOpenedScreens()
        {
            foreach (var screen in Screens.Values)
            {
                if (screen.IsOpenned()) screen.Close();
            }
        }
        #endregion

        #region Initialization
        private void InitializeScreens()
        {
            var screens = GetComponentsInChildren<Screen>(includeInactive: true);
            Screens = new(screens.Length);

            foreach (var screen in screens)
            {
                screen.Initialize(this);
                Screens.Add(screen.GetIdentifier(), screen);
            }
        }

        private async void TryOpenFirstScreen()
        {
            if (firstScreen == null) return;

            // Await one frame to let the First Screen components initialize
            await Awaitable.NextFrameAsync();
            await OpenFirstScreenAsync();
        }
        #endregion

        #region Subscriptions / Unsubscriptions
        private void FindScreenElements()
        {
            selectables = CurrentScreen.GetComponentsInChildren<ISelectable>(includeInactive: true);
            submitables = CurrentScreen.GetComponentsInChildren<ISubmitable>(includeInactive: true);
        }

        private void SubscribeScreenElements()
        {
            foreach (var selectable in selectables)
            {
                selectable.OnSelected += HandleSelectableSelected;
            }

            foreach (var submitable in submitables)
            {
                submitable.OnSubmitted += HandleSubmitableSubmited;
            }
        }

        private void UnsubscribeScreenElements()
        {
            foreach (var selectable in selectables)
            {
                selectable.OnSelected -= HandleSelectableSelected;
            }

            foreach (var submitable in submitables)
            {
                submitable.OnSubmitted -= HandleSubmitableSubmited;
            }
        }

        private void HandleSelectableSelected() => PlayAudio(data.selection);
        private void HandleSubmitableSubmited() => PlayAudio(data.submition);
        #endregion
    }
}