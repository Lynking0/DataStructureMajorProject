using System;
using Godot;
using GraphInformation.DoubleVector2Extensions;

namespace GraphInformation
{
    public class Edge
    {
        public Vertex A;
        public Vertex B;
        /// <summary>
        ///   点A的控制点（相对坐标）
        /// </summary>
        public Vector2D AC;
        /// <summary>
        ///   点B的控制点（相对坐标）
        /// </summary>
        public Vector2D BC;

        public Edge(Vertex a, Vertex b, Vector2D aC, Vector2D bC)
        {
            A = a;
            B = b;
            AC = aC;
            BC = bC;
        }
    }
}