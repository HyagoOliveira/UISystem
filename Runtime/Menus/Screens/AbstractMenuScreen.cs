namespace ActionCode.UISystem
{
    /// <summary>
    /// Abstract controller for a generic Menu Screen.
    /// <para>
    /// Menu Screens are used to display different parts of a Menu, as a sub-menu or a specific section.
    /// </para>
    /// </summary>
    public abstract class AbstractMenuScreen : AbstractController
    {
        public AbstractMenu Menu { get; private set; }

        internal virtual void Initialize(AbstractMenu menu) => Menu = menu;
    }
}