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

        public event Action<T> OnDataClicked;
        public event Action<T> OnDataSelected;

        public bool HasData() => Data != null;
        public void SetData(T data) => Data = data;

        public override string ToString()
        {
            var data = HasData() ? Data.ToString() : "No Data";
            return $"{gameObject.name} ({data})";
        }

        protected override void HandleClicked()
        {
            base.HandleClicked();
            OnDataClicked?.Invoke(Data);
        }

        protected override void HandleSelection()
        {
            base.HandleSelection();
            OnDataSelected?.Invoke(Data);
        }
    }
}