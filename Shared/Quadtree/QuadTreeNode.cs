using Godot;
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace Shared.QuadTree
{
    public partial class QuadTree<T> where T : class, ILocatable
    {
        public partial class QuadTreeNode
        {
            private Int32 Level;
            public Rect2 Bounds { get; private set; }
            public QuadTreeNodeType Type { get; private set; }
            private QuadTreeNode[]? Nodes;
            private ItemContainer?[]? ItemContainers;
            public UInt32 ItemCount { get; private set; }
            private QuadTreeNode? Parent;
            public readonly GeoHash GeoHash;

            private QuadTreeNode[] InitForInternal()
            {
                return new QuadTreeNode[]{
                        new QuadTreeNode(this,GeoCode.LU, QuadTreeNodeType.Leaf,Level + 1),
                        new QuadTreeNode(this,GeoCode.LD,QuadTreeNodeType.Leaf,Level + 1),
                        new QuadTreeNode(this,GeoCode.RU,QuadTreeNodeType.Leaf,Level + 1),
                        new QuadTreeNode(this,GeoCode.RD,QuadTreeNodeType.Leaf,Level + 1)
                    };
            }
            private QuadTreeNode(QuadTreeNode parent, GeoCode code, QuadTreeNodeType type, int level) : this(type, level, parent.GetSubBounds(code))
            {
                Parent = parent;
                GeoHash = new GeoHash(parent.GeoHash, code);
            }
            public QuadTreeNode(QuadTreeNodeType type, int level, Rect2 bounds)
            {
                Level = level;
                Bounds = bounds;
                Type = type;
                Parent = null;
                GeoHash = new GeoHash();
                if (type == QuadTreeNodeType.Internal)
                    Nodes = InitForInternal();
                else if (type == QuadTreeNodeType.Leaf)
                {
                    ItemContainers = new ItemContainer[Constants.MAX_ITEM];
                }
                else
                    throw new Exception("Invalid node type");
            }
            private GeoCode GetIndex(Vector2 position)
            {
                if (position.X < Bounds.Position.X + Bounds.Size.X / 2)
                {
                    if (position.Y < Bounds.Position.Y + Bounds.Size.Y / 2)
                        return GeoCode.LU;
                    else
                        return GeoCode.LD;
                }
                else
                {
                    if (position.Y < Bounds.Position.Y + Bounds.Size.Y / 2)
                        return GeoCode.RU;
                    else
                        return GeoCode.RD;
                }
            }
            private GeoCode GetIndex(T obj) { return GetIndex(obj.Position); }

            private Rect2 GetSubBounds(GeoCode code)
            {
                switch (code)
                {
                    case GeoCode.LU:
                        return new Rect2(Bounds.Position, Bounds.Size / 2);
                    case GeoCode.LD:
                        return new Rect2(Bounds.Position + new Vector2(0, Bounds.Size.Y / 2), Bounds.Size / 2);
                    case GeoCode.RU:
                        return new Rect2(Bounds.Position + new Vector2(Bounds.Size.X / 2, 0), Bounds.Size / 2);
                    case GeoCode.RD:
                        return new Rect2(Bounds.Position + Bounds.Size / 2, Bounds.Size / 2);
                    default:
                        throw new Exception("Invalid code");
                }
            }

            public Handle Insert(T obj)
            {
                if (Type == QuadTreeNodeType.Internal)
                {
                    var handle = Nodes![GetIndex(obj).GetHashCode()].Insert(obj);
                    ItemCount++;
                    return handle;
                }
                else if (Type == QuadTreeNodeType.Leaf)
                {
                    for (int i = 0; i < ItemContainers!.Length; i++)
                    {
                        if (ItemContainers[i] is null)
                        {
                            ItemContainers[i] = new ItemContainer(obj, new Handle(this));
                            ItemCount++;
                            return ItemContainers[i]!.Handle;
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
                if (Level == Constants.MAX_LEVEL - 1)
                    throw new Exception("Cannot split, max level reached.Shared. QuadTree.Constants.MAX_LEVEL = " + Constants.MAX_LEVEL);
                Debug.Assert(Type == QuadTreeNodeType.Leaf, "Node is not leaf, cannot split");
                Type = QuadTreeNodeType.Internal;
                Nodes = InitForInternal();
                for (int i = 0; i < Constants.MAX_ITEM; i++)
                {
                    if (ItemContainers![i] is not null)
                    {
                        Nodes[GetIndex(ItemContainers[i]!.Item).GetHashCode()].Adopt(ItemContainers[i]!);
                    }
                }
                ItemContainers = null;
            }

            public void Adopt(ItemContainer container)
            {
                Debug.Assert(Type == QuadTreeNodeType.Leaf, "Node is not leaf, cannot adopt");
                for (int i = 0; i < Constants.MAX_ITEM; i++)
                {
                    if (ItemContainers![i] is null)
                    {
                        ItemContainers[i] = container;
                        ItemContainers[i]!.Handle.Node = this;
                        ItemCount++;
                        return;
                    }
                }
            }

            public IEnumerable<ItemContainer> GetItemWithHandles()
            {
                if (Type == QuadTreeNodeType.Internal)
                {
                    foreach (var node in Nodes!)
                        if (node is not null)
                            foreach (var data in node.GetItemWithHandles())
                                yield return data;
                }
                else if (Type == QuadTreeNodeType.Leaf)
                {
                    for (int i = 0; i < Constants.MAX_ITEM; i++)
                        if (ItemContainers![i] is not null)
                            yield return ItemContainers[i]!;
                }
                else
                    throw new Exception("Invalid node type");
            }
            public IEnumerable<T> GetItems()
            {
                if (Type == QuadTreeNodeType.Internal)
                {
                    foreach (var node in Nodes!)
                        if (node is not null)
                            foreach (var item in node.GetItems())
                                yield return item;
                }
                else if (Type == QuadTreeNodeType.Leaf)
                {
                    foreach (var item in ItemContainers!)
                        if (item is not null)
                            yield return item.Item;
                }
                else
                    throw new Exception("Invalid node type");
            }
            public void Merge()
            {
                Debug.Assert(true, "Unverified function.");
                Debug.Assert(Type == QuadTreeNodeType.Internal, "Node is not internal, cannot merge");
                Debug.Assert(ItemCount <= Constants.MAX_ITEM, "Node has too many items, cannot merge");
                ItemContainers = new ItemContainer[Constants.MAX_ITEM];
                ItemCount = 0;
                foreach (var contaniner in GetItemWithHandles())
                {
                    ItemContainers[ItemCount++] = contaniner;
                }
                Type = QuadTreeNodeType.Leaf;
                Nodes = null;
            }

            public QuadTreeNode Query(GeoHash hash)
            {
                if (GeoHash.Include(hash))
                {
                    if (GeoHash == hash || Type == QuadTreeNodeType.Leaf)
                    {
                        return this;
                    }
                    if (Type == QuadTreeNodeType.Internal)
                    {
                        return Nodes![(GeoHash - hash).GetHashCode()].Query(hash);
                    }
                }
                else
                {
                    if (Parent is not null)
                        return Parent.Query(hash);
                }
                GD.Print(hash);
                throw new Exception("Invalid GeoHash.");
            }

            private QuadTreeNode Query(Vector2 position)
            {
                if (Type == QuadTreeNodeType.Internal)
                {
                    return Nodes![GetIndex(position).GetHashCode()].Query(position);
                }
                else if (Type == QuadTreeNodeType.Leaf)
                {
                    if (Bounds.HasPoint(position))
                    {
                        return this;
                    }
                    else
                    {
                        throw new Exception("Position not found");
                    }
                }
                else
                {
                    throw new Exception("Invalid node type");
                }
            }
            private QuadTreeNode Query(T obj) { return Query(obj.Position); }

            public void Remove(Vector2 position)
            {
                ItemCount--;
                if (Type == QuadTreeNodeType.Leaf)
                {
                    for (int i = 0; i < ItemContainers!.Length; i++)
                    {
                        if (ItemContainers[i]?.Item.Position == position)
                        {
                            ItemContainers[i] = null;
                            return;
                        }
                    }
                }
                else if (Type == QuadTreeNodeType.Internal)
                {
                    Nodes![GetIndex(position).GetHashCode()].Remove(position);
                    if (ItemCount <= Constants.MAX_ITEM)
                        Merge();
                }
                else
                    throw new Exception("Invalid node type");
            }
            public void Remove(T obj) { Remove(obj.Position); }

            public IEnumerable<IEnumerable<T>> Nearby(QuadTreeNode? exclude = null)
            {
                // TODO: NearBy
                if (Type == QuadTreeNodeType.Leaf)
                    yield return GetItems();
                uint distance = 1;
                while (true)
                {
                    IEnumerable<T> totalResult = Enumerable.Empty<T>();
                    var near = GeoHash.Around(distance++).ToList();
                    foreach (var item in near)
                    {
                        var node = Query(item);
                        if (node != exclude)
                        {
                            var result = node.GetItems();
                            if (result.Count() > 0)
                                totalResult = totalResult.Concat(result);
                        }
                    }
                    yield return totalResult;
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
                        if (itemCountByLevel.ContainsKey(node.Level))
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

    }
}