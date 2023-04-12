using System;
using Godot;
using GraphMoudle.DataStructureAndAlgorithm.OptimalCombinationAlgorithm;
using Shared.Extensions.DoubleVector2Extensions;
using GraphMoudle.DataStructureAndAlgorithm;
using GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTreeStructure;
namespace GraphMoudle
{
    using VEDC = VertexEdgeDistCalculator;
    using EEDC = EdgeEdgeDistCalculator;
    public class Edge : IRTreeData
    {
        public Vertex A;
        public Vertex B;
        public Curve2D Curve;
        public Edge(Vertex a, Vertex b, Curve2D curve)
        {
            A = a;
            B = b;
            Curve = curve;
        }
        /// <summary>
        ///   获取另一头的节点，若v不是该边的端点，则返回null。
        /// </summary>
        public Vertex? GetOtherEnd(Vertex v)
        {
            if (v == A)
                return B;
            if (v == B)
                return A;
            return null;
        }

        #region IRTreeDataImplementation

        public bool IsOverlap(IRTreeData other)
        {
            if (other is Vertex vertex)
            {
                if (vertex == A || vertex == B) // 若vertex为边的端点，则算作不碰撞
                    return false;
                VEDC.Instance.A = Curve.GetPointPosition(0);
                VEDC.Instance.B = Curve.GetPointPosition(0) + Curve.GetPointOut(0);
                VEDC.Instance.C = Curve.GetPointPosition(1) + Curve.GetPointIn(1);
                VEDC.Instance.D = Curve.GetPointPosition(1);
                VEDC.Instance.P = vertex.Position;
                VEDC.Instance.MinDistance = Graph.EdgesDistance;
                VEDC.Instance.MinStatus = 0;
                VEDC.Instance.MaxStatus = 1;
                return VEDC.Instance.Annealing().energy < Graph.EdgesDistance;
            }
            if (other is Edge edge)
            {
                EEDC.Instance.A1 = Curve.GetPointPosition(0);
                EEDC.Instance.B1 = Curve.GetPointPosition(0) + Curve.GetPointOut(0);
                EEDC.Instance.C1 = Curve.GetPointPosition(1) + Curve.GetPointIn(1);
                EEDC.Instance.D1 = Curve.GetPointPosition(1);
                EEDC.Instance.A2 = edge.Curve.GetPointPosition(0);
                EEDC.Instance.B2 = edge.Curve.GetPointPosition(0) + edge.Curve.GetPointOut(0);
                EEDC.Instance.C2 = edge.Curve.GetPointPosition(1) + edge.Curve.GetPointIn(1);
                EEDC.Instance.D2 = edge.Curve.GetPointPosition(1);
                EEDC.Instance.MinDistance = Graph.EdgesDistance;
                if (A == edge.B || B == edge.A)
                {
                    Utils.Swap(ref EEDC.Instance.A2, ref EEDC.Instance.D2);
                    Utils.Swap(ref EEDC.Instance.B2, ref EEDC.Instance.C2);
                }
                EEDC.Instance.MinStatus = A == edge.A || A == edge.B ? Graph.EdgeIgnoreRatio : 0;
                EEDC.Instance.MaxStatus = B == edge.A || B == edge.B ? 1 - Graph.EdgeIgnoreRatio : 1;
                double tempDist = EEDC.Instance.Annealing().energy;
                Utils.Swap(ref EEDC.Instance.A1, ref EEDC.Instance.A2);
                Utils.Swap(ref EEDC.Instance.B1, ref EEDC.Instance.B2);
                Utils.Swap(ref EEDC.Instance.C1, ref EEDC.Instance.C2);
                Utils.Swap(ref EEDC.Instance.D1, ref EEDC.Instance.D2);
                return Math.Min(tempDist, EEDC.Instance.Annealing().energy) < Graph.EdgesDistance;
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
            Vector2D p0 = Curve.GetPointPosition(0);
            Vector2D p1 = Curve.GetPointPosition(0) + Curve.GetPointOut(0);
            Vector2D p2 = Curve.GetPointPosition(1) + Curve.GetPointIn(1);
            Vector2D p3 = Curve.GetPointPosition(1);
            double minX, maxX, minY, maxY;
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
            return new RTRect2(new Vector2D(minX, minY), new Vector2D(maxX, maxY));
        }

        #endregion
    }
}