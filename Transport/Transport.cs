using Godot;
using System.Collections.Generic;
using System.Linq;
using GraphMoudle;
using IndustryMoudle.Link;
namespace TransportMoudle
{
    public class Transport
    {
        private const double MainLineRate = 0.2;
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
        private static List<Edge> ExtendedAnEnd(Edge edge, Vertex vertex, int maxLoad, HashSet<Vertex> visitedVertex)
        {
            var result = new List<Edge>();
            if (vertex != edge.A && vertex != edge.B)
            {
                Logger.error("vertex is not in edge.");
                throw new System.Exception("vertex is not in edge.");
            }
            var curEdge = edge;
            var curVertex = vertex;
            visitedVertex.Add(curVertex);
            while (true)
            {
                result.Add(curEdge);
                // curVertex后移
                curVertex = curEdge.GetOtherEnd(curVertex)!;
                visitedVertex.Add(curVertex);
                try
                {
                    curEdge = curVertex.Adjacencies
                    .Where(e => !visitedVertex.Contains(e.GetOtherEnd(curVertex)!))
                    .Where(e => ProduceLink.GetEdgeLoad(e).TotalLoad / (double)maxLoad > 0.5)
                    .Aggregate((a, b) => ProduceLink.GetEdgeLoad(a).TotalLoad > ProduceLink.GetEdgeLoad(b).TotalLoad ? a : b);
                }
                catch (System.InvalidOperationException)
                {
                    // 到头了
                    break;
                }
            }
            return result;
        }
        private static TrainLine BuildTrainLine(Edge edge, int maxLoad)
        {
            var visitedVertex = new HashSet<Vertex>();
            var line = new TrainLine();
            var left = ExtendedAnEnd(edge, edge.A, maxLoad, visitedVertex);
            left.Reverse();
            line.AddEdgeRange(left);
            line.AddEdge(edge);
            line.AddEdgeRange(ExtendedAnEnd(edge, edge.B, maxLoad, visitedVertex));
            return line;
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
                var line = BuildTrainLine(mainEdge, load);
                foreach (var edge in line.Edges)
                {
                    visitedEdges.Add(edge);
                }
            }
        }
    }
}