using UnityEngine;

namespace ActionCode.UISystem
{
    [DisallowMultipleComponent]
    public sealed class TabContent : MonoBehaviour
    {
        public TabScreen[] Tabs { get; private set; }
        public TabScreen CurrentTab { get; private set; }

        private void Awake() => InitializeTabs();

        private void InitializeTabs()
        {
            Tabs = GetComponentsInChildren<TabScreen>(includeInactive: true);
            for (uint i = 0; i < Tabs.Length; i++)
            {
                Tabs[i].Initialize(i);
            }
        }

        internal void Select(uint index) => CurrentTab = Tabs[index];
    }
}