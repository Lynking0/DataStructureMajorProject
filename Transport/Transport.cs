using Godot;
using System.Collections.Generic;
using System.Linq;
using GraphMoudle;
using IndustryMoudle.Link;
namespace TransportMoudle
{
    public class Transport
    {
        // 主干道占边的比例
        private const double MainLineRate = 0.2;
        // 最小负载比例
        private const double MinLoadRate = 0.4;
        private static HashSet<Edge> visitedEdges = new HashSet<Edge>();
        private class IntReverseComparer : IComparer<int>
        {
            public int Compare(int p1, int p2)
            {
                if (p1 > p2)
                    return -1;
                if (p1 == p2)
                {
                    return 0;
                }
                return 1;
            }
        }

        /// <returns>从edge的vertex到端点的边集，不包含edge</returns>
        private static List<Edge> ExtendedAnEnd(Edge edge, Vertex vertex, int maxLoad)
        {
            var result = new List<Edge>();
            if (vertex != edge.A && vertex != edge.B)
            {
                Logger.error("vertex is not in edge.");
                throw new System.Exception("vertex is not in edge.");
            }
            var curEdge = edge;
            var curVertex = vertex;
            while (true)
            {
                var l = curVertex.Adjacencies
                    .Where(e => !visitedEdges.Contains(e))
                    .Where(e => ProduceLink.GetEdgeLoad(e).TotalLoad / (double)maxLoad > MinLoadRate);
                if (l.Count() > 0)
                {
                    // curEdeg前进
                    curEdge = l.Aggregate((a, b) => ProduceLink.GetEdgeLoad(a).TotalLoad > ProduceLink.GetEdgeLoad(b).TotalLoad ? a : b);
                    // curVertex前进
                    curVertex = curEdge.GetOtherEnd(curVertex)!;
                    result.Add(curEdge);
                    visitedEdges.Add(curEdge);
                }
                else
                    // 到头了
                    break;
            }
            return result;
        }
        private static void BuildMainTrainLine(Edge edge, int maxLoad)
        {
            var edges = new List<Edge>();
            var left = ExtendedAnEnd(edge, edge.A, maxLoad);
            left.Reverse();
            edges.AddRange(left);
            edges.Add(edge);
            visitedEdges.Add(edge);
            edges.AddRange(ExtendedAnEnd(edge, edge.B, maxLoad));
            if (edges.Count < 8)
            {
                foreach (var e in edges)
                {
                    visitedEdges.Remove(e);
                }
                return;
            }
            var line = new TrainLine(TrainLineLevel.MainLine);
            line.AddEdgeRange(edges);
        }

        public static void BuildTrainLines()
        {
            // 确定主干道
            var visitedEdges = new HashSet<Edge>();
            var edgeLoad = new PriorityQueue<Edge, int>(new IntReverseComparer());

            foreach (var edge in Graph.Instance.Edges)
            {
                edgeLoad.Enqueue(edge, ProduceLink.GetEdgeLoad(edge).TotalLoad);
            }
            var edgeCount = edgeLoad.Count;
            // TODO:主干道边界判定
            while ((double)edgeLoad.Count / edgeCount > 1 - MainLineRate)
            {
                Edge? mainEdge = null;
                int load = 0;
                while (mainEdge is null || visitedEdges.Contains(mainEdge!))
                {
                    mainEdge = edgeLoad.Dequeue();
                    edgeLoad.TryDequeue(out mainEdge, out load);
                }
                if (mainEdge is null)
                    return;
                BuildMainTrainLine(mainEdge, load);
            }
        }
    }
}