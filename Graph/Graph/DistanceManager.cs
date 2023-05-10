using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace GraphMoudle
{
    public partial class Graph
    {
        private void CalcDistInfo()
        {
            Dictionary<Block, (double, Vertex, Vertex)>[] blockAdjInfo = new Dictionary<Block, (double, Vertex, Vertex)>[Blocks.Count];
            for (int i = 0; i < blockAdjInfo.Length; ++i)
                blockAdjInfo[i] = new Dictionary<Block, (double, Vertex, Vertex)>();
            Vertex[] vertices = new Vertex[Vertices.Count];
            VerticesContainer.CopyTo(vertices, 0);
            Mutex mut = new Mutex();
            Parallel.ForEach(vertices,
                (Vertex vertex) =>
                {
                    double key = System.Double.MaxValue;
                    Vertex? item = null;
                    foreach (Vertex vertex_ in vertices)
                    {
                        if (vertex.ParentBlock == vertex_.ParentBlock)
                            continue;
                        double distS = vertex.Position.DistanceSquaredToD(vertex_.Position);
                        if (distS < key)
                        {
                            key = distS;
                            item = vertex_;
                        }
                    }
                    (double, Vertex, Vertex) value = (key, vertex, item!);
                    (double, Vertex, Vertex) oldValue;
                    mut.WaitOne();
                    if (!blockAdjInfo[vertex.ParentBlock.Index].TryGetValue(item!.ParentBlock, out oldValue))
                        blockAdjInfo[vertex.ParentBlock.Index].Add(item.ParentBlock, value);
                    else if (key < oldValue.Item1)
                        blockAdjInfo[vertex.ParentBlock.Index][item.ParentBlock] = value;
                    mut.ReleaseMutex();
                }
            );
            for (int i = 0; i < Blocks.Count; ++i)
                Blocks[i].SetAdjacenciesInfo(blockAdjInfo[i]);
        }
    }
}