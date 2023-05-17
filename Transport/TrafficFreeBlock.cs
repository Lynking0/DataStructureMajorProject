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
            var ports = PassingLine.Keys.ToHashSet();
            var lineEdges = new List<List<Edge>>();
            foreach (var vertex in _vertexes.Where(v => !ports.Contains(v)))
            {
                // TODO: 三级路要按照link生成，不然有些路会空出来
                // 按link生成
                foreach (var link in vertex.GetLinks())
                {
                    var vertexes = new HashSet<Vertex>();
                    var edges = new List<Edge>();
                    foreach (var e in link.EdgeInfos.Select(info => info.Edge).Where(e => e is null ? false : _edges.Contains(e)))
                    {
                        if (e is null)
                            continue;
                        edges.Add(e);
                        vertexes.Add(e.A);
                        vertexes.Add(e.B);
                        if (vertexes.Intersect(ports).Count() > 0 && vertexes.Contains(vertex))
                            break;
                    }
                    if (edges is not null && edges.Count > 0)
                        lineEdges.Add(edges);
                }

                // 按最短出口生成
                {
                    // var queue = new PriorityQueue<(Vertex vertex, List<Edge> edges), double>();
                    // var visitedEdges = new HashSet<Edge>();
                    // foreach (var edge in vertex.Adjacencies.Where(e => _edges.Contains(e)))
                    // {
                    //     queue.Enqueue((edge.GetOtherEnd(vertex)!, new List<Edge>(new[] { edge })), edge.Length);
                    //     visitedEdges.Add(edge);
                    // }
                    // while (queue.Count > 0)
                    // {
                    //     queue.TryDequeue(out var temp, out var length);
                    //     var (nextVertex, edges) = temp;
                    //     if (ports.Contains(nextVertex))
                    //     {
                    //         lineEdges.Add(edges);
                    //         break;
                    //     }
                    //     else
                    //     {
                    //         var a = nextVertex.Adjacencies;
                    //         var b = nextVertex.Adjacencies.Where(e => _edges.Contains(e)).ToList();
                    //         foreach (var edge in nextVertex.Adjacencies.Where(e => _edges.Contains(e)))
                    //         {
                    //             if (visitedEdges.Contains(edge))
                    //                 continue;
                    //             var newEdges = new List<Edge>(edges);
                    //             newEdges.Add(edge);
                    //             queue.Enqueue((edge.GetOtherEnd(nextVertex)!, newEdges), length + edge.Length);
                    //             visitedEdges.Add(edge);
                    //         }
                    //     }
                    // }
                }
            }
            if (lineEdges.Count > 0)
            {
                bool include(List<Edge> t, List<Edge> o)
                {
                    if (t.Count < o.Count)
                        return false;
                    for (int i = 0; i < t.Count; i++)
                    {
                        if (t[i] != o[0])
                            continue;
                        for (int j = 0; j < o.Count && i + j < t.Count; j++)
                        {
                            if (t[i + j] != o[j])
                                break;
                            if (j == o.Count - 1)
                                return true;
                        }
                    }
                    return false;
                }
                lineEdges.Sort((a, b) => b.Count - a.Count);
                for (int i = 0; i < lineEdges.Count; i++)
                {
                    for (int j = i + 1; j < lineEdges.Count; j++)
                    {
                        var a = lineEdges[i];
                        var b = lineEdges[j];
                        var c = lineEdges[j].ToList();
                        c.Reverse();
                        if (include(a, b) || include(a, c))
                        {
                            lineEdges.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }
            foreach (var edges in lineEdges)
            {
                var l = new TrainLine(TrainLineLevel.FootPath);
                l.AddEdgeRange(edges);
            }
        }
    }
}