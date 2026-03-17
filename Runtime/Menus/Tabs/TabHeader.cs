using System;
using UnityEngine;
using ActionCode.InputSystem;

namespace ActionCode.UISystem
{
    [DisallowMultipleComponent]
    public sealed class TabHeader : MonoBehaviour
    {
        [Tooltip("If enabled, moving a Tab will warp from one side to another.")]
        public bool isWarpAllowed = true;
        public ActionPerformedListener leftSwitchListener;
        public ActionPerformedListener rightSwitchListener;

        public event Action<uint> OnTabSwitched;

        public TabButton[] Tabs { get; private set; }
        public TabButton CurrentTab { get; private set; }

        private void Awake() => InitializeTabs();
        private void OnEnable() => SubscribeEvents();
        private void OnDisable() => UnsubscribeEvents();

        private void InitializeTabs()
        {
            Tabs = GetComponentsInChildren<TabButton>(includeInactive: true);
            for (uint i = 0; i < Tabs.Length; i++)
            {
                Tabs[i].Initialize(i);
            }
        }

        internal void Select(uint index)
        {
            CurrentTab = Tabs[index];
            SelectOnlyCurrentTab();
        }

        internal uint GetMovedIndex(int direction)
        {
            var nextIndex = (int)CurrentTab.Index + direction;
            var isWarping = nextIndex < 0 || nextIndex >= Tabs.Length;
            var canWarp = isWarping && isWarpAllowed;

            if (canWarp) nextIndex = nextIndex < 0 ? Tabs.Length - 1 : 0;
            else nextIndex = Mathf.Clamp(nextIndex, 0, Tabs.Length - 1);

            return (uint)nextIndex;
        }

        private void SelectOnlyCurrentTab()
        {
            foreach (var tab in Tabs)
            {
                tab.Unselect();
            }

            CurrentTab.Select();
        }

        private void SubscribeEvents()
        {
            foreach (var tab in Tabs)
            {
                tab.OnSwitched += HandleTabSwitched;
            }
        }

        private void UnsubscribeEvents()
        {
            foreach (var tab in Tabs)
            {
                tab.OnSwitched -= HandleTabSwitched;
            }
        }

        private void HandleTabSwitched(uint index) => OnTabSwitched?.Invoke(index);
    }
}