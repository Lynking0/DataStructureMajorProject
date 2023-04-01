using Godot;
using System;
using System.Collections.Generic;

namespace Shared.QuadTree
{
    /*
    Layout:
     _____      _____ 
    |  _  |    / __  \
    | |/' |    `' / /'
    |  /| |      / /
    \ |_/ /    ./ /___
     \___/     \_____/
      __        _____ 
     /  |      |____ |
     `| |          / /
      | |          \ \
     _| |_     .___/ /
     \___/     \____/                                  
    */
    public partial class QuadTree<T> where T : class, ILocatable
    {
        private QuadTreeNode Root;
        public UInt32 Count { get => Root.ItemCount; }
        public QuadTree(Rect2 bounds)
        {
            Root = new QuadTreeNode(QuadTreeNodeType.Internal, 0, bounds);
        }
        public Handle Insert(T obj)
        {
            return Root.Insert(obj);
        }
        public void Remove(T obj)
        {
            Root.Remove(obj);
        }
        public IEnumerable<T> GetItems()
        {
            return Root.GetItems();
        }
#if DEBUG
        public void Detail()
        {
            Root.Detail();
        }
#endif
    }
}