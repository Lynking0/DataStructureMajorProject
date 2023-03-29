using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
    public interface ILocatable
    {
        Vector2 Position { get; }
    }
    public enum QuadTreeNodeType
    {
        Internal,
        Leaf,
    }
    public class QuadTree<T> where T : class, ILocatable
    {
        public class Handle
        {
            public QuadTreeNode? Node;
            public Handle(QuadTreeNode node)
            {
                Node = node;
            }
            public IEnumerable<T> Nearby(T obj)
            {
                return Nearby(obj.Position);
            }
            public IEnumerable<T> Nearby(Vector2 position)
            {
                if (Node == null)
                    throw new Exception("Handle is invalid");
                foreach (var nearFactories in Node.Nearby())
                {
                    var temp = nearFactories.ToList();
                    temp.Sort((a, b) => a.Position.DistanceSquaredTo(position).CompareTo(b.Position.DistanceSquaredTo(position)));
                    foreach (var factory in temp)
                        yield return factory;
                }
            }
        }
        public class QuadTreeNode
        {
            private Int32 Level;
            private Rect2 Bounds;
            private QuadTreeNodeType Type;
            private QuadTreeNode?[]? Nodes;
            private T?[]? Items;
            public UInt32 ItemCount { get; private set; }
            private QuadTreeNode? Parent;

            private QuadTreeNode[] InitForInternal()
            {
                return new QuadTreeNode[]{
                        // LU
                        new QuadTreeNode(this,QuadTreeNodeType.Leaf,Level + 1, new Rect2(Bounds.Position, Bounds.Size / 2)),
                        // LD
                        new QuadTreeNode(this,QuadTreeNodeType.Leaf,Level + 1, new Rect2(Bounds.Position + new Vector2(0, Bounds.Size.Y / 2), Bounds.Size / 2)),
                        // RU
                        new QuadTreeNode(this,QuadTreeNodeType.Leaf,Level + 1, new Rect2(Bounds.Position + new Vector2(Bounds.Size.X / 2, 0), Bounds.Size / 2)),
                        // RD
                        new QuadTreeNode(this,QuadTreeNodeType.Leaf,Level + 1, new Rect2(Bounds.Position + Bounds.Size / 2, Bounds.Size / 2))
                    };
            }
            private T[] InitForLeaf()
            {
                return new T[MAX_ITEM];
            }
            private QuadTreeNode(QuadTreeNode parent, QuadTreeNodeType type, int level, Rect2 bounds) : this(type, level, bounds)
            {
                Parent = parent;
            }
            public QuadTreeNode(QuadTreeNodeType type, int level, Rect2 bounds)
            {
                Level = level;
                Bounds = bounds;
                Type = type;
                Parent = null;
                if (type == QuadTreeNodeType.Internal)
                    Nodes = InitForInternal();
                else if (type == QuadTreeNodeType.Leaf)
                    Items = InitForLeaf();
                else
                    throw new Exception("Invalid node type");
            }
            private int GetIndex(Vector2 position)
            {
                if (position.X < Bounds.Position.X + Bounds.Size.X / 2)
                {
                    if (position.Y < Bounds.Position.Y + Bounds.Size.Y / 2)
                        return 0;
                    else
                        return 1;
                }
                else
                {
                    if (position.Y < Bounds.Position.Y + Bounds.Size.Y / 2)
                        return 2;
                    else
                        return 3;
                }
            }
            private int GetIndex(T obj) { return GetIndex(obj.Position); }

            public Handle Insert(T obj)
            {
                if (Type == QuadTreeNodeType.Internal)
                {
                    var handle = Nodes![GetIndex(obj)]!.Insert(obj);
                    ItemCount++;
                    return handle;
                }
                else if (Type == QuadTreeNodeType.Leaf)
                {
                    for (int i = 0; i < Items!.Length; i++)
                    {
                        if (Items[i] is null)
                        {
                            Items[i] = obj;
                            ItemCount++;
                            return new Handle(this);
                        }
                    }
                    Split();
                    return Insert(obj);
                }
                else
                    throw new Exception("Invalid node type");
            }

            public void Split()
            {
                Debug.Assert(Type == QuadTreeNodeType.Leaf, "Node is not leaf, cannot split");
                // var items = new T[4];
                // Items!.CopyTo(items, 0);
                Type = QuadTreeNodeType.Internal;
                ItemCount = 0;
                Nodes = InitForInternal();
                foreach (var item in Items!)
                {
                    if (item is not null)
                        Insert(item);
                }
                Items = null;
            }
            public IEnumerable<T> GetItems()
            {
                if (Type == QuadTreeNodeType.Internal)
                {
                    var items = new List<T>();
                    foreach (var node in Nodes!)
                    {
                        if (node is not null)
                            items.AddRange(node.GetItems());
                    }
                    return items;
                }
                else if (Type == QuadTreeNodeType.Leaf)
                {
                    var items = new List<T>();
                    foreach (var item in Items!)
                    {
                        if (item is not null)
                            items.Add(item);
                    }
                    return items;
                }
                else
                    throw new Exception("Invalid node type");
            }
            public void Merge()
            {
                Debug.Assert(Type == QuadTreeNodeType.Internal, "Node is not internal, cannot merge");
                Debug.Assert(ItemCount <= MAX_ITEM, "Node has too many items, cannot merge");
                Items = InitForLeaf();
                ItemCount = 0;
                foreach (var item in GetItems())
                    Items[ItemCount++] = item;
                Type = QuadTreeNodeType.Leaf;
                Nodes = null;
            }

            public QuadTreeNode Query(Vector2 position)
            {
                if (Type == QuadTreeNodeType.Internal)
                    return Nodes![GetIndex(position)]!.Query(position);
                else if (Type == QuadTreeNodeType.Leaf)
                    return this;
                else
                    throw new Exception("Invalid node type");
            }
            public QuadTreeNode Query(T obj) { return Query(obj.Position); }

            public void Remove(Vector2 position)
            {
                ItemCount--;
                if (Type == QuadTreeNodeType.Leaf)
                {
                    for (int i = 0; i < Items!.Length; i++)
                    {
                        if (Items[i]?.Position == position)
                        {
                            Items[i] = null;
                            return;
                        }
                    }
                }
                else if (Type == QuadTreeNodeType.Internal)
                {
                    Nodes![GetIndex(position)]!.Remove(position);
                    if (ItemCount <= MAX_ITEM)
                        Merge();
                }
                else
                    throw new Exception("Invalid node type");
            }
            public void Remove(T obj) { Remove(obj.Position); }

            public IEnumerable<IEnumerable<T>> Nearby(QuadTreeNode? exclude = null)
            {
                if (Type == QuadTreeNodeType.Leaf)
                    yield return GetItems();
                else if (Type == QuadTreeNodeType.Internal)
                    foreach (var node in Nodes!)
                    {
                        if (node is not null && node != exclude)
                        {
                            var result = node.GetItems();
                            if (result.Count() > 0)
                                yield return result;
                        }
                    }
                //parent
                if (Parent is not null)
                {
                    foreach (var item in Parent.Nearby(this))
                        yield return item;
                }
            }
#if DEBUG
            public void Detail()
            {
                Debug.Assert(Parent == null, "Just use for root node.");
                var itemCountByLevel = new Dictionary<int, int>();
                void Count(QuadTreeNode node)
                {

                    if (node.Type == QuadTreeNodeType.Internal)
                    {
                        foreach (var child in node.Nodes!)
                            if (child is not null)
                                Count(child);
                    }
                    else
                    {
                        if (itemCountByLevel!.ContainsKey(node.Level))
                            itemCountByLevel[node.Level]++;
                        else
                            itemCountByLevel[node.Level] = 1;
                    }
                }
                Count(this);
                GD.Print("-----------------QuadTree-----------------");
                GD.Print("Item Count: ", ItemCount);
                GD.Print("Max Level: ", itemCountByLevel.Keys.Max());
                GD.Print("Item Count By Level");
                foreach (var item in itemCountByLevel)
                    GD.Print("Level ", item.Key, ": ", item.Value);
                GD.Print("------------------------------------------");
            }
#endif
        }

        private const int MAX_ITEM = 4;
        private QuadTreeNode Root;
        public uint Count { get => Root.ItemCount; }
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