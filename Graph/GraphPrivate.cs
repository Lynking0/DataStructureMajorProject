using System;
using Godot;
using Shared.Extensions.DoubleVector2Extensions;
using System.Collections.Generic;
using GraphMoudle.DataStructureAndAlgorithm.OptimalCombinationAlgorithm.ComputeShader;

namespace GraphMoudle
{
    public partial class Graph
    {
        /// <summary>
        ///   首次筛选，将所有节点当作Intermediate看待并尝试寻找合法Edge，并将最后未找到Edge的节点标记为Isolated。
        /// </summary>
        private void FirstTimeFilter(List<(Vertex, Vertex)> pairs, List<Edge> alternativeEdges)
        {
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
        ///   第二次筛选，将刚刚被标记为Isolated的节点在判断一遍，看看能否变为Terminal。
        /// </summary>
        private void SecondTimeFilter(List<(Vertex, Vertex)> pairs, List<Edge> alternativeEdges)
        {
            HashSet<Vertex> terminalSet = new HashSet<Vertex>();
            foreach (Vertex vertex in Vertices)
                if (vertex.Type == Vertex.VertexType.Isolated)
                    terminalSet.Add(vertex);
            foreach ((Vertex, Vertex) pair in pairs)
            {
                (Vertex a, Vertex b) = pair;
                if (a.Type == Vertex.VertexType.Isolated && b.Type == Vertex.VertexType.Isolated)
                    continue;
                if (a.Type == Vertex.VertexType.Intermediate && b.Type == Vertex.VertexType.Intermediate)
                    continue;
                if (b.Type == Vertex.VertexType.Isolated)
                {
                    (a, b) = (b, a);
                    // a是Isolated，b是Intermediate
                }
            }
        }
    }
}