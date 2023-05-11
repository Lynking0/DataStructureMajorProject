using Godot;
using System.Collections.Generic;
using System.Linq;
using GraphMoudle;
using IndustryMoudle.Extensions;

namespace TransportMoudle
{
    public class TrafficFreeBlock
    {
        private List<Edge> _edges;
        private IReadOnlyList<Edge> Edges => _edges;
        public Dictionary<Vertex, TrainLine[]> PassingLine;
        private HashSet<Vertex> _vertexes;
        public IReadOnlyCollection<Vertex> Vertexes => _vertexes;

        public TrafficFreeBlock(IEnumerable<Edge> edges, Dictionary<Vertex, TrainLine[]> passingLine)
        {
            _edges = new List<Edge>(edges);
            _vertexes = edges.SelectMany(e => new Vertex[] { e.A, e.B }).Distinct().ToHashSet();
            PassingLine = passingLine.Where(p => _vertexes.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value);
        }

        public void GenerateFootPath()
        {
            // TODO: 合并部分重合的FootPath
            var ports = PassingLine.Keys.ToHashSet();
            foreach (var vertex in _vertexes.Where(v => !ports.Contains(v)))
            {
                var queue = new PriorityQueue<(Vertex vertex, List<Edge> edges), double>();
                var visitedEdges = new HashSet<Edge>();
                foreach (var edge in vertex.Adjacencies.Where(e => _edges.Contains(e)))
                {
                    queue.Enqueue((edge.GetOtherEnd(vertex)!, new List<Edge>(new[] { edge })), edge.Length);
                    visitedEdges.Add(edge);
                }
                while (queue.Count > 0)
                {
                    queue.TryDequeue(out var temp, out var length);
                    var (nextVertex, edges) = temp;
                    if (ports.Contains(nextVertex))
                    {
                        var l = new TrainLine(TrainLineLevel.FootPath);
                        edges.Reverse();
                        l.AddEdgeRange(edges);
                        break;
                    }
                    else
                    {
                        var a = nextVertex.Adjacencies;
                        var b = nextVertex.Adjacencies.Where(e => _edges.Contains(e)).ToList();
                        foreach (var edge in nextVertex.Adjacencies.Where(e => _edges.Contains(e)))
                        {
                            if (visitedEdges.Contains(edge))
                                continue;
                            var newEdges = new List<Edge>(edges);
                            newEdges.Add(edge);
                            queue.Enqueue((edge.GetOtherEnd(nextVertex)!, newEdges), length + edge.Length);
                            visitedEdges.Add(edge);
                        }
                    }
                }
            }
        }
    }
}