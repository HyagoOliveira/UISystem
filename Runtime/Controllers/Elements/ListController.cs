using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Controller for a traditional UI Game List using <see cref="ListView"/>.
    /// Use the available events to handle item selection or confirmation.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class ListController : AbstractController
    {
        [SerializeField, Tooltip("The Global Menu Data.")]
        private MenuData data;
        [SerializeField, Tooltip("The local AudioSource component.")]
        private AudioSource source;

        [Space]
        [Tooltip("The ListView name inside the UI Document.")]
        public string listName;
        [Tooltip("The optional style sheet added for each list item.")]
        public StyleSheet itemStyle;

        /// <summary>
        /// Event fired when a list item is selected by the gamepad/keyboard navigation buttons or by mouse hover.
        /// <para>The given item param will never be null.</para>
        /// </summary>
        public event Action<object> OnItemSelected;

        /// <summary>
        /// Event fired when a list item is confirmed by the gamepad/keyboard submit button or by mouse click.
        /// <para>The given item param will never be null.</para>
        /// </summary>
        public event Action<object> OnItemConfirmed;

        /// <summary>
        /// The current List View.
        /// </summary>
        public ListView List { get; private set; }

        /// <summary>
        /// The selected item from the list data source.
        /// </summary>
        public object SelectedItem => List.selectedItem;

        public Func<object, string> GetItemName { get; set; }
        public Func<object, string> GetItemText { get; set; }

        protected override void Reset()
        {
            base.Reset();
            source = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Set the ListView source and the selected index.
        /// </summary>
        /// <param name="source">The ListView source.</param>
        /// <param name="index">The selected index.</param>
        public void SetSource(IList source, int index = 0)
        {
            // Triggers all ListView callbacks
            List.itemsSource = source;
            List.Select(index);
        }

        protected override void FindReferences()
        {
            base.FindReferences();
            List = Find<ListView>(listName);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            List.makeItem += HandleItemMaked;
            List.bindItem += HandleItemBinded;
            List.itemsChosen += HandleItemsChosen; // Necessary to invoke Gamepad submit event
            List.selectionChanged += HandleSelectionChanged;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            List.makeItem -= HandleItemMaked;
            List.bindItem -= HandleItemBinded;
            List.itemsChosen -= HandleItemsChosen;
            List.selectionChanged -= HandleSelectionChanged;
        }

        private VisualElement HandleItemMaked() => new Label();

        private void HandleItemBinded(VisualElement element, int index)
        {
            var label = element as Label;
            var item = List.itemsSource[index];

            if (itemStyle) label.styleSheets.Add(itemStyle);

            label.name = GetItemName?.Invoke(item);
            label.text = GetItemText?.Invoke(item);

            label.RegisterCallback<ClickEvent>(HandleItemClicked);
            label.RegisterCallback<PointerEnterEvent>(_ => List.SetSelection(index));
        }

        private void HandleItemClicked(ClickEvent _) => ConfirmItem();
        private void HandleSelectionChanged(IEnumerable _) => SelectItem();
        private void HandleItemsChosen(IEnumerable<object> _) => ConfirmItem();

        private void SelectItem()
        {
            var item = List.selectedItem;
            if (item == null)
            {
                // Prevents list from losing focus.
                // (NavigationMoveEvent does not work on ListView)
                List.SelectFirst();
                return;
            }

            PlaySelectionSound();
            OnItemSelected?.Invoke(item);
        }

        private void ConfirmItem()
        {
            var item = List.selectedItem;
            if (item == null) return;

            PlayConfirmSound();
            OnItemConfirmed?.Invoke(item);
        }

        private void PlayConfirmSound() => source.PlayOneShot(data.submit);
        private void PlaySelectionSound() => source.PlayOneShot(data.selection);
    }
}