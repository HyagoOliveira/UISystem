using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityLocalizedString = UnityEngine.Localization.LocalizedString;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Field for a Horizontal Carousel Dropdown.
    /// </summary>
    /// <remarks>
    /// Values can be set using an Enum type or a custom array of values.<br/>
    /// Player can navigate between values using the gamepad/keyboard left/right buttons.<br/>
    /// Available values are displayed in a horizontal carousel using small dots.
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ColorTransition))]
    public sealed class DropdownField : AbstractField<string>, IClickable
    {
        [Space]
        [Tooltip("The Enum to use in the Dropdown Values. Use the fully qualified enum type name (e.g. UnityEngine.RuntimePlatform)")]
        [SerializeField] private string enumTypeName;
        [SerializeField] private Label displayValue;
        [SerializeField] private DropdownDots dots;

        [Header("BUTTONS")]
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;

        [Header("LOCALIZATION")]
        [SerializeField, Tooltip("[Optional] The Localization Keys where the Dropdown values are stored.")]
        private UnityLocalizedString[] localizationKeys;

        public event Action OnClicked;
        public event Action<Enum> OnEnumValueSubmitted;

        public int CurrentIndex { get; private set; }

        private Type enumType;
        private string[] values;
        private AudioHandler audioHandler;

        protected override void Awake()
        {
            base.Awake();
            if (IsRunning()) FindComponents();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (IsRunning()) SubscribeEvents();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (IsRunning()) UnsubscribeEvents();
        }

        #region MOVE
        public void MoveLeft() => Move(-1);
        public void MoveRight() => Move(1);

        public void Move(int direction)
        {
            if (values.Length == 0) return;

            CurrentIndex = (CurrentIndex + direction + values.Length) % values.Length;
            UpdateValue();

            if (audioHandler) audioHandler.PlayTabSelection();
        }

        private void SubscribeEvents()
        {
            leftButton.onClick.AddListener(HandleLeftButtonClicked);
            rightButton.onClick.AddListener(HandleRightButtonClicked);
        }

        private void UnsubscribeEvents()
        {
            leftButton.onClick.RemoveListener(HandleLeftButtonClicked);
            rightButton.onClick.RemoveListener(HandleRightButtonClicked);
        }

        private void HandleLeftButtonClicked() => MoveLeft();
        private void HandleRightButtonClicked() => MoveRight();
        #endregion

        #region ENUM
        public bool HasValues() => values != null && values.Length > 0;
        public bool HasEnumSource() => !string.IsNullOrEmpty(enumTypeName);
        public bool TryGetValue<TEnum>(out TEnum value) where TEnum : struct => Enum.TryParse(Value, out value);
        public string GetCurrentValue() => HasValues() ? values.GetValue(CurrentIndex).ToString() : string.Empty;

        /// <summary>
        /// Set the Dropbox values using the given enum type.
        /// </summary>
        /// <param name="enumTypeName">
        /// The Enum to use in the Dropdown Values. Use the enum type full name (e.g. UnityEngine.RuntimePlatform)
        /// </param>
        public void SetEnumSource(string enumTypeName)
        {
            var isEnum = TryGetEnumType(enumTypeName, out enumType);
            if (!isEnum)
            {
                Debug.LogError($"Could not load enum '{enumTypeName}'. Ensure you are using the Full Name (Namespace.TypeName).");
                return;
            }

            SetValues(Enum.GetNames(enumType));
        }

        /// <summary>
        /// <inheritdoc cref="SetEnumSource(string)"/>
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        public void SetEnumSource<T>() where T : Enum
        {
            var type = typeof(T);
            SetValues(Enum.GetNames(type));
        }

        /// <summary>
        /// Sets the current value based on the specified enum value if it exists.
        /// </summary>
        /// <param name="value">The enum value to set.</param>
        public void SetEnumValue(Enum value)
        {
            if (!HasValues() || !TryGetIndex(value.ToString(), out int index)) return;
            CurrentIndex = index;
            UpdateValue();
        }

        /// <summary>
        /// Set the Dropbox values using the given values.
        /// </summary>
        /// <param name="values">The Dropdown values.</param>
        public void SetValues(string[] values)
        {
            dots.Destroy();
            CurrentIndex = 0;
            this.values = values;

            dots.Spawn(this.values.Length);
            UpdateValue();
        }

        public void Clear()
        {
            dots.Destroy();
            CurrentIndex = 0;
            values = Array.Empty<string>();
            displayValue.Text = string.Empty;
        }

        public void SetValueWithoutNotify(Enum value) => SetValueWithoutNotify(value.ToString());

        public override void SetValueWithoutNotify(string value)
        {
            if (!HasValues() || !TryGetIndex(value, out int index)) return;

            CurrentIndex = index;
            UpdateDisplay();
            base.SetValueWithoutNotify(value);
        }

        private bool TryGetIndex(string target, out int index)
        {
            index = Array.IndexOf(values, target);
            var hasIndex = index >= 0;
            if (!hasIndex) Debug.LogError($"Could no find '{target}'. It's not a valid Enum value.");
            return hasIndex;
        }

        private bool TryGetEnumType(string typeName, out Type type)
        {
            type = Type.GetType(typeName) ??
                AppDomain.CurrentDomain.GetAssemblies()
                    .Select(a => a.GetType(typeName))
                    .FirstOrDefault(t => t != null);
            return type?.IsEnum == true;
        }
        #endregion

        #region INTERFACES
        // Triggered when Mouse clicks on it
        public void OnPointerClick(PointerEventData evt)
        {
            var isLeftClick = evt.button == PointerEventData.InputButton.Left;
            if (isLeftClick) Press();
        }

        // Triggered when Gamepad/Keyboard submits or Mouse clicks on it
        public void OnSubmit(BaseEventData _) => Press();

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);

            switch (eventData.moveDir)
            {
                case MoveDirection.Right:
                    MoveRight();
                    break;

                case MoveDirection.Left:
                    MoveLeft();
                    break;
            }
        }

        private void Press()
        {
            if (!IsAvailable()) return;

            OnClicked?.Invoke();

            if (enumType == null) return;
            var hasEnum = Enum.TryParse(enumType, Value, out var result);
            if (hasEnum) OnEnumValueSubmitted?.Invoke((Enum)result);
        }
        #endregion

        #region INITIALIZATION
        protected override void ResetFields()
        {
            base.ResetFields();
            dots = GetComponentInChildren<DropdownDots>();
        }

        private void FindComponents()
        {
            if (HasEnumSource()) SetEnumSource(enumTypeName);
            audioHandler = GetComponentInParent<AudioHandler>();
        }

        private void UpdateValue()
        {
            Value = GetCurrentValue();
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            dots.Select(CurrentIndex);

            var hasLocalization = TryGetCurrentLocalization(out var localization);
            if (hasLocalization) displayValue.UpdateLocalization(localization);
            else
            {
                displayValue.ClearLocalization();
                displayValue.Text = Value;
            }
        }

        private bool TryGetCurrentLocalization(out UnityLocalizedString localization)
        {
            var hasLocalization = localizationKeys.Length > CurrentIndex;
            localization = hasLocalization ? localizationKeys[CurrentIndex] : null;
            return localization != null;
        }
        #endregion
    }
}