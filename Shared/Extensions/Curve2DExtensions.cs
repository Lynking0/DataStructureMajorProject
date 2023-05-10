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
                    curve.GetPointOut(i),
                    curve.GetPointIn(i)
                );
            }
        }
        /// <summary>
        ///   将当前曲线与若干曲线合并，不改变原曲线
        /// </summary>
        /// <returns>合并后的曲线，合并失败则报错</returns>
        public static Curve2D Concat(this Curve2D thisCurve, params Curve2D[] curves)
        {
            IEnumerable<(Vector2 pos, Vector2 @in, Vector2 @out)> points;
            Curve2D newCurve = new Curve2D();
            Curve2D lastCurve = thisCurve;
            for (int i = 0; i < curves.Length; ++i)
            {
                if (newCurve.PointCount == 0)
                {
                    if (lastCurve.PointCount == 0)
                    {
                        lastCurve = curves[i];
                        continue;
                    }
                    else if (curves[i].PointCount == 0)
                        continue;
                    else if (lastCurve.GetPointPosition(0).IsEqualApprox(curves[i].GetPointPosition(0)))
                        points = lastCurve.GetEnumerableReverse().Concat(curves[i].GetEnumerable());
                    else if (lastCurve.GetPointPosition(0).IsEqualApprox(curves[i].GetPointPosition(curves[i].PointCount - 1)))
                        points = lastCurve.GetEnumerableReverse().Concat(curves[i].GetEnumerableReverse());
                    else if (lastCurve.GetPointPosition(lastCurve.PointCount - 1).IsEqualApprox(curves[i].GetPointPosition(0)))
                        points = lastCurve.GetEnumerable().Concat(curves[i].GetEnumerable());
                    else if (lastCurve.GetPointPosition(lastCurve.PointCount - 1).IsEqualApprox(curves[i].GetPointPosition(curves[i].PointCount - 1)))
                        points = lastCurve.GetEnumerable().Concat(curves[i].GetEnumerableReverse());
                    else
                        throw new Exception("Shared.Extensions.Curve2DExtensions.Concat(): Value error!");
                }
                else
                {
                    if (curves[i].PointCount == 0)
                        continue;
                    else if (newCurve.GetPointPosition(newCurve.PointCount - 1).IsEqualApprox(curves[i].GetPointPosition(0)))
                        points = curves[i].GetEnumerable();
                    else if (newCurve.GetPointPosition(newCurve.PointCount - 1).IsEqualApprox(curves[i].GetPointPosition(curves[i].PointCount - 1)))
                        points = curves[i].GetEnumerableReverse();
                    else
                        throw new Exception("Shared.Extensions.Curve2DExtensions.Concat(): Value error!");
                }
                foreach ((Vector2 pos, Vector2 @in, Vector2 @out) in points)
                {
                    if (newCurve.PointCount > 0 && newCurve.GetPointPosition(newCurve.PointCount - 1) == pos && @in == Vector2.Zero && newCurve.GetPointOut(newCurve.PointCount - 1) == Vector2.Zero)
                        newCurve.SetPointOut(newCurve.PointCount - 1, @out);
                    else
                        newCurve.AddPoint(pos, @in, @out);
                }
            }
            if (newCurve.PointCount == 0)
            {
                foreach ((Vector2 pos, Vector2 @in, Vector2 @out) in lastCurve.GetEnumerable())
                    newCurve.AddPoint(pos, @in, @out);
            }
            return newCurve;
        }
    }
}