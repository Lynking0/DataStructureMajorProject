using Godot;
using System.Collections.Generic;
using System.Linq;
using GraphMoudle;
using TransportMoudle.Extensions;
using Shared.Extensions.Curve2DExtensions;

namespace TransportMoudle
{
    public enum TrainLineLevel
    {
        MainLine,
        SideLine,
        FootPath,
    }
    public partial class TrainLine
    {
        private static int MainLineIDCount = 0;
        private static int SideLineIDCount = 0;
        private static int FootPathIDCount = 0;
        public readonly string ID;
        public readonly TrainLineLevel Level;
        private static List<TrainLine> _trainLines = new List<TrainLine>();
        public static IReadOnlyList<TrainLine> TrainLines => _trainLines;

        private List<Edge> _edges = new List<Edge>();
        public IReadOnlyList<Edge> Edges => _edges;
        public Path2D Path = new Path2D();
        public IReadOnlyCollection<Vertex> Vertexes
        {
            get
            {
                var result = new HashSet<Vertex>();
                foreach (var edge in _edges)
                {
                    result.Add(edge.A);
                    result.Add(edge.B);
                }
                return result;
            }
        }
        public Dictionary<Vertex, float> Station = new Dictionary<Vertex, float>();
        public float TotalLength;
        public Color Color;
        public TrainLine(TrainLineLevel level)
        {
            Level = level;
            _trainLines.Add(this);
            Path.Curve = new Curve2D();
            switch (level)
            {
                case TrainLineLevel.MainLine:
                    ID = $"M{MainLineIDCount++}";
                    Color = new Color(GD.Randf() / 3, GD.Randf() / 3, GD.Randf() / 3, 1);
                    break;
                case TrainLineLevel.SideLine:
                    ID = $"S{SideLineIDCount++}";
                    Color = new Color(GD.Randf() / 3 + 1f / 3, GD.Randf() / 3 + 1f / 3, GD.Randf() / 3 + 1f / 3, 1);
                    break;
                case TrainLineLevel.FootPath:
                    ID = $"F{FootPathIDCount++}";
                    Color = new Color(GD.Randf() / 3 + 2f / 3, GD.Randf() / 3 + 2f / 3, GD.Randf() / 3 + 2f / 3, 1);
                    break;
                default:
                    throw new System.Exception("Unknown TrainLineLevel");
            }
        }
        public void AddEdge(Edge edge)
        {
            _edges.Add(edge);
        }
        public void AddEdgeRange(IEnumerable<Edge> edges)
        {
            _edges.AddRange(edges);
        }
        public void GenerateCurve()
        {
            var vertexes = _edges.SelectMany(e => new[] { e.A, e.B }).Distinct().ToList();
            var curves = _edges.Select(e => e.Curve).ToList();
            for (int i = 0; i < curves.Count; i++)
            {
                Station[vertexes[i]] = Path.Curve.GetBakedLength();
                Path.Curve = Path.Curve.Concat(curves[i]);
            }
            Station[vertexes.Last()] = Path.Curve.GetBakedLength();
            TotalLength = Path.Curve.GetBakedLength();
        }
    }
}