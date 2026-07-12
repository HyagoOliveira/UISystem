using ActionCode.LocalizationSystem;
using System;
using UnityEngine;

namespace ActionCode.UISystem
{
    [DisallowMultipleComponent]
    public sealed class ActionSlotButton : ActionButton
    {
        public uint Slot { get; private set; }

        public event Action<uint> OnSlotClicked;
        public event Action<uint> OnSlotSelected;

        public void SetSlot(uint slot) => Slot = slot;

        public void SetSlotAndLocalization(uint slot, string localizationIndexName)
        {
            SetSlot(slot);
            Label.Localization.StringReference.UpdateDynamicLocalization(localizationIndexName, Slot + 1);
        }

        protected override void HandleClicked()
        {
            base.HandleClicked();
            OnSlotClicked?.Invoke(Slot);
        }

        protected override void HandleSelection()
        {
            base.HandleSelection();
            OnSlotSelected?.Invoke(Slot);
        }
    }
}