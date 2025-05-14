using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract controller for a UI Toolkit Tab element.
    /// </summary>
    public abstract class AbstractTab : MonoBehaviour
    {
        /// <summary>
        /// The document Root Visual Element.
        /// </summary>
        public Tab Tab { get; private set; }

        public bool IsEnabled => gameObject.activeInHierarchy;

        protected virtual void OnDisable() => UnsubscribeEvents();

        public void Initialize(Tab tab)
        {
            Tab = tab;
            FindReferences();
            SubscribeEvents();
        }

        public T Find<T>(string name) where T : VisualElement => Tab.Find<T>(name);
        public virtual Button GetFirstButton() => Tab.Q<Button>();

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