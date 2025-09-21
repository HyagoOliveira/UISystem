using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract controller for a UI Toolkit Tab element.
    /// </summary>
    public abstract class AbstractTab : MonoBehaviour
    {
        [SerializeField] private string firstInput;

        /// <summary>
        /// The document Root Visual Element.
        /// </summary>
        public Tab Tab { get; private set; }

        public bool IsEnabled => gameObject.activeInHierarchy;

        private void OnEnable()
        {
            FindReferences();
            SubscribeEvents();
        }

        private void OnDisable() => UnsubscribeEvents();

        public void Initialize(Tab tab) => Tab = tab;

        public virtual string GetName() => GetType().Name;
        public T Find<T>(string name) where T : VisualElement => Tab.Find<T>(name);

        public bool TryGetFirstInput(out VisualElement input)
        {
            input = Find<VisualElement>(firstInput);
            return input != null;
        }

        public virtual void Focus()
        {
            if (TryGetFirstInput(out VisualElement input))
                input.Focus();
        }

        protected virtual void FindReferences() { }
        protected virtual void SubscribeEvents() { }
        protected virtual void UnsubscribeEvents() { }

        protected void EnableButtons() => SetButtonsEnabled(true);
        protected void DisableButtons() => SetButtonsEnabled(false);

        private void SetButtonsEnabled(bool enabled)
        {
            foreach (var button in Tab.Query<Button>().ToList())
            {
                button.SetEnabled(enabled);
            }
        }
    }
}