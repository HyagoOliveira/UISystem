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

        public void MoveLeft() => Move(-1);
        public void MoveRight() => Move(1);

        /// <summary>
        /// Moves to the given direction.
        /// Warps to the other side if <see cref="TabHeader.isWarpAllowed"/> is enabled.
        /// </summary>
        /// <param name="direction">The direction to warp. Positive to right, negative to left.</param>
        public void Move(int direction)
        {
            if (direction == 0) return;

            var index = Header.GetMovedIndex(direction);
            var canMove = Header.CurrentTab.Index != index;
            if (canMove) Select(index);
        }

        public override Awaitable OpenFirstScreenAsync()
        {
            if (firstScreen && firstScreen.TryGetComponent(out TabScreen tab))
            {
                Header.Select(tab.Index);
            }

            return base.OpenFirstScreenAsync();
        }


        private void Select(uint index)
        {
            Header.Select(index);
            Content.Select(index);
            Audio.PlayTabSelection();

            _ = OpenScreenAsync(Content.CurrentTab);
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

        private void HandleTabSwitched(uint index) => Select(index);
    }
}