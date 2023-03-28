using System;
using Godot;
using Shared.Extensions.DoubleVector2Extensions;

namespace NetworkGraph
{
    public class Edge
    {
        public Vertex A;
        public Vertex B;
        public Curve2D Curve;
        public Edge(Vertex a, Vertex b, Curve2D curve)
        {
            A = a;
            B = b;
            Curve = curve;
        }
        /// <summary>
        ///   获取另一头的节点，若v不是该边的端点，则返回null。
        /// </summary>
        public Vertex? GetOtherEnd(Vertex v)
        {
            if (v == A)
                return B;
            if (v == B)
                return A;
            return null;
        }
    }
}