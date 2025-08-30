using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    public abstract class AbstractElement<T> : MonoBehaviour, IDisposable where T : VisualElement
    {
        private UQueryBuilder<T> elements;

        public bool IsInitialized() => elements == default;

        public void Initialize(VisualElement root)
        {
            elements = root.Query<T>(className: GetClassName());
            elements.ForEach(RegisterEvent);
        }

        public void Dispose()
        {
            if (!IsInitialized()) return;

            elements.ForEach(UnregisterEvent);
            elements = default;
        }

        protected abstract string GetClassName();
        protected abstract void RegisterEvent(T e);
        protected abstract void UnregisterEvent(T e);
    }
}