using System;
using Godot;

namespace GraphInformation.DoubleVector2Extensions
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
        public bool IsInRect(double minX, double minY, double maxX, double maxY)
        {
            return X >= minX && Y >= minY && X < maxX && Y < maxY;
        }
    }
}