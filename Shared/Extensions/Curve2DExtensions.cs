using System;
using Godot;
using System.Collections.Generic;
using Shared.Extensions.ICollectionExtensions;
/// <summary>
///   提供IEnumerable<T>和ICollection<T>的扩展方法
/// </summary>
namespace Shared.Extensions.Curve2DExtensions
{
    public static class _Curve2DExtensions
    {
        public static IEnumerable<(Vector2 pos, Vector2 @in, Vector2 @out)> GetEnumerable(this Curve2D curve)
        {
            for (int i = 0; i < curve.PointCount; ++i)
            {
                yield return (
                    curve.GetPointPosition(i),
                    curve.GetPointIn(i),
                    curve.GetPointOut(i)
                );
            }
        }
        public static IEnumerable<(Vector2 pos, Vector2 @in, Vector2 @out)> GetEnumerableReverse(this Curve2D curve)
        {
            for (int i = curve.PointCount - 1; i >= 0; --i)
            {
                yield return (
                    curve.GetPointPosition(i),
                    curve.GetPointIn(i),
                    curve.GetPointOut(i)
                );
            }
        }
        /// <summary>
        ///   将当前曲线与若干曲线合并，不改变原曲线
        /// </summary>
        /// <returns>合并后的曲线，合并失败则返回null</returns>
        public static Curve2D? Concat(this Curve2D thisCurve, params Curve2D[] curves)
        {
            IEnumerable<(Vector2 pos, Vector2 @in, Vector2 @out)> points;
            if (thisCurve.GetPointPosition(0).IsEqualApprox(curves[0].GetPointPosition(0)))
                points = thisCurve.GetEnumerableReverse().Concat(curves[0].GetEnumerable());
            else if (thisCurve.GetPointPosition(0).IsEqualApprox(curves[0].GetPointPosition(curves[0].PointCount - 1)))
                points = thisCurve.GetEnumerableReverse().Concat(curves[0].GetEnumerableReverse());
            else if (thisCurve.GetPointPosition(thisCurve.PointCount - 1).IsEqualApprox(curves[0].GetPointPosition(0)))
                points = thisCurve.GetEnumerable().Concat(curves[0].GetEnumerable());
            else if (thisCurve.GetPointPosition(thisCurve.PointCount - 1).IsEqualApprox(curves[0].GetPointPosition(curves[0].PointCount - 1)))
                points = thisCurve.GetEnumerable().Concat(curves[0].GetEnumerableReverse());
            else
                return null;
            Curve2D newCurve = new Curve2D();
            foreach ((Vector2 pos, Vector2 @in, Vector2 @out) in points)
                newCurve.AddPoint(pos, @in, @out);
            for (int i = 1; i < curves.Length; ++i)
            {
                if (newCurve.GetPointPosition(newCurve.PointCount - 1).IsEqualApprox(curves[i].GetPointPosition(0)))
                    points = curves[i].GetEnumerable();
                else if (newCurve.GetPointPosition(newCurve.PointCount - 1).IsEqualApprox(curves[i].GetPointPosition(curves[i].PointCount - 1)))
                    points = curves[i].GetEnumerableReverse();
                else
                    return null;
                foreach ((Vector2 pos, Vector2 @in, Vector2 @out) in points)
                    newCurve.AddPoint(pos, @in, @out);
            }
            return newCurve;
        }
    }
}