using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Button component with clickable and cancelable callbacks.
    /// <br/><br/>
    /// Similar to UI Toolkit System, parent instances implementing <see cref="ICancelable"/> 
    /// interface are notified when this button is cancellable. 
    /// Use it to cancel any component implementing the <see cref="ICancelable"/> interface.
    /// <br/><br/>
    /// <inheritdoc cref="ActionSelectable"/>
    /// </summary>
    /// <remarks>
    /// Use <see cref="OnClicked"/> to get notified when this object is clicked.<br/>
    /// Use <see cref="OnCanceled"/> to get notified when this object is cancelled.<br/>
    /// <inheritdoc cref="ActionSelectable"/>
    /// </remarks>
    [DisallowMultipleComponent]
    public class ActionButton : ActionSelectable, IClickable
    {
        [Header("Events")]
        [Tooltip("Function definition for a button click event.")]
        public UnityEvent onClicked; // to maintain compatibility to Unity.Button

        public event Action OnClicked;

        // Triggered when Mouse clicks on it
        public void OnPointerClick(PointerEventData evt)
        {
            var isLeftClick = evt.button == PointerEventData.InputButton.Left;
            if (isLeftClick) Press();
        }

        // Triggered when Gamepad/Keyboard submits or Mouse clicks on it
        public void OnSubmit(BaseEventData _)
        {
            Press();

            // if we get set disabled during the press, 
            // don't run the coroutine.
            if (!IsAvailable()) return;

            DoStateTransition(SelectionState.Pressed, instant: false);
            StartCoroutine(OnFinishSubmit());
        }

        public virtual void Press()
        {
            if (!IsAvailable()) return;

            onClicked?.Invoke();
            OnClicked?.Invoke();
        }

        private System.Collections.IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, instant: false);
        }
    }
}