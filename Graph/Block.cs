using System;
using System.Collections.Generic;

namespace GraphMoudle
{
    public class Block
    {
        public int Count => Vertices.Count;
        public int Index;
        public List<Vertex> Vertices;
        public BlockAdjInfo[]? AdjacenciesInfo;
        public Block()
        {
            Vertices = new List<Vertex>();
        }
        public Block(params Vertex[] vertices)
        {
            Vertices = new List<Vertex>(vertices);
        }
        public Block(IEnumerable<Vertex> vertices)
        {
            Vertices = new List<Vertex>(vertices);
        }
        public void SetAdjacenciesInfo(ICollection<KeyValuePair<Block, (double, Vertex, Vertex)>> info)
        {
            AdjacenciesInfo = new BlockAdjInfo[info.Count];
            int idx = 0;
            foreach ((Block block, (double distSquared, Vertex vertex1, Vertex vertex2)) in info)
                AdjacenciesInfo[idx] = new BlockAdjInfo(block, vertex1, vertex2, Math.Sqrt(distSquared));
            Array.Sort(AdjacenciesInfo);
        }
    }
}