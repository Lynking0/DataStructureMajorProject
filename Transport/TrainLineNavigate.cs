using System.Collections.Generic;
using System.Linq;
using GraphMoudle;
using IndustryMoudle.Extensions;
using TransportMoudle.Extensions;

namespace TransportMoudle
{
    public partial class TrainLine
    {
        public static List<Trip> Navigate(IReadOnlyList<Vertex> vertexes)
        {
            var result = new List<Trip>();
            var lines = vertexes[0].GetTrainLines();
            var t = new Trip() { Start = vertexes[0] };
            for (int i = 0; i < vertexes.Count(); i++)
            {
                var v = vertexes[i];
                lines = lines.Intersect(v.GetTrainLines());
                if (lines.Count() == 0)
                {
                    // 当前线路无法达到该点，开始下一段trip
                    result.Add(t);
                    lines = v.GetTrainLines().ToList();
                    if (lines.Count() == 0)
                    {
                        var a = vertexes.Select(v => v.GetFactory()!.ID);
                    }
                    t = new Trip() { Start = vertexes[i - 1], End = vertexes[i], Line = lines.First() };
                }
                else
                {
                    t.End = v;
                    t.Line = lines.First();
                }
            }
            result.Add(t);
            return result;
        }
    }
}