using System.Collections.Generic;
using System.Linq;
using GraphMoudle;
using IndustryMoudle.Extensions;
using TransportMoudle.Extensions;

namespace TransportMoudle
{
    public partial class TrainLine
    {
        public static List<Trip> Navigate(IList<Vertex> vertexes)
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
                    if (lines.Count() == 0)
                    {
                        // 遇到了被删掉的点
                        var reachableVertexes = vertexes.Take(i).ToHashSet();
                        var target = vertexes.Last();
                        var targetNearVertexes = new PriorityQueue<(IEnumerable<Vertex> path, Vertex vertex), float>();
                        foreach (var e in target.Adjacencies)
                        {
                            var otherEnd = e.GetOtherEnd(target)!;
                            targetNearVertexes.Enqueue((new Vertex[] { }, otherEnd), e.Length);
                        }
                        while (targetNearVertexes.Count > 0)
                        {
                            targetNearVertexes.TryDequeue(out var info, out var length);
                            if (info.vertex is null)
                                break;
                            if (reachableVertexes.Contains(info.vertex))
                            {
                                // 找到了一个可达的点
                                var newPath = new List<Vertex>();
                                foreach (var vertex in reachableVertexes)
                                {
                                    newPath.Add(vertex);
                                    if (vertex == info.vertex)
                                        break;
                                }
                                newPath.AddRange(info.path.Reverse());
                                return Navigate(newPath);
                            }
                            else
                            {
                                // 找到了一个不可达的点, 将该点的邻接点加入队列
                                foreach (var e in info.vertex.Adjacencies)
                                {
                                    var otherEnd = e.GetOtherEnd(info.vertex)!;
                                    targetNearVertexes.Enqueue(
                                        (info.path.Concat(new[] { info.vertex }), otherEnd),
                                        length + e.Length);
                                }
                            }
                        }
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