using System;
using Shared.Extensions.DoubleVector2Extensions;
using System.Collections.Generic;
using GraphMoudle.DataStructureAndAlgorithm.DisjointSet;
using GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTreeStructure;
using Godot;

namespace GraphMoudle
{
    public class Vertex : IDisjointSetElement<Vertex>, IRTreeData
    {
        public Vertex? DisjointSetParent { get; set; }
        public int DisjointSetSize { get; set; }

        public Vector2D Position;
        public Vector2D Gradient;
        public List<Edge> Adjacencies;
        public enum VertexType
        {
            Isolated = 0, // 孤立点，没有任何的边，在建边后将被删除
            Terminal, // 控制点仅在一个方向，即梯度的反方向
            Intermediate // 控制点可在两个方向，分别为梯度顺/逆时针转90度方向
        }
        /// <summary>
        ///   控制生成Edge时控制点方向的选择，仅在生成Edge时使用。
        /// </summary>
        public VertexType Type;
        public Vertex(double x, double y)
        {
            Position.X = (float)x;
            Position.Y = (float)y;
            Adjacencies = new List<Edge>();
        }
        public Vertex(Vector2D pos)
        {
            Position = pos;
            Adjacencies = new List<Edge>();
        }

        #region IRTreeDataImplementation

        public bool IsOverlap(IRTreeData other)
        {
            if (other is Vertex vertex)
                return false; // EdgesDistance < VerticesDistance
            if (other is Edge edge)
                return edge.IsOverlap(this);
            throw new Exception($"{GetType()}.IsOverlap(IRTreeData): Unexpected type.");
        }
        private RTRect2? _rectangle = null;
        public RTRect2 Rectangle { get => _rectangle ??= _getRectangle(); }
        /// <summary>
        ///   点与边之间最小距离为Graph.EdgesDistance，在MBR的基础上需向四边扩张Graph.EdgesDistance / 2距离
        /// </summary>
        private RTRect2 _getRectangle()
        {
            double minX = Position.X - Graph.EdgesDistance / 2;
            double maxX = Position.X + Graph.EdgesDistance / 2;
            double minY = Position.Y - Graph.EdgesDistance / 2;
            double maxY = Position.Y + Graph.EdgesDistance / 2;
            return new RTRect2(new Vector2D(minX, minY), new Vector2D(maxX, maxY));
        }

        #endregion
    }
}