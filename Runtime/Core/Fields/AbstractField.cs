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
                this.value = value;
                ChangeValue(this.value);
            }
        }

        /// <summary>
        /// Event fired when the Value is changed.
        /// </summary>
        /// <remarks>
        /// It will not be fired if the value is set using <see cref="SetValueWithoutNotify(T)"/>.
        /// </remarks>
        public event Action<T> OnValueChanged;

        private T value;

        /// <summary>
        /// Sets the Value without invoking the <see cref="OnValueChanged"/> event.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public virtual void SetValueWithoutNotify(T value) => this.value = value;

        protected virtual void ChangeValue(T value) => OnValueChanged?.Invoke(value);
    }
}