using System;
using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract button used to hold a generic data. 
    /// You can subscribe to a Clicked and Selected events.
    /// </summary>
    /// <remarks>
    /// Implement this class when creating slot buttons to Load game, show characters information etc.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    [DisallowMultipleComponent]
    public abstract class AbstractActionDataButton<T> : ActionButton where T : class
    {
        public T Data { get; private set; }
        public int Slot { get; private set; }

        public event Action<T, int> OnDataClicked;
        public event Action<T, int> OnDataSelected;

        public bool HasData() => Data != null;
        public void SetData(T data) => Data = data;
        public void SetSlot(int slot) => Slot = slot;

        public virtual void SetDataAndSlot(T data, int slot)
        {
            SetData(data);
            SetSlot(slot);
        }

        public override string ToString()
        {
            var data = HasData() ? Data.ToString() : "No Data";
            return $"{gameObject.name} ({data})";
        }

        protected override void HandleClicked()
        {
            base.HandleClicked();
            OnDataClicked?.Invoke(Data, Slot);
        }

        protected override void HandleSelection()
        {
            base.HandleSelection();
            OnDataSelected?.Invoke(Data, Slot);
        }
    }
}