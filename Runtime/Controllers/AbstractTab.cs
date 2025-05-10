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

        public void Initialize(Tab tab)
        {
            Tab = tab;
            FindReferences();
            SubscribeEvents();
        }

        protected virtual void OnDisable() => UnsubscribeEvents();

        public T Find<T>(string name) where T : VisualElement => Tab.Find<T>(name);

        protected virtual void FindReferences() { }
        protected virtual void SubscribeEvents() { }
        protected virtual void UnsubscribeEvents() { }
        public virtual Button GetFirstButton() => Tab.Q<Button>();
    }
}