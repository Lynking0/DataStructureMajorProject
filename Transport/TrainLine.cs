using Godot;
using GraphMoudle;
using System.Collections.Generic;
using System.Linq;
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
            // Path.Curve.AddPoint(edge.Curve.GetPointPosition(0));
            // Path.Curve.AddPoint(edge.Curve.GetPointPosition(1));

            // if (Path.Curve.PointCount == 0)
            //     Path.Curve = edge.Curve;
            // else
            Path.Curve = Path.Curve.Concat(edge.Curve);
        }
        public void AddEdgeRange(IEnumerable<Edge> edges)
        {
            foreach (var edge in edges)
            {
                AddEdge(edge);
            }
            // _edges.AddRange(edges);
            // foreach (var edge in edges)
            // {
            //     Path.Curve = Path.Curve.Concat(edge.Curve);
            // }
        }
    }
}