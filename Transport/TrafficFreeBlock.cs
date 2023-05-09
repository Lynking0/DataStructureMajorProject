using Godot;
using System.Collections.Generic;
using System.Linq;
using GraphMoudle;

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
    }
}