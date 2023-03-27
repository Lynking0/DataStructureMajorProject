using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Shared
{
    namespace QuadTree
    {
        /*
        Layout:
         __       _____ 
        /  |     |  _  |
        `| |     | |/' |
         | |     |  /| |
        _| |_    \ |_/ /
        \___/     \___/ 

         _____      _____ 
        / __  \    |____ |
        `' / /'        / /
        / /          \ \
        ./ /___    .___/ /
        \_____/    \____/         
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

                public IEnumerable<List<T>> Nearby()
                {
                    if (Node == null)
                        throw new Exception("Handle is invalid");
                    return Node.Nearby();
                }
            }
            public class QuadTreeNode
            {
                private Int32 Level;
                private Rect2 Bounds;
                private QuadTreeNodeType Type;
                private QuadTreeNode?[]? Nodes;
                private T?[]? Items;
                private UInt32 ObjectCount = 0;
                private QuadTreeNode? Parent;

                private QuadTreeNode[] InitForInternal()
                {
                    return new QuadTreeNode[]{
                        new QuadTreeNode(this,QuadTreeNodeType.Leaf,Level + 1, new Rect2(Bounds.Position + new Vector2(Bounds.Size.X / 2, 0), Bounds.Size / 2)),
                        new QuadTreeNode(this,QuadTreeNodeType.Leaf,Level + 1, new Rect2(Bounds.Position, Bounds.Size / 2)),
                        new QuadTreeNode(this,QuadTreeNodeType.Leaf,Level + 1, new Rect2(Bounds.Position + new Vector2(0, Bounds.Size.Y / 2), Bounds.Size / 2)),
                        new QuadTreeNode(this,QuadTreeNodeType.Leaf,Level + 1, new Rect2(Bounds.Position + Bounds.Size / 2, Bounds.Size / 2))
                    };
                }
                private T[] InitForLeaf()
                {
                    return new T[MAX_OBJECT];
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
                            return 1;
                        else
                            return 2;
                    }
                    else
                    {
                        if (position.Y < Bounds.Position.Y + Bounds.Size.Y / 2)
                            return 0;
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
                        ObjectCount++;
                        return handle;
                    }
                    else if (Type == QuadTreeNodeType.Leaf)
                    {
                        for (int i = 0; i < Items!.Length; i++)
                        {
                            if (Items[i] is null)
                            {
                                Items[i] = obj;
                                ObjectCount++;
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
                    ObjectCount = 0;
                    Nodes = InitForInternal();
                    foreach (var item in Items!)
                    {
                        if (item is not null)
                            Insert(item);
                    }
                    Items = null;
                }
                public List<T> GetItems()
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
                    Debug.Assert(ObjectCount <= MAX_OBJECT, "Node has too many objects, cannot merge");
                    Items = InitForLeaf();
                    ObjectCount = 0;
                    foreach (var item in GetItems())
                        Items[ObjectCount++] = item;
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
                    ObjectCount--;
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
                        if (ObjectCount <= MAX_OBJECT)
                            Merge();
                    }
                    else
                        throw new Exception("Invalid node type");
                }
                public void Remove(T obj) { Remove(obj.Position); }

                public IEnumerable<List<T>> Nearby(QuadTreeNode? exclude = null)
                {
                    if (Type == QuadTreeNodeType.Leaf)
                        yield return GetItems();
                    else if (Type == QuadTreeNodeType.Internal)
                        foreach (var node in Nodes!)
                        {
                            if (node is not null && node != exclude)
                            {
                                var result = node.GetItems();
                                if (result.Count > 0)
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
            }
            private const int MAX_OBJECT = 4;

            public QuadTreeNode Root;

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
        }
    }
}