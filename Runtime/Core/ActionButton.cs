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
    public class ActionButton : ActionSelectable, IClickable, ICancelable
    {
        [Header("Events")]
        [Tooltip("Function definition for a button click event.")]
        public UnityEvent onClicked; // to maintain compatibility to Unity.Button

        public event Action OnClicked;
        public event Action OnCanceled;

        public bool IsInvalid() => !IsActive() || !IsInteractable();

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
            if (IsInvalid()) return;

            DoStateTransition(SelectionState.Pressed, instant: false);
            StartCoroutine(OnFinishSubmit());
        }

        // Triggered when the cancel button (typically the Escape key or Gamepad East Button) is pressed
        public void OnCancel(BaseEventData evt)
        {
            PropagateUp(evt);
            Cancel();
        }

        public virtual void Press()
        {
            if (IsInvalid()) return;

            onClicked?.Invoke();
            OnClicked?.Invoke();
        }

        public virtual void Cancel() => OnCanceled?.Invoke();

        // Similar to UI Toolkit Bubble-Up event propagation,
        // notify the cancellable parent about the cancellation
        protected virtual void PropagateUp(BaseEventData evt)
        {
            if (transform.parent == null) return;
            var parentCancelable = transform.parent.GetComponentInParent<ICancelable>();
            parentCancelable?.OnCancel(evt);
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