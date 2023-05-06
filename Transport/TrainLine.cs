using Godot;
using GraphMoudle;
using System.Collections.Generic;
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

        public Color Color = new Color(GD.Randf() / 2, GD.Randf() / 2, GD.Randf() / 2, 1);

        public TrainLine(TrainLineLevel level)
        {
            Level = level;
            _trainLines.Add(this);
            switch (level)
            {
                case TrainLineLevel.MainLine:
                    ID = $"M{MainLineIDCount++}";
                    break;
                case TrainLineLevel.SideLine:
                    ID = $"S{SideLineIDCount++}";
                    break;
                case TrainLineLevel.FootPath:
                    ID = $"F{FootPathIDCount++}";
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
    }
}