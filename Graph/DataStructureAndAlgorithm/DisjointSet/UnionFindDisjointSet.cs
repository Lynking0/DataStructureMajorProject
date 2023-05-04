using System;
using Godot;
using System.Collections.Generic;

namespace GraphMoudle.DataStructureAndAlgorithm.DisjointSet
{
    public interface IDisjointSetElement<T>
    {
        T? DisjointSetParent { get; set; }
        /// <summary>
        ///   仅当当前元素为根时此属性有效
        /// </summary>
        int DisjointSetSize { get; set; }
    }
    public static class UnionFindDisjointSet<T> where T : class, IDisjointSetElement<T>
    {
        public static void Init(IEnumerable<T> elements)
        {
            foreach (T ele in elements)
            {
                ele.DisjointSetParent = ele;
                ele.DisjointSetSize = 1;
            }
        }
        public static T Find(T x)
        {
            if (x.DisjointSetParent == x)
                return x;
            return x.DisjointSetParent = Find(x.DisjointSetParent!);
        }
        /// <summary>
        ///   若两元素不在一个集合中，则进行合并
        /// </summary>
        /// <returns>是否进行了合并</returns>
        public static bool Union(T x, T y)
        {
            x = Find(x);
            y = Find(y);
            if (x != y)
            {
                if (x.DisjointSetSize > y.DisjointSetSize)
                {
                    x.DisjointSetSize += y.DisjointSetSize;
                    y.DisjointSetParent = x;
                }
                else
                {
                    y.DisjointSetSize += x.DisjointSetSize;
                    x.DisjointSetParent = y;
                }
                return true;
            }
            return false;
        }
    }
}