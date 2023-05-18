using System;
using Godot;
using System.Collections.Generic;
using GraphMoudle.DataStructureAndAlgorithm.OptimalCombinationAlgorithm;
using Shared.Extensions.DoubleVector2Extensions;
using Shared.Extensions.BrokenLineExtensions;
using GraphMoudle.DataStructureAndAlgorithm;
using GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTreeStructure;
namespace GraphMoudle
{
    public class Edge : IRTreeData, IBrokenLineLike
    {
        public readonly Vertex A;
        public readonly Vertex B;
        public readonly Curve2D Curve;
        public bool IsBridge => A.ParentBlock != B.ParentBlock;
        private float? _length = null;
        public float Length => _length ??= Curve.GetBakedLength();
        public Edge(Vertex a, Vertex b, Curve2D curve, int pMaxDepth = 4, double pAngle = Math.PI * 5 / 180)
        {
            A = a;
            B = b;
            Curve = curve;
            PMaxDepth = pMaxDepth;
            PAngle = pAngle;
        }
        /// <summary>
        ///   获取节点v在当前边另一头的节点，若v不是当前边的端点，则返回null。
        /// </summary>
        public Vertex? GetOtherEnd(Vertex v)
        {
            if (v == A)
                return B;
            if (v == B)
                return A;
            return null;
        }
        /// <summary>
        ///   获取当前边在节点v的控制点的方向角，若v不是当前边的端点，则返回null。
        /// </summary>
        public float? GetCtrlAngle(Vertex v)
        {
            if (v == A)
                return Curve.GetPointOut(0).Angle();
            if (v == B)
                return Curve.GetPointIn(Curve.PointCount - 1).Angle();
            return null;
        }
        /// <summary>
        ///   以节点v为中心旋转当前边在节点v的控制点，若v不是当前边的端点，则不做任何操作
        /// </summary>
        public void RotateCtrlPoint(Vertex v, float angle)
        {
            if (v == A)
                Curve.SetPointOut(0, Curve.GetPointOut(0).Rotated(angle));
            if (v == B)
                Curve.SetPointIn(Curve.PointCount - 1, Curve.GetPointIn(Curve.PointCount - 1).Rotated(angle));
            ClearCacheInfo(); // 曲线改变，需清理缓存信息
        }
        /// <summary>
        ///   以节点v为中心旋转当前边在节点v的控制点，若v不是当前边的端点，则不做任何操作
        /// </summary>
        public void RotateCtrlPointTo(Vertex v, float angle)
        {
            if (v == A)
                Curve.SetPointOut(0, Curve.GetPointOut(0).RotatedTo(angle));
            if (v == B)
                Curve.SetPointIn(Curve.PointCount - 1, Curve.GetPointIn(Curve.PointCount - 1).RotatedTo(angle));
            ClearCacheInfo(); // 曲线改变，需清理缓存信息
        }
        public Curve2D GetCurveCopy()
        {
            Curve2D curve = new Curve2D();
            for (int i = 0; i < Curve.PointCount; ++i)
            {
                curve.AddPoint(
                    Curve.GetPointPosition(i),
                    @in: Curve.GetPointIn(i),
                    @out: Curve.GetPointOut(i)
                );
            }
            return curve;
        }
        public void SetCurveCopy(Curve2D curve)
        {
            Curve.ClearPoints();
            for (int i = 0; i < curve.PointCount; ++i)
            {
                Curve.AddPoint(
                    curve.GetPointPosition(i),
                    @in: curve.GetPointIn(i),
                    @out: curve.GetPointOut(i)
                );
            }
            ClearCacheInfo(); // 曲线改变，需清理缓存信息
        }
        private void ClearCacheInfo()
        {
            _length = null;
            _points = null;
            _rectangle = null;
        }

        #region IBrokenLineLikeImplementation

        private List<Vector2D>? _points = null;
        public List<Vector2D> Points { get => _points ??= _getBrokenLinePoints(); }

        private readonly int PMaxDepth;
        private readonly double PAngle;

        private List<Vector2D> _getBrokenLinePoints()
        {
            List<Vector2D> result = new List<Vector2D>();
            result.Add(Curve.GetPointPosition(0));
            for (int i = 1; i < Curve.PointCount; ++i)
            {
                __getBrokenLinePoints(
                    result, 0, 1, 0,
                    Curve.GetPointPosition(i - 1),
                    Curve.GetPointPosition(i - 1) + Curve.GetPointOut(i - 1),
                    Curve.GetPointPosition(i) + Curve.GetPointIn(i),
                    Curve.GetPointPosition(i)
                );
                result.Add(Curve.GetPointPosition(i));
            }
            return result;
        }
        private void __getBrokenLinePoints(List<Vector2D> result, double minT, double maxT, int depth, Vector2D a, Vector2D b, Vector2D c, Vector2D d)
        {
            if (depth == PMaxDepth)
                return;

            double midT = (minT + maxT) * 0.5;

            __getBrokenLinePoints(result, minT, midT, depth + 1, a, b, c, d);

            double _minT = 1 - minT;
            double _midT = 1 - midT;
            double _maxT = 1 - maxT;

            Vector2D start = a * (_minT * _minT * _minT) + b * (3 * _minT * _minT * minT) + c * (3 * _minT * minT * minT) + d * (minT * minT * minT);
            Vector2D middle = a * (_midT * _midT * _midT) + b * (3 * _midT * _midT * midT) + c * (3 * _midT * midT * midT) + d * (midT * midT * midT);
            Vector2D end = a * (_maxT * _maxT * _maxT) + b * (3 * _maxT * _maxT * maxT) + c * (3 * _maxT * maxT * maxT) + d * (maxT * maxT * maxT);

            Vector2D normalA = (middle - start).NormalizedD();
            Vector2D normalB = (end - middle).NormalizedD();

            // 角度过大则进行分段
            if (normalA.DotD(normalB) < Math.Cos(PAngle))
                result.Add(middle);

            __getBrokenLinePoints(result, midT, maxT, depth + 1, a, b, c, d);
        }

        #endregion

        #region IRTreeDataImplementation

        public bool IsOverlap(IRTreeData other)
        {
            if (other is Vertex vertex)
            {
                if (vertex == A || vertex == B) // 若vertex为边的端点，则算作不碰撞
                    return false;
                return this.IsTooClose(vertex.Position, Graph.EdgesDistanceSquared);
            }
            if (other is Edge edge)
            {
                if (A == edge.A)
                {
                    return this.IsTooCloseIgnore(edge,
                        Graph.EdgesDistanceSquared, Graph.EdgeIgnoreDist, false, false);
                }
                if (A == edge.B)
                {
                    return this.IsTooCloseIgnore(edge,
                        Graph.EdgesDistanceSquared, Graph.EdgeIgnoreDist, false, true);
                }
                if (B == edge.A)
                {
                    return this.IsTooCloseIgnore(edge,
                        Graph.EdgesDistanceSquared, Graph.EdgeIgnoreDist, true, false);
                }
                if (B == edge.B)
                {
                    return this.IsTooCloseIgnore(edge,
                        Graph.EdgesDistanceSquared, Graph.EdgeIgnoreDist, true, true);
                }
                return this.IsTooClose(edge, Graph.EdgesDistanceSquared);
            }
            throw new Exception($"{GetType()}.IsOverlap(IRTreeData): Unexpected type.");
        }
        private RTRect2? _rectangle = null;
        public RTRect2 Rectangle { get => _rectangle ??= _getRectangle(); }
        /// <summary>
        ///   边与边之间最小距离为Graph.EdgesDistance，在MBR的基础上需向四边扩张Graph.EdgesDistance / 2距离
        /// </summary>
        private RTRect2 _getRectangle()
        {
            // x = (-x0+3x1-3x2+x3)t^3 + (3x0-6x1+3x2)t^2 + (-3x0+3x1)t + x0
            // x'= 3(-x0+3x1-3x2+x3)t^2 + 2(3x0-6x1+3x2)t + (-3x0+3x1) = 0
            double minXAns = System.Double.MaxValue;
            double maxXAns = System.Double.MinValue;
            double minYAns = System.Double.MaxValue;
            double maxYAns = System.Double.MinValue;
            for (int i = 1; i < Curve.PointCount; ++i)
            {
                double minX, maxX, minY, maxY;
                Vector2D p0 = Curve.GetPointPosition(i - 1);
                Vector2D p1 = Curve.GetPointPosition(i - 1) + Curve.GetPointOut(i - 1);
                Vector2D p2 = Curve.GetPointPosition(i) + Curve.GetPointIn(i);
                Vector2D p3 = Curve.GetPointPosition(i);
                double a, b, c, d, a_, b_, c_, t1, t2;
                // X
                a = -p0.X + 3 * p1.X - 3 * p2.X + p3.X;
                b = 3 * p0.X - 6 * p1.X + 3 * p2.X;
                c = -3 * p0.X + 3 * p1.X;
                d = p0.X;
                a_ = 3 * a;
                b_ = 2 * b;
                c_ = c;
                t1 = (-b_ + Mathf.Sqrt(b_ * b_ - 4 * a_ * c_)) / (2 * a_);
                t2 = (-b_ - Mathf.Sqrt(b_ * b_ - 4 * a_ * c_)) / (2 * a_);
                minX = Mathf.Min(d, a + b + c + d);
                maxX = Mathf.Max(d, a + b + c + d);
                if (t1 >= 0 && t1 <= 1)
                {
                    minX = Mathf.Min(minX, a * t1 * t1 * t1 + b * t1 * t1 + c * t1 + d);
                    maxX = Mathf.Max(maxX, a * t1 * t1 * t1 + b * t1 * t1 + c * t1 + d);
                }
                if (t2 >= 0 && t2 <= 1)
                {
                    minX = Mathf.Min(minX, a * t2 * t2 * t2 + b * t2 * t2 + c * t2 + d);
                    maxX = Mathf.Max(maxX, a * t2 * t2 * t2 + b * t2 * t2 + c * t2 + d);
                }
                minX -= Graph.EdgesDistance / 2;
                maxX += Graph.EdgesDistance / 2;
                // Y
                a = -p0.Y + 3 * p1.Y - 3 * p2.Y + p3.Y;
                b = 3 * p0.Y - 6 * p1.Y + 3 * p2.Y;
                c = -3 * p0.Y + 3 * p1.Y;
                d = p0.Y;
                a_ = 3 * a;
                b_ = 2 * b;
                c_ = c;
                t1 = (-b_ + Mathf.Sqrt(b_ * b_ - 4 * a_ * c_)) / (2 * a_);
                t2 = (-b_ - Mathf.Sqrt(b_ * b_ - 4 * a_ * c_)) / (2 * a_);
                minY = Mathf.Min(d, a + b + c + d);
                maxY = Mathf.Max(d, a + b + c + d);
                if (t1 >= 0 && t1 <= 1)
                {
                    minY = Mathf.Min(minY, a * t1 * t1 * t1 + b * t1 * t1 + c * t1 + d);
                    maxY = Mathf.Max(maxY, a * t1 * t1 * t1 + b * t1 * t1 + c * t1 + d);
                }
                if (t2 >= 0 && t2 <= 1)
                {
                    minY = Mathf.Min(minY, a * t2 * t2 * t2 + b * t2 * t2 + c * t2 + d);
                    maxY = Mathf.Max(maxY, a * t2 * t2 * t2 + b * t2 * t2 + c * t2 + d);
                }
                minY -= Graph.EdgesDistance / 2;
                maxY += Graph.EdgesDistance / 2;
                minXAns = Math.Min(minX, minXAns);
                maxXAns = Math.Max(maxX, maxXAns);
                minYAns = Math.Min(minY, minYAns);
                maxYAns = Math.Max(maxY, maxYAns);
            }
            return new RTRect2(new Vector2D(minXAns, minYAns), new Vector2D(maxXAns, maxYAns));
        }

        #endregion
    }
}