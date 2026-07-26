using ActionCode.InputSystem;
using System;
using UnityEngine;

namespace ActionCode.UISystem
{
    [DisallowMultipleComponent]
    public sealed class TabHeader : MonoBehaviour
    {
        [Tooltip("If enabled, moving a Tab will warp from one side to another.")]
        public bool isWarpAllowed = true;

        [Space]
        public ActionPerformedListener leftSwitchListener;
        public ActionPerformedListener rightSwitchListener;

        public event Action<uint> OnTabSwitched;

        public TabButton[] Tabs { get; private set; }
        public TabButton CurrentTab { get; private set; }

        private void Awake() => InitializeTabs();
        private void OnEnable() => SubscribeEvents();
        private void OnDisable() => UnsubscribeEvents();

        public void SwitchTab(uint index) => OnTabSwitched?.Invoke(index);
        public void MoveLeft() => Move(-1);
        public void MoveRight() => Move(1);

        public void Move(int direction)
        {
            if (direction == 0) return;

            var index = GetMovedIndex(direction);
            var canMove = CurrentTab.Index != index;
            if (canMove) SwitchTab(index);
        }

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

            leftSwitchListener.OnActionPerformed.AddListener(HandleLeftSwitchPerformed);
            rightSwitchListener.OnActionPerformed.AddListener(HandleRightSwitchPerformed);
        }

        private void UnsubscribeEvents()
        {
            foreach (var tab in Tabs)
            {
                tab.OnSwitched -= HandleTabSwitched;
            }

            leftSwitchListener.OnActionPerformed.RemoveListener(HandleLeftSwitchPerformed);
            rightSwitchListener.OnActionPerformed.RemoveListener(HandleRightSwitchPerformed);
        }

        private void HandleLeftSwitchPerformed() => MoveLeft();
        private void HandleRightSwitchPerformed() => MoveRight();
        private void HandleTabSwitched(uint index) => SwitchTab(index);
    }
}