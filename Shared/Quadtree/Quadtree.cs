using Godot;
using System;
using System.Collections.Generic;

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
        public class QuadTree<T> where T : ILocatable
        {
            class QuadTreeNode
            {
                protected int Level;
                protected Rect2 Bounds;
                protected QuadTreeNode(int level, Rect2 bounds)
                {
                    Level = level;
                    Bounds = bounds;
                }
            }
            class QuadTreeLeaf : QuadTreeNode
            {
                private T?[] Items;
                private int ObjectCount = 0;
                public QuadTreeLeaf(int level, Rect2 bounds) : base(level, bounds)
                {
                    Items = new T[MAX_OBJECT];
                }
                public bool isFull()
                {
                    return ObjectCount == MAX_OBJECT;
                }
                public bool isNull()
                {
                    return ObjectCount == 0;
                }
                public bool Insert(T obj)
                {
                    for (int i = 0; i < Items.Length; i++)
                    {
                        if (Items[i] is null)
                        {
                            Items[i] = obj;
                            ObjectCount++;
                            return true;
                        }
                    }
                    return false;
                }
                public void Remove(T obj)
                {
                    for (int i = 0; i < Items.Length; i++)
                    {
                        if (Items[i] is not null && Items[i]!.Position == obj.Position)
                        {
                            Items[i] = default;
                            ObjectCount--;
                            return;
                        }
                    }
                    throw new Exception("Object not found");
                }
                public QuadTreeInternal Split()
                {
                    QuadTreeInternal node = new QuadTreeInternal(Level, Bounds);
                    for (int i = 0; i < Items.Length; i++)
                    {
                        if (Items[i] is not null)
                            node.Insert(Items[i]!);
                    }
                    return node;
                }
            }
            class QuadTreeInternal : QuadTreeNode
            {
                private QuadTreeNode?[] Nodes;
                private int ObjectCount = 0;
                public QuadTreeInternal(int level, Rect2 bounds) : base(level, bounds)
                {
                    Nodes = new QuadTreeNode[]{
                        new QuadTreeLeaf(level + 1, new Rect2(bounds.Position + new Vector2(bounds.Size.X / 2, 0), bounds.Size / 2)),
                        new QuadTreeLeaf(level + 1, new Rect2(bounds.Position, bounds.Size / 2)),
                        new QuadTreeLeaf(level + 1, new Rect2(bounds.Position + new Vector2(0, bounds.Size.Y / 2), bounds.Size / 2)),
                        new QuadTreeLeaf(level + 1, new Rect2(bounds.Position + bounds.Size / 2, bounds.Size / 2))
                    };
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

                private void Insert(T obj, int nodeIndex)
                {
                    if (Nodes[nodeIndex] is QuadTreeInternal n)
                        n.Insert(obj);
                    else if (Nodes[nodeIndex] is QuadTreeLeaf l)
                    {
                        if (!l.isFull())
                            l.Insert(obj);
                        else
                        {
                            GD.Print("Full");
                            Nodes[nodeIndex] = l.Split();
                            (Nodes[nodeIndex] as QuadTreeInternal)!.Insert(obj);
                        }
                    }
                }
                public void Insert(T obj)
                {
                    Insert(obj, GetIndex(obj));
                    ObjectCount++;
                }

                private QuadTreeInternal Query(T obj, int nodeIndex)
                {
                    if (Nodes[nodeIndex] is QuadTreeInternal n)
                        return n.Query(obj);
                    else if (Nodes[nodeIndex] is QuadTreeLeaf l)
                        return this;
                    else
                        throw new Exception("Node is null");
                }
                public QuadTreeInternal Query(T obj)
                {
                    if (!Bounds.HasPoint(obj.Position))
                        throw new Exception("Object is not in the bounds of the QuadTree");
                    return Query(obj, GetIndex(obj));
                }

                public void Remove(T obj)
                {
                    if (!Bounds.HasPoint(obj.Position))
                        throw new Exception("Object is not in the bounds of the QuadTree");
                    var leaf = (Nodes[GetIndex(obj)] as QuadTreeLeaf);
                    leaf?.Remove(obj);
                    ObjectCount--;
                    if (ObjectCount <= MAX_OBJECT)
                    {
                        Merge();
                    }
                }

                private void Merge()
                {
                    // TODO: Merge
                }
            }

            private const int MAX_OBJECT = 4;

            private QuadTreeInternal Root;

            public QuadTree(Rect2 bounds)
            {
                Root = new QuadTreeInternal(0, bounds);
            }
            public void Insert(T obj)
            {
                Root.Insert(obj);
            }
            public void Remove(T obj)
            {
                Root.Query(obj).Remove(obj);
            }
            //TODO: Test it
        }
    }
}