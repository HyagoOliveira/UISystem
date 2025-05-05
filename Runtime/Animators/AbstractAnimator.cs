using UnityEngine;
using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class AbstractAnimator : MonoBehaviour
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
                document = value;
                Reload();
            }
        }

        protected virtual void Reset() => document = GetComponent<UIDocument>();
        protected virtual void OnEnable() => Reload();
        private void Update() => UpdateAnimation();

        public T Find<T>(string name) where T : VisualElement => Root.Find<T>(name);

        protected virtual void Reload() => FindReferences();

        protected abstract void FindReferences();
        protected abstract void UpdateAnimation();
    }
}