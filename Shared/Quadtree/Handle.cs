using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.QuadTree
{
    public partial class QuadTree<T> where T : class, ILocatable
    {
        public class Handle
        {
            public QuadTreeNode Node;
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
    }
}