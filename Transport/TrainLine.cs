using Godot;
using GraphMoudle;
using System.Collections.Generic;
namespace TransportMoudle
{
    class TrainLine
    {

        private static List<TrainLine> _trainLines = new List<TrainLine>();
        public static IReadOnlyList<TrainLine> TrainLines => _trainLines;

        private List<Edge> _edges = new List<Edge>();
        public IReadOnlyList<Edge> Edges => _edges;

        public Color Color = new Color(GD.Randf() / 2, GD.Randf() / 2, GD.Randf() / 2, 1);

        public TrainLine()
        {
            _trainLines.Add(this);
        }

        public void AddEdge(Edge edge)
        {
            _edges.Add(edge);
        }
        public void AddEdgeRange(IEnumerable<Edge> edges)
        {
            _edges.AddRange(edges);
        }
    }
}