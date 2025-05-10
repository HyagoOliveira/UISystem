using UnityEngine;
using UnityEngine.UIElements;
using ActionCode.InputSystem;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Controller for a TabView element with sound.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public sealed class TabViewController : AbstractController
    {
        [Tooltip("The name used to find the TabView element.")]
        public string tabViewName;
        [Tooltip("If enabled, moving tab will warp from the other side when reaching the end.")]
        public bool isWarpAllowed = true;

        [Header("Audio")]
        [Tooltip("The local AudioSource component.")]
        public AudioSource source;
        [Tooltip("The audio played when tab is moved.")]
        public AudioClip moveSound;

        [Header("Input")]
        [SerializeField, Tooltip("The Input Action asset whet your move tabs input is")]
        private InputActionAsset input;
        [SerializeField, Tooltip("The 1D Axis input where negative moves to left and positive moves to the right.")]
        private InputActionPopup inputAction = new(nameof(input), "UI");

        /// <summary>
        /// The input action used to move between tabs.
        /// <para>
        /// It's a 1D Axis input where negative moves to the left and positive moves to the right.
        /// </para>
        /// </summary>
        public InputAction InputAction { get; private set; }

        /// <summary>
        /// The TabView element.
        /// </summary>
        public TabView TabView { get; private set; }

        /// <summary>
        /// All the Tabs.
        /// </summary>
        public List<Tab> Tabs { get; private set; }

        /// <summary>
        /// The current selected tab index.
        /// Values bellow/above from the TabView capacity will be clamped.
        /// <para>Only sets the value if different from the current.</para>
        /// </summary>
        public int CurrentTabIndex
        {
            get => TabView.selectedTabIndex;
            set
            {
                var isNewValue = TabView.selectedTabIndex != value;
                if (isNewValue) TabView.selectedTabIndex = value;
            }
        }

        protected override void Reset()
        {
            base.Reset();
            source = GetComponent<AudioSource>();
        }

        private void Awake() => FindInputAction();

        /// <summary>
        /// Moves to the given direction.
        /// Warps to the other direction if <see cref="isWarpAllowed"/> is enabled.
        /// </summary>
        /// <param name="direction">The direction to warp. Positive to right, negative to left.</param>
        public void Move(int direction)
        {
            if (direction == 0) return;

            var nextIndex = CurrentTabIndex + direction;
            var canMove = nextIndex >= 0 && nextIndex < Tabs.Count;

            if (canMove)
            {
                CurrentTabIndex += direction;
                return;
            }
            else if (isWarpAllowed) CurrentTabIndex = GetWarpedIndex(nextIndex);
        }

        public void MoveRight() => Move(1);
        public void MoveLeft() => Move(-1);
        public void PlayMoveSound() => source.PlayOneShot(moveSound);

        protected override void FindReferences()
        {
            base.FindReferences();
            TabView = Find<TabView>(tabViewName);
            Tabs = TabView.Query<Tab>().ToList();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            TabView.activeTabChanged += HandleActiveTabChanged;
            InputAction.performed += HandleInputActionPerformed;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            TabView.activeTabChanged -= HandleActiveTabChanged;
            InputAction.performed -= HandleInputActionPerformed;
        }

        private int GetWarpedIndex(int index) => index < 0 ? Tabs.Count - 1 : 0;

        private void FindInputAction() => InputAction = input.FindAction(inputAction.GetPath());

        private void HandleInputActionPerformed(InputAction.CallbackContext ctx)
        {
            // Cannot read int
            var direction = ctx.ReadValue<float>();
            Move((int)direction);
        }

        private void HandleActiveTabChanged(Tab _, Tab __) => PlayMoveSound();
    }
}