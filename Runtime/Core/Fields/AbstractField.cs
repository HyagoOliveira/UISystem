using System;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract class for fields with value notifying when changed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractField<T> : ActionSelectable
    {
        /// <summary>
        /// The current Value.
        /// </summary>
        public T Value
        {
            get => value;
            set
            {
                if (value.Equals(this.value)) return;

                if (IsReadOnly)
                {
                    DenyReadOnlyValueChange();
                    return;
                }

                this.value = value;
                ChangeValue(this.value);
            }
        }

        /// <summary>
        /// Whether the value is read only and cannot be modified.
        /// <para>
        /// <see cref="OnReadOnlyValueChangeDenied"/> event will be fired when trying to change read only values.
        /// </para>
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Event fired when the Value is changed.
        /// </summary>
        /// <remarks>
        /// It will not be fired if the value is set using <see cref="SetValueWithoutNotify(T)"/>.
        /// </remarks>
        public event Action<T> OnValueChanged;

        /// <summary>
        /// Event fired when trying to change a read only Value, when <see cref="IsReadOnly"/> is true.
        /// </summary>
        public event Action OnReadOnlyValueChangeDenied;

        private T value;

        /// <summary>
        /// Sets the Value without invoking the <see cref="OnValueChanged"/> event.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public virtual void SetValueWithoutNotify(T value) => this.value = value;

        protected virtual void ChangeValue(T value) => OnValueChanged?.Invoke(value);
        protected virtual void DenyReadOnlyValueChange() => OnReadOnlyValueChangeDenied?.Invoke();
    }
}