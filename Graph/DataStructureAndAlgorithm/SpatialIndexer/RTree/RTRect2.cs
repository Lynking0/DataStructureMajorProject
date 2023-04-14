using System;
using Shared.Extensions.DoubleVector2Extensions;
using Godot;

namespace GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTreeStructure
{
    /// <summary>
    ///   仅供RTree及其相关函数使用的Rect2
    /// </summary>
    public struct RTRect2
    {
        /// <summary>
        ///   Top Left
        /// </summary>
        public readonly Vector2D TL;
        /// <summary>
        ///   Bottom Right
        /// </summary>
        public readonly Vector2D BR;
        public readonly double Area;
        public RTRect2(Vector2D tl, Vector2D br)
        {
            TL = tl;
            BR = br;
            Area = (BR.X - TL.X) * (BR.Y - TL.Y);
        }
        public static explicit operator Rect2(RTRect2 rTRect2) => new Rect2((Vector2)rTRect2.TL, (Vector2)(rTRect2.BR - rTRect2.TL));
        public static explicit operator RTRect2(Rect2 rect2) => new RTRect2(rect2.Position, rect2.Position + rect2.Size);
        public bool IsOverLap(in RTRect2 other)
        {
            return !(
                TL.X > other.BR.X ||
                TL.Y > other.BR.Y ||
                BR.X < other.TL.X ||
                BR.Y < other.TL.Y
            );
        }
        /// <summary>
        ///   计算两矩形形成MBR的扩张面积
        /// </summary>
        public static double CalcExpandCost(in RTRect2 a, in RTRect2 b)
        {
            double MinX = Math.Min(a.TL.X, b.TL.X);
            double MinY = Math.Min(a.TL.Y, b.TL.Y);
            double MaxX = Math.Max(a.BR.X, b.BR.X);
            double MaxY = Math.Max(a.BR.Y, b.BR.Y);
            return (MaxX - MinX) * (MaxY - MinY) - a.Area - b.Area;
        }
        /// <summary>
        ///   计算该矩形在加入other后的扩张面积
        /// </summary>
        public double CalcExpandCost(in RTRect2 other)
        {
            double MinX = Math.Min(TL.X, other.TL.X);
            double MinY = Math.Min(TL.Y, other.TL.Y);
            double MaxX = Math.Max(BR.X, other.BR.X);
            double MaxY = Math.Max(BR.Y, other.BR.Y);
            return (MaxX - MinX) * (MaxY - MinY) - Area;
        }
    }
}