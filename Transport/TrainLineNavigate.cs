using System.Collections.Generic;
using System.Linq;
using GraphMoudle;
using TransportMoudle.Extensions;

namespace TransportMoudle
{
    public partial class TrainLine
    {
        public static List<Trip> Navigate(IReadOnlyList<Vertex> vertexes)
        {
            /// TODO:
            /// 传进的vertexes是之前生产link的时候生成的点集
            /// 有些点已经被删掉了的
            /// 大体可以借助之前的信息导航
            /// 但是如果有被删掉的点
            /// 则从这点开始计算后面的路径
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