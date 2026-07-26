using UnityEngine;

namespace ActionCode.UISystem
{
    [DisallowMultipleComponent]
    public sealed class TabMenu : Menu
    {
        [Space]
        [Tooltip("If true, the menu will be activated using the last opened Tab.")]
        public bool useLastTab;

        [Space]
        [SerializeField] private TabHeader header;
        [SerializeField] private TabContent content;

        public TabHeader Header => header;
        public TabContent Content => content;

        protected override void Reset()
        {
            base.Reset();
            header = GetComponentInChildren<TabHeader>();
            content = GetComponentInChildren<TabContent>();
        }

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

        public override async Awaitable OpenScreenAsync(string identifier, bool undoable = false)
        {
            var isOpeningFirstTab = !IsOpened;
            await base.OpenScreenAsync(identifier, undoable);

            var hasScreen = Screens.TryGetValue(identifier, out var screen);
            if (hasScreen && screen is TabScreen tab)
            {
                if (useLastTab) firstScreen = screen;
                Select(tab.Index, playAudio: !isOpeningFirstTab);
            }
        }

        private void Select(uint index, bool playAudio = true)
        {
            Header.Select(index);
            Content.Select(index);
            if (playAudio) Audio.PlayTabSelection();
        }

        private void SubscribeEvents() => Header.OnTabSwitched += HandleTabSwitched;
        private void UnsubscribeEvents() => Header.OnTabSwitched -= HandleTabSwitched;
        private void HandleTabSwitched(uint index) => _ = OpenScreenAsync(Content.Tabs[index]);
    }
}