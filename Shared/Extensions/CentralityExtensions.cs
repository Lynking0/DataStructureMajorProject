using GraphMoudle;
using System.Collections.Generic;
using System;
using Godot;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;


namespace Shared.Extensions.CentralityExtensions
{
    public static class _CentralityExtensions
    {
        private static Vertex[] vs;
        private static int len;
        public static Dictionary<Vertex, float> dijkstra(Vertex s)
        {
            Vertex v = s, w;
            Dictionary<Vertex, float> dist = new Dictionary<Vertex, float>();
            Dictionary<Vertex, bool> visit = new Dictionary<Vertex, bool>();
            PriorityQueue<Vertex, float> heap = new PriorityQueue<Vertex, float>();
            foreach (Vertex vtemp in vs)
            {
                dist.Add(vtemp, float.MaxValue);
                visit.Add(vtemp, false);
            }
            dist[s] = 0; // 自己到自己为0
            heap.Enqueue(s, 0);
            while (heap.Count != 0)
            {
                v = heap.Peek();
                heap.Dequeue();
                if (visit[v]) continue;
                visit[v] = true;
                // 更新到每个点的权重
                foreach (Edge e in v.Adjacencies)
                {
                    w = e.GetOtherEnd(v);
                    if (visit[w]) continue;
                    if (dist[w] > dist[v] + e.Length)
                    {
                        dist[w] = dist[v] + e.Length;
                        heap.Enqueue(w, dist[w]);
                    }
                }
            }
            return dist;
        }
        // private static int sigmaST(int s, int t) // 计算s到t的最短路径数
        // {
        //     if (DV[s][t].Count == 0) return 1; // 回到起点
        //     int res = 0;
        //     for (int i = 0; i < DV[s][t].Count; i++)
        //     {
        //         res += sigmaST(s, DV[s][t][i]);
        //     }
        //     return res;
        // }
        // private static int sigmaSTV(int s, int t, int v, bool flag) // 计算s到t的且经过v的最短路径数
        // {
        //     if (DV[s][t].Count == 0)
        //     { // 回到起点
        //         if (flag == false) return 0; // 不经过C
        //         else return 1; // 经过c
        //     }
        //     int res = 0;
        //     for (int i = 0; i < DV[s][t].Count; i++)
        //     {
        //         if (DV[s][t][i] == v) res += sigmaSTV(s, DV[s][t][i], v, true);
        //         else res += sigmaSTV(s, DV[s][t][i], v, false);
        //     }
        //     return res;
        // }
        /* variables */

        /// <summary>
        ///   对于给定的图，计算所有节点的中心性，并存入各个Vertex对应属性
        /// </summary>
        private static void print(string s)
        {
            string result1 = @"result2.txt";
            FileStream fs = new FileStream(result1, FileMode.Append);
            StreamWriter wr = null;
            wr = new StreamWriter(fs);
            wr.WriteLine(s);
            wr.Close();
        }
        public static void PrecomputeCentrality(this Graph graph)
        {
            string result1 = @"result2.txt";
            FileStream fs = new FileStream(result1, FileMode.Create);
            StreamWriter wr = null;
            wr = new StreamWriter(fs);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();  // 开始计时
            /* code */
            vs = new Vertex[graph.Vertices.Count];
            len = vs.Length;
            int index = 0;
            foreach (Vertex v in graph.Vertices) // 节点副本
            {
                vs[index++] = v;
            }
            TimeSpan timespan = stopwatch.Elapsed;
            wr.WriteLine("初始化程序运行时间：" + timespan.TotalMilliseconds + "毫秒");
            wr.Close();
            ConcurrentDictionary<Vertex, Dictionary<Vertex, float>>? distInfo = new ConcurrentDictionary<Vertex, Dictionary<Vertex, float>>();
            Parallel.ForEach(vs,
                (Vertex s) =>
                {
                    distInfo.TryAdd(s, dijkstra(s));
                }
            );
            timespan = stopwatch.Elapsed;
            print(timespan.TotalMilliseconds + "毫秒");
            // for (int i = 0; i < len; i++)
            // {
            //     for (int j = 0; j < len; j++)
            //     {
            //         if (D[i, j] != float.PositiveInfinity) print(D[vs[i], vs[j]].ToString());
            //     }
            // }
            // int realI = 0;
            // foreach (Vertex vt in graph.Vertices)
            // {
            //     // 点度中心性
            //     vt.DegreeCentrality = vt.Adjacencies.Count / (len - 1);
            //     // 接近中心性
            //     float res = 0;
            //     for (int i = 0; i < len; i++)
            //     {
            //         res += D[realI, i];
            //     }
            //     vt.ClosenessCentrality = 1 / res;
            //     // 中介中心性
            //     res = 0;
            //     for (int s = 0; s < len; s++) // 起点
            //     {
            //         for (int t = 0; t < len; t++) // 终点
            //         {
            //             res += sigmaSTV(s, t, realI, false) / sigmaST(s, t);
            //         }
            //     }
            //     vt.BetweennessCentrality = res;
            //     realI++;
            // }
            // stopwatch.Stop();
        }
    }
}