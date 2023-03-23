using System;
using Godot;
using System.Collections;
using System.Collections.Generic;
using GraphInformation.DoubleVector2Extensions;


namespace GraphInformation.SpatialIndexer
{
    /// <summary>
    ///   使用网格对节点进行空间索引
    /// </summary>
    public class VertexSpatialIndexer : ICollection<Vertex>, IReadOnlyCollection<Vertex>
    {
        private Vertex?[,] Grid;

        public int Count { get; private set; }
        public bool IsReadOnly { get => false; }

        public VertexSpatialIndexer()
        {
            Grid = new Vertex?[
                Mathf.CeilToInt((Graph.MaxX - Graph.MinX) * Mathf.Sqrt(2.0) / Graph.VerticesDistance),
                Mathf.CeilToInt((Graph.MaxY - Graph.MinY) * Mathf.Sqrt(2.0) / Graph.VerticesDistance)
            ];
            Count = 0;
        }

        /// <summary>
        ///   此函数不会检查pos是否在地图内。
        /// </summary>
        private static Vector2I GetGridCoord(Vector2D pos)
        {
            pos.X -= Graph.MinX;
            pos.Y -= Graph.MinY;
            return (Vector2I)(pos * Mathf.Sqrt(2.0) / Graph.VerticesDistance);
        }

        /// <summary>
        ///   若重复在同一网格添加节点会覆盖。
        /// </summary>
        public void Add(Vertex vertex)
        {
            Vector2I vi = GetGridCoord(vertex.Position);
            if (Grid[vi.X, vi.Y] is null)
                ++Count;
            Grid[vi.X, vi.Y] = vertex;
        }
        public void Clear()
        {
            if (Count > 0)
            {
                for (int i = 0; i < Grid.GetLength(0); ++i)
                    for (int j = 0; j < Grid.GetLength(1); ++j)
                        Grid[i, j] = null;
                Count = 0;
            }
        }
        public bool Contains(Vertex vertex)
        {
            Vector2I vi = GetGridCoord(vertex.Position);
            return Grid[vi.X, vi.Y] == vertex;
        }
        public void CopyTo(Vertex[] array, int arrayIndex)
        {
            if (array is null)
                throw new ArgumentNullException("The array cannot be null.");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
            if (Count > array.Length - arrayIndex)
                throw new ArgumentException("The destination array has fewer elements than the collection.");

            foreach (Vertex? vertex in Grid)
                if (vertex is not null)
                    array[arrayIndex++] = vertex;
        }
        public bool Remove(Vertex vertex)
        {
            Vector2I vi = GetGridCoord(vertex.Position);
            if (Grid[vi.X, vi.Y] != vertex)
                return false;
            Grid[vi.X, vi.Y] = null;
            return true;
        }
        public IEnumerator<Vertex> GetEnumerator()
        {
            foreach (Vertex? vertex in Grid)
                if (vertex is not null)
                    yield return vertex;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        ///   查询某位置附近是否有其他点，此函数不会检查pos是否在地图内。
        /// </summary>
        public bool HasAdjacency(Vector2D pos)
        {
            Vector2I vi = GetGridCoord(pos);
            for (int i = vi.X - 2; i <= vi.X + 2; ++i)
            {
                if (i < 0 || i >= Grid.GetLength(0))
                    continue;
                for (int j = vi.Y - 2; j <= vi.Y + 2; ++j)
                {
                    if (j < 0 || j >= Grid.GetLength(1))
                        continue;
                    if (Grid[i, j] is Vertex otherVertex)
                        if (pos.DistanceToD(otherVertex.Position) <= Graph.VerticesDistance)
                            return true;
                }
            }
            return false;
        }
    }
}