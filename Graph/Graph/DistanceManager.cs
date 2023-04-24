using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace GraphMoudle
{
    public partial class Graph
    {
        private ConcurrentDictionary<Vertex, Vertex[]>? DistInfo;
        private void CalcDistInfo()
        {
            DistInfo = new ConcurrentDictionary<Vertex, Vertex[]>();
            Vertex[] vertices = new Vertex[Vertices.Count];
            VerticesContainer.CopyTo(vertices, 0);
            Parallel.ForEach(vertices,
                (Vertex vertex) =>
                {
                    Vertex[] items = new Vertex[vertices.Length - 1];
                    double[] keys = new double[vertices.Length - 1];
                    int idx = 0;
                    foreach (Vertex vertex_ in vertices)
                    {
                        if (vertex == vertex_)
                            continue;
                        items[idx] = vertex_;
                        keys[idx] = vertex.Position.DistanceSquaredToD(vertex_.Position);
                        ++idx;
                    }
                    Array.Sort(keys, items);
                    DistInfo.TryAdd(vertex, items);
                }
            );
        }
        private Task CalcDistInfoAsync()
        {
            DistInfo = new ConcurrentDictionary<Vertex, Vertex[]>();
            Vertex[] vertices = new Vertex[Vertices.Count];
            VerticesContainer.CopyTo(vertices, 0);
            return Parallel.ForEachAsync(vertices,
                async (Vertex vertex, CancellationToken _) =>
                {
                    await Task.Run(
                        () =>
                        {
                            Vertex[] items = new Vertex[vertices.Length];
                            vertices.CopyTo(items, 0);
                            double[] keys = new double[vertices.Length];
                            for (int i = 0; i < items.Length; ++i)
                                keys[i] = vertex.Position.DistanceSquaredToD(items[i].Position);
                            Array.Sort(keys, items);
                            DistInfo.TryAdd(vertex, items);
                        }
                    );
                }
            );
        }
    }
}