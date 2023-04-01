namespace Shared.QuadTree
{
    public partial class QuadTree<T> where T : class, ILocatable
    {
        public class ItemContainer
        {
            public T Item;
            public Handle Handle;
            public ItemContainer(T item, Handle handle)
            {
                Item = item;
                Handle = handle;
            }
        }
    }
}