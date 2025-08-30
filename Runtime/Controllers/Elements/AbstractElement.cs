using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    public abstract class AbstractElement<T> : MonoBehaviour, IDisposable where T : VisualElement
    {
        private UQueryBuilder<T> elements; // its a struct, so its never null

        public bool IsInitialized { get; private set; }

        public void Initialize(VisualElement root)
        {
            elements = root.Query<T>(className: GetClassName());
            elements.ForEach(RegisterEvent);
            IsInitialized = true;
        }

        public void Dispose()
        {
            if (!IsInitialized) return;

            elements.ForEach(UnregisterEvent);
            IsInitialized = false;
        }

        protected abstract string GetClassName();
        protected abstract void RegisterEvent(T e);
        protected abstract void UnregisterEvent(T e);
    }
}