using Godot;
using System.Collections.Generic;
using System.Linq;
using GraphMoudle;

namespace TransportMoudle
{
    public class TrafficFreeBlock
    {
        private List<Edge> _edges;
        public IReadOnlyList<Edge> Edges => _edges;
        public Dictionary<Vertex, TrainLine> Transfer;

        public TrafficFreeBlock(IEnumerable<Edge> edges)
        {
            _edges = new List<Edge>(edges);
            Transfer = new Dictionary<Vertex, TrainLine>();
        }
    }
}