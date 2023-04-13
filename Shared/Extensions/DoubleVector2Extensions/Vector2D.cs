using System;
using Godot;

namespace Shared.Extensions.DoubleVector2Extensions
{
    public partial struct Vector2D
    {
        public double X;
        public double Y;
        public Vector2D(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public void Deconstruct(out double x, out double y)
        {
            x = X;
            y = Y;
        }
        public bool IsInRect(double minX, double minY, double maxX, double maxY)
        {
            return X >= minX && Y >= minY && X < maxX && Y < maxY;
        }
        public static Vector2D Lerp(Vector2D from, Vector2D to, double weight)
        {
            return from + (to - from) * weight;
        }
    }
}