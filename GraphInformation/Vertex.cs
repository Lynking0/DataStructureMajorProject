using System;
using Godot;
using GraphInformation.DoubleVector2Extensions;
using System.Collections.Generic;

namespace GraphInformation
{
    public class Vertex
    {
        public Vector2D Position;
        public Vector2D Gradient;
        public List<Edge> Adjacencies;
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