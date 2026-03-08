using System;
using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Screens are used to display different parts of a Menu, as a 
    /// sub-menu Screen, a Tab Screen or other specific menu section.
    /// </summary>
    /// <remarks>
    /// Screens contains one or multiple elements that can be selected, 
    /// submitted or canceled, playing the corresponding audio from the <see cref="MenuData"/>
    /// </remarks>
    [DisallowMultipleComponent]
    public class Screen : MonoBehaviour
    {
        [Tooltip("[Optional] The first input to be selected when this screen is opened.")]
        public GameObject firstInput;

        /// <summary>
        /// The current Menu for this screen.
        /// </summary>
        public Menu Menu { get; private set; }

        private ISelectable[] selectables;
        private ISubmitable[] submitables;
        private ICancelable[] cancelables;

        public virtual void Initialize(Menu menu)
        {
            Menu = menu;
            ClearElements();
        }

        protected virtual void OnEnable() => SubscribeEvents();

        protected virtual void OnDisable()
        {
            UnsubscribeEvents();
            UnsubscribeScreenElements();
        }

        public bool IsOpenned() => gameObject.activeSelf;
        public bool IsClosed() => !IsOpenned();

        public string GetIdentifier() => gameObject.name;

        public virtual void Open() => gameObject.SetActive(true);
        public virtual void Close() => gameObject.SetActive(false);

        protected virtual void SubscribeEvents() { }
        protected virtual void UnsubscribeEvents() { }

        #region Elements
        internal void BindElements()
        {
            FindElements();
            SubscribeElements();
        }

        internal void UnbindElements()
        {
            UnsubscribeScreenElements();
            ClearElements();
        }

        private void FindElements()
        {
            selectables = GetComponentsInChildren<ISelectable>(includeInactive: true);
            submitables = GetComponentsInChildren<ISubmitable>(includeInactive: true);
            cancelables = GetComponentsInChildren<ICancelable>(includeInactive: true);
        }

        private void ClearElements()
        {
            selectables = Array.Empty<ISelectable>();
            submitables = Array.Empty<ISubmitable>();
            cancelables = Array.Empty<ICancelable>();
        }

        private void SubscribeElements()
        {
            foreach (var selectable in selectables)
            {
                selectable.OnSelected += HandleAnyUISelected;
            }

            foreach (var submitable in submitables)
            {
                submitable.OnSubmitted += HandleAnyUISubmited;
            }

            foreach (var cancelable in cancelables)
            {
                cancelable.OnCanceled += HandleAnyUICanceled;
            }
        }

        private void UnsubscribeScreenElements()
        {
            foreach (var selectable in selectables)
            {
                selectable.OnSelected -= HandleAnyUISelected;
            }

            foreach (var submitable in submitables)
            {
                submitable.OnSubmitted -= HandleAnyUISubmited;
            }

            foreach (var cancelable in cancelables)
            {
                cancelable.OnCanceled += HandleAnyUICanceled;
            }
        }

        private void HandleAnyUISelected() => Menu.PlaySelectionAudio();
        private void HandleAnyUISubmited() => Menu.PlaySubmitionAudio();
        private void HandleAnyUICanceled() => Menu.PlayCancelationAudio();
        #endregion
    }
}