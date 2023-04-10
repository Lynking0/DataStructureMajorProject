using System;
using Shared.Extensions.DoubleVector2Extensions;

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
        public bool IsOverLap(RTRect2 other)
        {
            return !(
                TL.X > other.BR.X ||
                TL.Y > other.BR.Y ||
                BR.X < other.TL.X ||
                BR.Y < other.TL.Y
            );
        }
        /// <summary>
        ///   计算该矩形在加入other后的扩张面积
        /// </summary>
        public double CalcExpandCost(RTRect2 other)
        {
            double MinX = Math.Min(TL.X, other.TL.X);
            double MinY = Math.Min(TL.Y, other.TL.Y);
            double MaxX = Math.Max(BR.X, other.BR.X);
            double MaxY = Math.Max(BR.Y, other.BR.Y);
            return (MaxX - MinX) * (MaxY - MinY) - Area;
        }
        /// <summary>
        ///   计算该矩形在加入other后的扩张面积以及扩张后的矩形
        /// </summary>
        public double CalcExpandCost(RTRect2 other, out RTRect2 expandedRect)
        {
            expandedRect = new RTRect2(
                new Vector2D(
                    Math.Min(TL.X, other.TL.X),
                    Math.Min(TL.Y, other.TL.Y)
                ),
                new Vector2D(
                    Math.Max(BR.X, other.BR.X),
                    Math.Max(BR.Y, other.BR.Y)
                )
            );
            return expandedRect.Area - Area;
        }
    }
}