using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using ActionCode.InputSystem;
using System.Collections;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract controller for a Game Load Screen using slots to show Game Saves.
    /// </summary>
    /// <remarks>
    /// List, load and delete game save files. You can further extend any behavior.
    /// </remarks>
    [RequireComponent(typeof(ListController))]
    public abstract class AbstractGameLoadScreen : AbstractMenuScreen
    {
        [SerializeField, Tooltip("The local List Controller component used to list the game save files.")]
        private ListController list;

        [Header("Data Names")]
        [SerializeField] private string dataDetailsName = "DataDetails";
        [SerializeField] private string availableDataContainerName = "AvailableData";
        [SerializeField] private string unavalibleDataContainerName = "UnavailableData";

        [Header("Inputs")]
        [SerializeField, Tooltip("The Input Asset where the bellow actions are.")]
        private InputActionAsset input;
        [SerializeField, Tooltip("The delete input action used to Delete the current data file.")]
        private InputActionPopup deleteInput = new(nameof(input), "UI", "Delete");

        public VisualElement DataDetails { get; private set; }
        public VisualElement AvailableDataContainer { get; private set; }
        public VisualElement UnavailableDataContainer { get; private set; }

        private InputAction deleteAction;

        protected override void Reset()
        {
            base.Reset();
            list = GetComponent<ListController>();
        }

        protected virtual void Awake() => FindActions();

        public override void Focus()
        {
            if (list.HasItems()) list.Focus();
            else UnavailableDataContainer.Focus();
        }

        public override async Awaitable LoadAnyContent() => await LoadDataAsync();

        protected abstract void SelectData(object item);
        protected abstract void ConfirmDataLoad(object item);

        protected abstract Awaitable<bool> TryDeleteDataAsync(object item);
        protected abstract Awaitable<IList> LoadDataListAsync();

        protected override void FindReferences()
        {
            base.FindReferences();

            DataDetails = Find<VisualElement>(dataDetailsName);
            AvailableDataContainer = Find<VisualElement>(availableDataContainerName);
            UnavailableDataContainer = Find<VisualElement>(unavalibleDataContainerName);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            list.GetItemText = GetDataText;
            list.GetItemName = GetDataName;

            list.OnItemSelected += HandleDataSelected;
            list.OnItemConfirmed += HandleDataConfirmed;

            deleteAction.performed += HandleDeleteActionPerformed;
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            list.GetItemText = null;
            list.GetItemName = null;

            list.OnItemSelected -= HandleDataSelected;
            list.OnItemConfirmed -= HandleDataConfirmed;

            deleteAction.performed -= HandleDeleteActionPerformed;
        }

        protected async Awaitable LoadDataAsync()
        {
            AvailableDataContainer.SetDisplayEnabled(false);
            UnavailableDataContainer.SetDisplayEnabled(false);

            var dataList = await LoadDataListAsync();
            var hasAnyData = dataList.Count > 0;

            if (hasAnyData)
            {
                list.SetSource(dataList);
                AvailableDataContainer.SetDisplayEnabled(true);
            }
            else UnavailableDataContainer.SetDisplayEnabled(true);
        }

        protected virtual void FindActions()
        {
            deleteAction = input.FindAction(deleteInput.GetPath());
            deleteAction.Disable();
        }

        private void HandleDataSelected(object item)
        {
            var hasItem = item != null;
            if (hasItem) SelectData(item);
            deleteAction.SetEnabled(hasItem);
        }

        private void HandleDataConfirmed(object item)
        {
            var hasInvalidItem = item == null;
            if (hasInvalidItem) return;

            ConfirmDataLoad(item);
            MenuController.SetSendNavigationEvents(false);
        }

        private void HandleDeleteActionPerformed(InputAction.CallbackContext _)
        {
            Popups.Dialogue.Show(
                message: new LocalizedString("LoadMenu", "confirm_message", "Are you sure?"),
                title: new LocalizedString("LoadMenu", "delete_title", "Deleting data"),
                onConfirm: DeleteSelectedData
            );
        }

        private async void DeleteSelectedData()
        {
            var item = list.SelectedItem;
            var hasInvalidItem = item == null;
            if (hasInvalidItem) return;

            var wasDeleted = await TryDeleteDataAsync(item);
            if (wasDeleted) await LoadDataAsync();
        }

        private static string GetDataName(object item) => $"data-slot-{item}";

        private static string GetDataText(object _, int index)
        {
            var localized = new LocalizedString("LoadMenu", "profile_text", "Slot");
            return $"{localized.GetLocalizedText()} {index + 1}";
        }
    }
}