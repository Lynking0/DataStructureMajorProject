using System;
using Godot;
using System.Collections.Generic;

namespace GraphInformation.DataStructureAndAlgorithm.DisjointSet
{
    public interface IDisjointSetElement<T>
    {
        T? DisjointSetParent { get; set; }
        /// <summary>
        ///   仅当当前元素为根时此属性有效
        /// </summary>
        int DisjointSetSize { get; set; }
    }
    public class UnionFindDisjointSet<T> where T : class, IDisjointSetElement<T>
    {
        public UnionFindDisjointSet(IEnumerable<T> elements)
        {
            foreach (T ele in elements)
            {
                ele.DisjointSetParent = ele;
                ele.DisjointSetSize = 1;
            }
        }
        public T Find(T x)
        {
            if (x.DisjointSetParent == x)
                return x;
            return x.DisjointSetParent = Find(x.DisjointSetParent!);
        }
        public void Union(T x, T y)
        {
            T parentX = Find(x);
            T parentY = Find(y);
            if (x != y)
            {
                if (x.DisjointSetSize > y.DisjointSetSize) {
                    x.DisjointSetSize += y.DisjointSetSize;
                    y.DisjointSetParent = x;
                }
                else {
                    y.DisjointSetSize += x.DisjointSetSize;
                    x.DisjointSetParent = y;
                }
            }
        }
    }
}