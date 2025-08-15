using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract controller for a UI Toolkit document.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public abstract class AbstractController : MonoBehaviour
    {
        [SerializeField, Tooltip("The local UI Document component.")]
        private UIDocument document;

        /// <summary>
        /// The document Root Visual Element.
        /// </summary>
        public VisualElement Root => Document.rootVisualElement;

        /// <summary>
        /// The UI Document component.
        /// </summary>
        public UIDocument Document
        {
            get => document;
            set
            {
                UnsubscribeEvents();
                document = value;
                Reload();
            }
        }

        public bool IsEnabled => gameObject.activeInHierarchy;

        protected virtual void Reset() => document = GetComponent<UIDocument>();
        protected virtual void OnEnable() => Reload();
        protected virtual void OnDisable() => UnsubscribeEvents();

        public void Activate() => gameObject.SetActive(true);
        public void Deactivate() => gameObject.SetActive(false);
        public void Show() => SetVisibility(true);
        public void Hide() => SetVisibility(false);
        public void SetEnabled(bool enabled) => Root.SetEnabled(enabled);
        public void SetVisibility(bool visible) => Root.visible = visible;

        private void Reload()
        {
            FindReferences();
            SubscribeEvents();
            IgnorePickingMode();
        }

        /// <summary>
        /// Adds all necessary components for a menu.
        /// </summary>
        [ContextMenu("Setup Menu")]
        public void SetupForMenu()
        {
            TryAddComponent<ButtonClickAudioPlayer>();
            TryAddComponent<ElementHighlighter>();
            TryAddComponent<ElementFirstSelector>();
            TryAddComponent<ElementFocusAudioPlayer>();
        }

        public T Find<T>(string name) where T : VisualElement => Root.Find<T>(name);

        protected virtual void FindReferences() { }
        protected virtual void SubscribeEvents() { }
        protected virtual void UnsubscribeEvents() { }

        private void TryAddComponent<T>() where T : Component
        {
            var hasNoComponent = GetComponent<T>() == null;
            if (hasNoComponent) gameObject.AddComponent<T>();
        }

        private void IgnorePickingMode()
        {
            // Necessary to ignore the deselection behavior.
            // Check BackgroundClickDisabler for more information.
            Root.pickingMode = PickingMode.Ignore;
        }
    }
}