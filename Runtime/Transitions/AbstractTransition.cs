using UnityEngine;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract component to transit UI components based on <see cref="SelectionState"/>.
    /// </summary>
    public abstract class AbstractTransition : MonoBehaviour
    {
        public abstract void Transit(SelectionState state, bool instant);
    }
}