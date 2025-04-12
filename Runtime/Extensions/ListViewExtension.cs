using UnityEngine.UIElements;

namespace ActionCode.UISystem
{
    /// <summary>
    /// Extension class for <see cref="ListView"/>.
    /// </summary>
    public static class ListViewExtension
    {
        /// <summary>
        /// Selects the list first element.
        /// <para>Focus the list if not.</para>
        /// </summary>
        /// <param name="list"></param>
        public static void SelectFirst(this ListView list) => Select(list, 0);

        /// <summary>
        /// Selects the list element pointed by the given index.
        /// <para>Focus the list if not.</para>
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index">The list element index.</param>
        public static void Select(this ListView list, int index)
        {
            list.Focus();
            list.SetSelection(index);
        }
    }
}