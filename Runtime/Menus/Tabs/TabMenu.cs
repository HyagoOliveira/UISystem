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
            if (canMove) _ = OpenScreenAsync(Content.Tabs[index]);
        }

        public override async Awaitable OpenScreenAsync(string identifier, bool undoable = false)
        {
            var isOpeningFirstTab = !IsActive;
            await base.OpenScreenAsync(identifier, undoable);
            var hasScreen = Screens.TryGetValue(identifier, out var screen);
            if (hasScreen && screen is TabScreen tab) Select(tab.Index, playAudio: !isOpeningFirstTab);
        }

        private void Select(uint index, bool playAudio = true)
        {
            Header.Select(index);
            Content.Select(index);
            if (playAudio) Audio.PlayTabSelection();
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