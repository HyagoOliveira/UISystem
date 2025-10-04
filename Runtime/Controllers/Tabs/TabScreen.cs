using System;
using System.Collections.Generic;
using ActionCode.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Controller for a Tab Screen like Game Options.
    /// <para>
    /// You can move between tabs using UI Buttons or the Gamepad.
    /// </para>
    /// </summary>
    public sealed class TabScreen : AbstractMenuScreen
    {
        [Header("Tab")]
        [Tooltip("The name used to find the TabView element.")]
        public string tabViewName;
        [Tooltip("The first tab to activated when start.")]
        public AbstractTab firstTab;
        [Tooltip("If enabled, moving tab will warp from the other side when reaching the end.")]
        public bool isWarpAllowed = true;

        [Header("Audio")]
        [SerializeField, Tooltip("The Global Menu Data.")]
        private MenuData data;

        [Header("Input")]
        [Tooltip("The Input Action asset whet your move tabs input is")]
        public InputActionAsset input;
        [Tooltip("The 1D Axis input where negative moves to left and positive moves to the right.")]
        public InputActionPopup inputAction = new(nameof(input), "UI");

        /// <summary>
        /// The current selected tab index.
        /// Values bellow/above from the TabView capacity will be clamped.
        /// <para>Only sets the value if different from the current.</para>
        /// </summary>
        public int CurrentTabIndex
        {
            get => TabView.selectedTabIndex;
            set
            {
                var isNewValue = TabView.selectedTabIndex != value;
                if (isNewValue) TabView.selectedTabIndex = value;
            }
        }

        /// <summary>
        /// The input action used to move between tabs.
        /// <para>
        /// It's a 1D Axis input where negative moves to the left and positive moves to the right.
        /// </para>
        /// </summary>
        public InputAction InputAction { get; private set; }

        /// <summary>
        /// The TabView element.
        /// </summary>
        public TabView TabView { get; private set; }

        /// <summary>
        /// All available Tabs in this screen, indexed by their Tab element.
        /// </summary>
        public Dictionary<Tab, AbstractTab> Tabs { get; private set; }

        /// <summary>
        /// Action fired when tab changed.
        /// </summary>
        public event Action<AbstractTab> OnTabChanged;

        public override void Initialize(MenuController menu)
        {
            base.Initialize(menu);
            InputAction = input.FindAction(inputAction.GetPath());
        }

        public override void Focus()
        {
            TabView.Focus();
            if (firstTab) firstTab.Focus();
        }

        /// <summary>
        /// Moves to the given direction.
        /// Warps to the other direction if <see cref="isWarpAllowed"/> is enabled.
        /// </summary>
        /// <param name="direction">The direction to warp. Positive to right, negative to left.</param>
        public void Move(int direction)
        {
            if (direction == 0) return;

            var nextIndex = CurrentTabIndex + direction;
            var canMove = nextIndex >= 0 && nextIndex < Tabs.Count;

            if (canMove)
            {
                CurrentTabIndex += direction;
                return;
            }
            else if (isWarpAllowed) CurrentTabIndex = GetWarpedIndex(nextIndex);
        }

        public void MoveRight() => Move(1);
        public void MoveLeft() => Move(-1);
        public void PlaySelectionSound() => Menu.Audio.PlayOneShot(data.selectTab);

        protected override void FindReferences()
        {
            base.FindReferences();

            TabView = Find<TabView>(tabViewName);
            InitializeTabs();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            TabView.activeTabChanged += HandleActiveTabChanged;
            InputAction.performed += HandleInputActionPerformed;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            TabView.activeTabChanged -= HandleActiveTabChanged;
            InputAction.performed -= HandleInputActionPerformed;
        }

        private int GetWarpedIndex(int index) => index < 0 ? Tabs.Count - 1 : 0;

        private void InitializeTabs()
        {
            var tabs = GetComponentsInChildren<AbstractTab>(includeInactive: true);
            Tabs = new(tabs.Length);

            foreach (var tab in tabs)
            {
                var name = tab.GetName();
                var tabElement = Root.Find<Tab>(name);

                tab.Initialize(tabElement);
                Tabs.Add(tabElement, tab);
            }
        }

        private void HandleInputActionPerformed(InputAction.CallbackContext ctx)
        {
            // Cannot read int
            var direction = ctx.ReadValue<float>();
            Move((int)direction);
        }

        private void HandleActiveTabChanged(Tab _, Tab current)
        {
            var tab = Tabs[current];

            if (tab.TryGetFirstInput(out var input))
                Menu.FocusPlayer.FocusWithoutSound(input);

            PlaySelectionSound();
            OnTabChanged?.Invoke(tab);
        }
    }
}