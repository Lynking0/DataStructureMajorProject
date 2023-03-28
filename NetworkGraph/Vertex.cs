using System;
using Godot;
using Shared.Extensions.DoubleVector2Extensions;
using System.Collections.Generic;
using NetworkGraph.DataStructureAndAlgorithm.DisjointSet;

namespace NetworkGraph
{
    public class Vertex : IDisjointSetElement<Vertex>
    {
        public Vertex? DisjointSetParent { get; set; }
        public int DisjointSetSize { get; set; }

        public Vector2D Position;
        public Vector2D Gradient;
        public List<Edge> Adjacencies;
        public enum VertexType
        {
            Terminal = 0, // 控制点仅在一个方向，即梯度的反方向
            Intermediate // 控制点可在两个方向，分别为梯度顺/逆时针转90度方向
        }
        /// <summary>
        ///   控制生成Edge时控制点方向的选择，在生成Edge时使用。
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
    }
}