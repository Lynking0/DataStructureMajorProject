using System;
using Godot;
using Shared.Extensions.DoubleVector2Extensions;
using System.Collections.Generic;
using GraphMoudle.DataStructureAndAlgorithm.OptimalCombinationAlgorithm.ComputeShader;
using GraphMoudle.DataStructureAndAlgorithm.DisjointSet;
using GraphMoudle.DataStructureAndAlgorithm;
using static Shared.RandomMethods;

namespace GraphMoudle
{
    public partial class Graph
    {
        /// <summary>
        ///   首次筛选，将所有节点当作Intermediate看待并尝试寻找合法Edge，并将最后未找到Edge的节点标记为Isolated。
        /// </summary>
        private void FirstTimeFilter(List<(Vertex, Vertex)> pairs, List<Edge> alternativeEdges)
        {
            /// <summary>
            ///   将各个点对处理成(a, aCtrl, bCtrl, b)的形式
            /// </summary>
            EdgeEvaluatorInvoker.Data = new List<(Vector2D a, Vector2D b, Vector2D c, Vector2D d)>();
            foreach ((Vertex a, Vertex b) in pairs)
            {
                Vector2D aCtrl, bCtrl;
                if (Mathf.Abs(a.Gradient.OrthogonalD().AngleToD(b.Position - a.Position)) < Math.PI / 2)
                    aCtrl = a.Position + a.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance;
                else
                    aCtrl = a.Position - a.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance;
                if (Mathf.Abs(b.Gradient.OrthogonalD().AngleToD(a.Position - b.Position)) < Math.PI / 2)
                    bCtrl = b.Position + b.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance;
                else
                    bCtrl = b.Position - b.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance;
                EdgeEvaluatorInvoker.Data.Add((a.Position, aCtrl, bCtrl, b.Position));
            }
            // 计算海拔高度的合法性
            EdgeEvaluatorInvoker.Invoke();
            float[] isVaild = EdgeEvaluatorInvoker.Receive();
            for (int i = 0; i < pairs.Count; ++i)
            {
                if (isVaild[i] == 1.0)
                {
                    (Vertex a, Vertex b) = pairs[i];
                    (Vector2D _, Vector2D aCtrl, Vector2D bCtrl, Vector2D _) = EdgeEvaluatorInvoker.Data[i];
                    Edge edge = new Edge(a, b, new Curve2D());
                    edge.Curve.AddPoint((Vector2)a.Position, @out: (Vector2)(aCtrl - a.Position));
                    edge.Curve.AddPoint((Vector2)b.Position, @in: (Vector2)(bCtrl - b.Position));
                    alternativeEdges.Add(edge);
                    a.Type = Vertex.VertexType.Intermediate;
                    b.Type = Vertex.VertexType.Intermediate;
                }
            }
        }
        /// <summary>
        ///   第二次筛选，将刚刚未被标记为Intermediate的节点再判断一遍，看看能否变为Terminal。
        /// </summary>
        private void SecondTimeFilter(List<(Vertex, Vertex)> pairs, List<Edge> alternativeEdges)
        {
            /// <summary>
            ///   将两点分别是Isolated和Intermediate的各个点对处理成(a, aCtrl, bCtrl, b)的形式
            /// </summary>
            EdgeEvaluatorInvoker.Data = new List<(Vector2D a, Vector2D b, Vector2D c, Vector2D d)>();
            List<(Vertex, Vertex)> checkPairs = new List<(Vertex, Vertex)>();
            foreach ((Vertex, Vertex) pair in pairs)
            {
                (Vertex a, Vertex b) = pair;
                if (a.Type == Vertex.VertexType.Isolated && b.Type == Vertex.VertexType.Isolated)
                    continue;
                if (a.Type == Vertex.VertexType.Intermediate && b.Type == Vertex.VertexType.Intermediate)
                    continue;
                if (b.Type == Vertex.VertexType.Isolated)
                    Utils.Swap(ref a, ref b);
                // a是Isolated，b是Intermediate
                Vector2D aCtrl, bCtrl;
                if (Mathf.Abs((-a.Gradient).AngleToD(b.Position - a.Position)) < Math.PI / 2)
                    aCtrl = a.Position - a.Gradient.NormalizedD() * Graph.CtrlPointDistance;
                else
                    continue;
                if (Mathf.Abs(b.Gradient.OrthogonalD().AngleToD(a.Position - b.Position)) < Math.PI / 2)
                    bCtrl = b.Position + b.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance;
                else
                    bCtrl = b.Position - b.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance;
                checkPairs.Add((a, b));
                EdgeEvaluatorInvoker.Data.Add((a.Position, aCtrl, bCtrl, b.Position));
            }
            // 计算海拔高度的合法性
            EdgeEvaluatorInvoker.Invoke();
            float[] isVaild = EdgeEvaluatorInvoker.Receive();
            for (int i = 0; i < checkPairs.Count; ++i)
            {
                if (isVaild[i] == 1.0)
                {
                    // a是Isolated，b是Intermediate
                    (Vertex a, Vertex b) = checkPairs[i];
                    (Vector2D _, Vector2D aCtrl, Vector2D bCtrl, Vector2D _) = EdgeEvaluatorInvoker.Data[i];
                    Edge edge = new Edge(a, b, new Curve2D());
                    edge.Curve.AddPoint((Vector2)a.Position, @out: (Vector2)(aCtrl - a.Position));
                    edge.Curve.AddPoint((Vector2)b.Position, @in: (Vector2)(bCtrl - b.Position));
                    alternativeEdges.Add(edge);
                    a.Type = Vertex.VertexType.Terminal;
                }
            }
        }
        /// <summary>
        ///   根据边与边的最小距离从可选边中选择实际生成的边，并通过并查集生成分块信息。
        /// </summary>
        private void BuildEdges(List<Edge> alternativeEdges)
        {
            GD.Print(Vertices.Count);
            GD.Print(alternativeEdges.Count);
            RandomDislocate(alternativeEdges);
            foreach (Edge edge in alternativeEdges)
            {
                if (GISInfoStorer.CanAdd(edge))
                {
                    GISInfoStorer.Add(edge);
                    edge.A.Adjacencies.Add(edge);
                    edge.B.Adjacencies.Add(edge);
                }
            }
            GD.Print(GISInfoStorer.Count - Vertices.Count);
        }
    }
}