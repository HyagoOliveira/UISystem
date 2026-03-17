using UnityEngine;

namespace ActionCode.UISystem
{
    [DisallowMultipleComponent]
    public class TabScreen : BaseScreen
    {
        public uint Index { get; private set; }

        internal void Initialize(uint index) => Index = index;
    }
}