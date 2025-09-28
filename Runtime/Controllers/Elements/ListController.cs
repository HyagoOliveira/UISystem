using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Controller for a traditional UI Game List using <see cref="ListView"/>.
    /// Use the available events to handle item selection or confirmation.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ListController : AbstractController
    {
        [Tooltip("The Global Menu Data.")]
        public MenuData data;

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
        /// The ScrollView from the List.
        /// </summary>
        public ScrollView Scroll { get; private set; }

        /// <summary>
        /// The selected item from the list data source.
        /// </summary>
        public object SelectedItem => List.selectedItem;

        /// <summary>
        /// Function called when setting the item element name, used as its id.
        /// </summary>
        public Func<object, string> GetItemName { get; set; }

        /// <summary>
        /// Function called when settings the item display text.
        /// </summary>
        public Func<object, int, string> GetItemText { get; set; }

        /// <summary>
        /// The AudioSource used to play the sounds.
        /// </summary>
        public AudioSource Audio { get; private set; }

        /// <summary>
        /// The optional Menu Controller this List belongs to.
        /// </summary>
        public MenuController Menu { get; private set; }

        /// <summary>
        /// The last selected index.
        /// </summary>
        public int LasSelectedIndex { get; private set; } = -1;

        private void Awake()
        {
            Audio = GetComponentInParent<AudioSource>();
            Menu = GetComponentInParent<MenuController>();
        }

        /// <summary>
        /// Whether the List has any item.
        /// </summary>
        /// <returns>Always a boolean.</returns>
        public bool HasItems() => List?.itemsSource?.Count > 0;

        /// <summary>
        /// Focus the ListView.
        /// </summary>
        public override void Focus() => List.Focus();

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

        /// <summary>
        /// Deletes the List.
        /// </summary>
        public void Delete() => List.RemoveFromHierarchy();

        protected override void FindReferences()
        {
            base.FindReferences();

            List = Find<ListView>(listName);
            Scroll = List.Q<ScrollView>();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            List.makeItem += HandleItemMaked;
            List.bindItem += HandleItemBinded;
            List.itemsChosen += HandleItemsChosen; // Necessary to invoke Gamepad submit event
            List.selectionChanged += HandleSelectionChanged;

            Scroll.contentContainer.RegisterCallback<NavigationCancelEvent>(HandleNavigationCancelEvent);
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            List.makeItem -= HandleItemMaked;
            List.bindItem -= HandleItemBinded;
            List.itemsChosen -= HandleItemsChosen;
            List.selectionChanged -= HandleSelectionChanged;

            Scroll.contentContainer.UnregisterCallback<NavigationCancelEvent>(HandleNavigationCancelEvent);
        }

        private VisualElement HandleItemMaked() => new Label();

        private void HandleItemBinded(VisualElement element, int index)
        {
            var label = element as Label;
            var item = List.itemsSource[index];

            if (itemStyle) label.styleSheets.Add(itemStyle);

            label.name = GetItemName?.Invoke(item);
            label.text = GetItemText?.Invoke(item, index);
            label.focusable = false;

            label.RegisterCallback<ClickEvent>(HandleItemClicked);
            label.RegisterCallback<PointerEnterEvent>(_ => List.SetSelection(index));
        }

        private void HandleItemClicked(ClickEvent _) => ConfirmItem();
        private void HandleSelectionChanged(IEnumerable _) => SelectItem();
        private void HandleItemsChosen(IEnumerable<object> _) => ConfirmItem();

        private void HandleNavigationCancelEvent(NavigationCancelEvent evt)
        {
            if (Menu == null) return;

            evt.StopPropagation();
            MenuController.SetSendNavigationEvents(false);
            Menu.OnCancel();
        }

        private void SelectItem()
        {
            var item = List.selectedItem;
            if (item == null)
            {
                // Prevents list from losing focus.
                // (NavigationMoveEvent does not work on ListView)
                List.Select(LasSelectedIndex);
                return;
            }

            var wasSame = LasSelectedIndex == List.selectedIndex;
            if (wasSame) return;

            LasSelectedIndex = List.selectedIndex;
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

        private void PlayConfirmSound() => Audio.PlayOneShot(data.submit);
        private void PlaySelectionSound() => Audio.PlayOneShot(data.selection);
    }
}