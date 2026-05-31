using UnityEngine;

namespace ActionCode.UISystem
{
    [DisallowMultipleComponent]
    public sealed class TabMenu : Menu
    {
        [Space]
        [SerializeField] private TabHeader header;
        [SerializeField] private TabContent content;

        public TabHeader Header => header;
        public TabContent Content => content;

        protected override void OnEnable()
        {
            base.OnEnable();
            SubscribeEvents();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnsubscribeEvents();
        }

        public void MoveLeft() => MoveToDirection(-1);
        public void MoveRight() => MoveToDirection(1);

        /// <summary>
        /// Moves to the given direction.
        /// Warps to the other side if <see cref="TabHeader.isWarpAllowed"/> is enabled.
        /// </summary>
        /// <param name="direction">The direction to warp. Positive to right, negative to left.</param>
        public void MoveToDirection(int direction)
        {
            if (direction == 0) return;

            var index = Header.GetMovedIndex(direction);
            Move(index);
        }

        public void Move(uint index)
        {
            var canMove = !IsOpening && Header.CurrentTab.Index != index;
            if (!canMove) return;

            Audio.PlayTabSelection();
            _ = OpenScreenAsync(Content.Tabs[index]);
        }

        protected override void StartOpenCurrentScreen()
        {
            base.StartOpenCurrentScreen();
            if (CurrentScreen is TabScreen tab) Select(tab.Index);
        }

        private void Select(uint index)
        {
            Header.Select(index);
            Content.Select(index);
        }

        private void SubscribeEvents()
        {
            Header.OnTabSwitched += HandleTabSwitched;
            Header.leftSwitchListener.OnActionPerformed.AddListener(MoveLeft);
            Header.rightSwitchListener.OnActionPerformed.AddListener(MoveRight);
        }

        private void UnsubscribeEvents()
        {
            Header.OnTabSwitched -= HandleTabSwitched;
            Header.leftSwitchListener.OnActionPerformed.RemoveListener(MoveLeft);
            Header.rightSwitchListener.OnActionPerformed.RemoveListener(MoveRight);
        }

        private void HandleTabSwitched(uint index) => Move(index);
    }
}