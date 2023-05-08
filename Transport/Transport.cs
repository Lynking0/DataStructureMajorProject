using Godot;
using System.Collections.Generic;
using System.Linq;
using GraphMoudle;
using IndustryMoudle.Link;
using IndustryMoudle.Extensions;
namespace TransportMoudle
{
    public class Transport
    {
        // 主干道占边的比例
        private const double MainLineRate = 0.15;
        // 主干道最小负载比例
        private const double MainLineMinLoadRate = 0.6;
        // 辅路最小负载比例
        private const double SideLineMinLoadRate = 0.2;
        // 主干道最大边数
        private const int MainLineMaxEdgeCount = 16;
        // 主干道最小边数
        private const int MainLineMinEdgeCount = 8;
        // 辅路最小边数
        private const int SideLineMinEdgeCount = 4;
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
        private static List<Edge> ExtendedAnEnd(Edge edge, Vertex vertex, int maxLoad, HashSet<Vertex> visitedVertexes, TrainLineLevel level)
        {
            double MinLoadRate = 0;
            switch (level)
            {
                case TrainLineLevel.MainLine:
                    MinLoadRate = MainLineMinLoadRate;
                    break;
                case TrainLineLevel.SideLine:
                    MinLoadRate = SideLineMinLoadRate;
                    break;
            }
            var result = new List<Edge>();
            if (vertex != edge.A && vertex != edge.B)
            {
                Logger.error("vertex is not in edge.");
                throw new System.Exception("vertex is not in edge.");
            }
            var curEdge = edge;
            var curVertex = vertex;
            visitedVertexes.Add(curVertex);
            while (true)
            {
                var edges = curVertex.Adjacencies
                    .Where(e => !visitedEdges.Contains(e))
                    .Where(e => !visitedVertexes.Contains(e.GetOtherEnd(curVertex)!))
                    .Where(e => ProduceLink.GetEdgeLoad(e).TotalLoad / (double)maxLoad > MinLoadRate);
                if (edges.Count() > 0)
                {
                    // curEdeg前进
                    curEdge = edges.Aggregate((a, b) => ProduceLink.GetEdgeLoad(a).TotalLoad > ProduceLink.GetEdgeLoad(b).TotalLoad ? a : b);
                    // curVertex前进
                    curVertex = curEdge.GetOtherEnd(curVertex)!;
                    result.Add(curEdge);
                    visitedEdges.Add(curEdge);
                    visitedVertexes.Add(curVertex);
                }
                else
                    // 到头了
                    break;
            }
            return result;
        }
        private static void BuildMainTrainLine(Edge edge, int maxLoad)
        {
            var visitedVertexes = new HashSet<Vertex>();
            var edges = new List<Edge>();
            visitedEdges.Add(edge);
            var left = ExtendedAnEnd(edge, edge.A, maxLoad, visitedVertexes, TrainLineLevel.MainLine);
            left.Reverse();
            edges.AddRange(left);
            edges.Add(edge);
            edges.AddRange(ExtendedAnEnd(edge, edge.B, maxLoad, visitedVertexes, TrainLineLevel.MainLine));
            if (edges.Count < MainLineMinEdgeCount)
            {
                foreach (var e in edges)
                {
                    visitedEdges.Remove(e);
                    visitedVertexes.Remove(e.A);
                    visitedVertexes.Remove(e.B);
                }
                return;
            }
            while (edges.Count > MainLineMaxEdgeCount)
            {
                var minLoadEdge = edges.First().GetLoadInfo().TotalLoad > edges.Last().GetLoadInfo().TotalLoad ? edges.Last() : edges.First();
                edges.Remove(minLoadEdge);
                visitedEdges.Remove(minLoadEdge);
                visitedVertexes.Remove(minLoadEdge.A);
                visitedVertexes.Remove(minLoadEdge.B);
            }
            var line = new TrainLine(TrainLineLevel.MainLine);
            line.AddEdgeRange(edges);
        }
        private static void BuildSideLine(Vertex vertex)
        {
            var edges = new List<Edge>();
            var visitedVertexes = new HashSet<Vertex>();
            var curEdge = vertex.Adjacencies
                .Where(e => !visitedEdges.Contains(e))
                .OrderByDescending(e => e.GetLoadInfo().TotalLoad)
                .FirstOrDefault();
            if (curEdge is null)
                return;
            edges.Add(curEdge);
            visitedEdges.Add(curEdge);
            visitedVertexes.Add(vertex);
            edges.AddRange(ExtendedAnEnd(curEdge, curEdge.GetOtherEnd(vertex)!, curEdge.GetLoadInfo().TotalLoad, visitedVertexes, TrainLineLevel.SideLine));
            if (edges.Count < SideLineMinEdgeCount)
            {
                foreach (var e in edges)
                {
                    visitedEdges.Remove(e);
                    visitedVertexes.Remove(e.A);
                    visitedVertexes.Remove(e.B);
                }
                return;
            }
            var line = new TrainLine(TrainLineLevel.SideLine);
            line.AddEdgeRange(edges);
        }
        public static void BuildTrainLines()
        {
            // 确定主干道
            var totalEdges = Graph.Instance.Edges
                                                    .OrderByDescending(e => ProduceLink.GetEdgeLoad(e).TotalLoad)
                                                    .Where(e => !visitedEdges.Contains(e));
            foreach (var edge in totalEdges)
            {
                if ((double)visitedEdges.Count() / totalEdges.Count() > MainLineRate)
                    break;
                BuildMainTrainLine(edge, edge.GetLoadInfo().TotalLoad);
            }
            // 依托主干道构建辅路
            var mainLines = TrainLine.TrainLines.Where(l => l.Level == TrainLineLevel.MainLine).ToArray();
            foreach (var vertex in mainLines
                                            .SelectMany(l => l.Vertexes))
            {
                BuildSideLine(vertex);
            }


            var sideLines = TrainLine.TrainLines.Where(l => l.Level == TrainLineLevel.SideLine).ToArray();

            var trafficFreeBlocks = new List<TrafficFreeBlock>();
            var trafficFreeEdges = Graph.Instance.Edges
                                    .Where(e => !TrainLine.TrainLines.SelectMany(l => l.Edges).Contains(e));
            foreach (var edge in trafficFreeEdges)
            {
                if (visitedEdges.Contains(edge))
                    continue;
                var edges = new List<Edge>();
                edges.Add(edge);
                visitedEdges.Add(edge);
                var waitVertexes = new List<Vertex>();
                waitVertexes.AddRange(new[] { edge.A, edge.B });
                while (waitVertexes.Count > 0)
                {
                    var vertex = waitVertexes.First();
                    waitVertexes.Remove(vertex);
                    var adjacentEdges = vertex.Adjacencies
                        .Where(e => !visitedEdges.Contains(e))
                        .Where(e => trafficFreeEdges.Contains(e));
                    foreach (var curEdge in adjacentEdges)
                    {
                        edges.Add(curEdge);
                        visitedEdges.Add(curEdge);
                        waitVertexes.AddRange(new[] { curEdge.A, curEdge.B });
                    }
                }
                var line = new TrainLine(TrainLineLevel.FootPath);
                line.AddEdgeRange(edges);
                // TODO: 基层配送拆解
                var trafficFreeBlock = new TrafficFreeBlock(edges);
            }
            var footPaths = TrainLine.TrainLines.Where(l => l.Level == TrainLineLevel.FootPath).ToArray();
            var edgeCount = Graph.Instance.Edges.Count();
            var mainLineEdgeCount = mainLines.Sum(l => l.Edges.Count);
            var sideLineEdgeCount = sideLines.Sum(l => l.Edges.Count);
            var footPathEdgeCount = footPaths.Sum(l => l.Edges.Count);
            Logger.trace($"主干道：{mainLines.Count()}条，占比{mainLineEdgeCount / (double)edgeCount * 100}%");
            Logger.trace($"辅路：{sideLines.Count()}条，占比{sideLineEdgeCount / (double)edgeCount * 100}%");
            Logger.trace($"基层配送：{footPaths.Count()}条，占比{footPathEdgeCount / (double)edgeCount * 100}%");
        }
    }
}