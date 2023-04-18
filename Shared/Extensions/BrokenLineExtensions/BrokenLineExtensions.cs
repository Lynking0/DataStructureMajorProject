using System;
using System.Collections.Generic;
using Shared.Extensions.DoubleVector2Extensions;
using Godot;

namespace Shared.Extensions.BrokenLineExtensions
{
    public static class _BrokenLineExtensions
    {
        /// <summary>
        ///   判断线段ab与cd是否相交
        /// </summary>
        private static bool IsIntersect(Vector2D a, Vector2D b, Vector2D c, Vector2D d)
        {
            Vector2D ab = b - a;
            Vector2D cd = d - c;
            return cd.CrossD(a - c) * cd.CrossD(b - c) <= 0 && ab.CrossD(c - a) * ab.CrossD(d - a) <= 0;
        }
        private static double GetSegmentPointSquaredDist(Vector2D p, Vector2D a, Vector2D b)
        {
            Vector2D ap = p - a;
            Vector2D bp = p - b;
            Vector2D ab = b - a;
            double dotA = ap.DotD(ab);
            double dotB = bp.DotD(-ab);
            if (dotA < 0 || dotB < 0)
                return double.PositiveInfinity;
            double cross = ap.CrossD(ab); // 注意cross可能为负
            return cross * cross / ab.LengthSquaredD();
        }
        public static bool IsTooClose(this IBrokenLineLike shape, Vector2D point, double minDistSquared)
        {
            if (point.DistanceSquaredToD(shape.Points[0]) < minDistSquared)
                return true;
            for (int i = 1; i < shape.Points.Count; ++i)
            {
                if (point.DistanceSquaredToD(shape.Points[i]) < minDistSquared)
                    return true;
                if (GetSegmentPointSquaredDist(point, shape.Points[i - 1], shape.Points[i]) < minDistSquared)
                    return true;
            }
            return false;
        }
        public static bool IsTooClose(this IBrokenLineLike shape, IBrokenLineLike other, double minDistSquared)
        {
            for (int i = 1; i < shape.Points.Count; ++i)
                for (int j = 1; j < other.Points.Count; ++j)
                    if (IsIntersect(shape.Points[i - 1], shape.Points[i], other.Points[j], other.Points[j - 1]))
                        return true;
            for (int i = 0; i < shape.Points.Count; ++i)
                if (other.IsTooClose(shape.Points[i], minDistSquared))
                    return true;
            for (int i = 0; i < other.Points.Count; ++i)
                if (shape.IsTooClose(other.Points[i], minDistSquared))
                    return true;
            return false;
        }
        /// <summary>
        ///   两条折线前IgnoreDist距离为忽略区域，这段区域只要不相交（第一个点除外）均视为合法。
        /// </summary>
        public static bool IsTooCloseIgnore(this IBrokenLineLike shape, IBrokenLineLike other, double minDistSquared, double ignoreDist, bool shouldReverse, bool shouldOtherReverse)
        {
            List<Vector2D> shapePoints = new List<Vector2D>(shape.Points);
            List<Vector2D> otherPoints = new List<Vector2D>(other.Points);
            if (shouldReverse)
                shapePoints.Reverse();
            if (shouldOtherReverse)
                otherPoints.Reverse();
            // 两折线第一段必相交，故不检测
            for (int i = 1; i < shapePoints.Count; ++i)
                for (int j = 1; j < otherPoints.Count; ++j)
                    if ((i != 1 || j != 1) && IsIntersect(shapePoints[i - 1], shapePoints[i], otherPoints[j], otherPoints[j - 1]))
                        return true;
            double sumLen = 0;
            for (int i = 1; i < shapePoints.Count; ++i)
            {
                if (sumLen < ignoreDist)
                {
                    sumLen += shapePoints[i].DistanceToD(shapePoints[i - 1]);
                    if (sumLen > ignoreDist)
                    {
                        if (other.IsTooClose(
                            shapePoints[i].MoveTowardD(shapePoints[i - 1], sumLen - ignoreDist)
                            , minDistSquared
                        ))
                            return true;
                    }
                }
                if (sumLen >= ignoreDist)
                {
                    if (other.IsTooClose(shapePoints[i], minDistSquared))
                        return true;
                }
            }
            sumLen = 0;
            for (int i = 1; i < otherPoints.Count; ++i)
            {
                if (sumLen < ignoreDist)
                {
                    sumLen += otherPoints[i].DistanceToD(otherPoints[i - 1]);
                    if (sumLen > ignoreDist)
                    {
                        if (shape.IsTooClose(
                            otherPoints[i].MoveTowardD(otherPoints[i - 1], sumLen - ignoreDist)
                            , minDistSquared
                        ))
                            return true;
                    }
                }
                if (sumLen >= ignoreDist)
                {
                    if (shape.IsTooClose(otherPoints[i], minDistSquared))
                        return true;
                }
            }
            return false;
        }
    }
}