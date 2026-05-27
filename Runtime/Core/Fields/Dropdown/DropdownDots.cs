using UnityEngine;
using UnityEngine.UI;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Controller for a Horizontal Carousel Dropdown Dots.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ContentSizeFitter))]
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public sealed class DropdownDots : MonoBehaviour
    {
        [SerializeField] private GameObject dotsPrefab;

        private Image[] dots;

        private static readonly Color selectedColor = Color.white;
        private static readonly Color unselectedColor = Color.gray;

        public void Destroy()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        public void Spawn(int count)
        {
            dots = new Image[count];
            for (int i = 0; i < count; i++)
            {
                var instance = Instantiate(dotsPrefab, transform);
                instance.name = $"{dotsPrefab.name}_{i:D2}";

                dots[i] = instance.GetComponent<Image>();
                dots[i].color = unselectedColor;
            }
        }

        public void Select(int index)
        {
            UnselectDots();
            dots[index].color = selectedColor;
        }

        private void UnselectDots() => UpdateDotsColor(unselectedColor);

        private void UpdateDotsColor(Color color)
        {
            foreach (var dot in dots)
            {
                dot.color = color;
            }
        }
    }
}
