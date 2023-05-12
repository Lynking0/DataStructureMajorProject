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
    public class TrainLine
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
            Path.Curve = Path.Curve.Concat(_edges.Select(e => e.Curve).ToArray());
        }
        public static List<Trip> Navigate(IReadOnlyList<Vertex> vertexes)
        {
            var result = new List<Trip>();
            var lines = vertexes[0].GetTrainLines().ToList();
            var t = new Trip() { Start = vertexes[0] };
            for (int i = 0; i < vertexes.Count(); i++)
            {
                var v = vertexes[i];
                if (lines.Count == 0)
                {
                    // 当前线路无法达到该点，开始下一段trip
                    result.Add(t);
                    lines = v.GetTrainLines().ToList();
                    t = new Trip() { Start = v };
                }
                else
                {
                    t.End = v;
                    t.Line = lines.First();

                    lines = lines.Intersect(v.GetTrainLines()).ToList();

                }
            }
            result.Add(t);
            return result;
        }
    }
}