namespace ActionCode.UISystem
{
    /// <summary>
    /// Enum to control Selectable state.<br/>
    /// Cannot use the SelectionState from Unity UI since it is a protected enum inside <see cref="UnityEngine.UI.Selectable"/>.
    /// </summary>
    public enum SelectionState
    {
        Normal,
        Highlighted,
        Pressed,
        Selected,
        Disabled
    }
}