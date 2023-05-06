using System.Collections.Generic;
using System.Linq;
using GraphMoudle;

namespace TransportMoudle.Extensions
{
    public static class _Vertex
    {
        /// <summary>
        /// 获取所有经过该点的线路
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public static IEnumerable<TrainLine> GetTrainLines(this Vertex vertex)
        {
            return TrainLine.TrainLines
                    .Where(line => line.Edges.Any(edge => edge.A == vertex || edge.B == vertex));
        }
    }

    public static class _Edge
    {
        /// <summary>
        /// 获取所有经过该边的线路
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public static IEnumerable<TrainLine> GetTrainLines(this Edge edge)
        {
            return TrainLine.TrainLines
                    .Where(line => line.Edges.Contains(edge));
        }
    }
}