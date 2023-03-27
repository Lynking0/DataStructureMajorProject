using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
// using System.Collections.Generic;

namespace Shared
{
    namespace QuadTree
    {
        public interface ILocatable
        {
            Vector2 Position { get; }
        }
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
        public class QuadTree<T> where T : class, ILocatable
        {
            enum QuadTreeNodeType
            {
                Internal,
                Leaf,
            }
            class QuadTreeNode
            {
                private Int32 Level;
                private Rect2 Bounds;
                private QuadTreeNodeType Type;
                private QuadTreeNode?[]? Nodes;
                private T?[]? Items;

                private UInt32 ObjectCount = 0;
                // private Object[] Objects;
                // private QuadTreeNode[] Nodes
                // {
                //     get
                //     {
                //         if (Type != QuadTreeNodeType.Internal)
                //             // throw new Exception("Node is not internal, cannot get nodes");
                //             return new QuadTreeNode[MAX_OBJECT];
                //         return (QuadTreeNode[])Nodes;
                //     }
                // }
                // private T[] Items
                // {
                //     get
                //     {
                //         if (Type != QuadTreeNodeType.Leaf)
                //             // throw new Exception("Node is not leaf, cannot get items");
                //             return new T[MAX_OBJECT];
                //         return (T[])Objects;
                //     }
                // }

                private QuadTreeNode[] InitForInternal()
                {
                    return new QuadTreeNode[]{
                        new QuadTreeNode(QuadTreeNodeType.Leaf,Level + 1, new Rect2(Bounds.Position + new Vector2(Bounds.Size.X / 2, 0), Bounds.Size / 2)),
                        new QuadTreeNode(QuadTreeNodeType.Leaf,Level + 1, new Rect2(Bounds.Position, Bounds.Size / 2)),
                        new QuadTreeNode(QuadTreeNodeType.Leaf,Level + 1, new Rect2(Bounds.Position + new Vector2(0, Bounds.Size.Y / 2), Bounds.Size / 2)),
                        new QuadTreeNode(QuadTreeNodeType.Leaf,Level + 1, new Rect2(Bounds.Position + Bounds.Size / 2, Bounds.Size / 2))
                    };
                }
                private T[] InitForLeaf()
                {
                    return new T[MAX_OBJECT];
                }
                public QuadTreeNode(QuadTreeNodeType type, int level, Rect2 bounds)
                {
                    Level = level;
                    Bounds = bounds;
                    Type = type;
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

                public void Insert(T obj)
                {
                    if (Type == QuadTreeNodeType.Internal)
                    {
                        Nodes![GetIndex(obj)]!.Insert(obj);
                        ObjectCount++;
                    }
                    else if (Type == QuadTreeNodeType.Leaf)
                    {
                        for (int i = 0; i < Items!.Length; i++)
                        {
                            if (Items[i] is null)
                            {
                                Items[i] = obj;
                                ObjectCount++;
                                return;
                            }
                        }
                        Split();
                        Insert(obj);
                    }
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
                        var items = new T[ObjectCount];
                        for (int i = 0; i < Items!.Length; i++)
                        {
                            items[i] = Items[i]!;
                        }
                        return new List<T>(items);
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
            }
            private const int MAX_OBJECT = 4;

            private QuadTreeNode Root;

            public QuadTree(Rect2 bounds)
            {
                Root = new QuadTreeNode(QuadTreeNodeType.Internal, 0, bounds);
            }
            public void Insert(T obj)
            {
                Root.Insert(obj);
            }
            public void Remove(T obj)
            {
                Root.Remove(obj);
            }
            //TODO: Test it
        }
    }
}