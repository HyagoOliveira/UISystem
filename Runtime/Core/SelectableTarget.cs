using UnityEngine;
using UnityEngine.UI;

namespace ActionCode.UISystem
{
    [System.Serializable]
    public sealed class SelectableTarget
    {
        [Tooltip("The target to change color.")]
        public Graphic target;

        [Space]
        public Color normalColor;
        public Color selectedColor;
        public Color pressedColor;
        public Color disabledColor;

        public Color Color
        {
            get => HasTarget() ? target.color : default;
            set
            {
                if (HasTarget()) target.color = value;
            }
        }

        public SelectableTarget() => SetColors(Color.white);

        public bool HasTarget() => target != null;

        public void SetColors(Color baseColor)
        {
            normalColor = baseColor;
            pressedColor = baseColor * 0.8f;
            selectedColor = baseColor * 1.2f;
            disabledColor = baseColor * 0.2F;
        }

        public void FadeTarget(Color color, float duration)
        {
            if (!HasTarget()) return;
            target.CrossFadeColor(
                color,
                duration,
                ignoreTimeScale: true,
                useAlpha: true
            );
        }
    }
}