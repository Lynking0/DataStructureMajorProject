using System;

namespace GraphMoudle
{
    public struct BlockAdjInfo : IComparable<BlockAdjInfo>
    {
        /// <summary>
        ///   邻近的区块
        /// </summary>
        public Block AdjBlock;
        /// <summary>
        ///   属于当前区块，和Vertex2一起构成两区块最近的Vertex对
        /// </summary>
        public Vertex Vertex1;
        /// <summary>
        ///   属于邻近区块，和Vertex1一起构成两区块最近的Vertex对
        /// </summary>
        public Vertex Vertex2;
        /// <summary>
        ///   两区块最近的Vertex对的距离
        /// </summary>
        public double Distance;
        public BlockAdjInfo(Block adjBlock, Vertex vertex1, Vertex vertex2, double distance)
        {
            AdjBlock = adjBlock;
            Vertex1 = vertex1;
            Vertex2 = vertex2;
            Distance = distance;
        }
        public int CompareTo(BlockAdjInfo other) { return Math.Sign(Distance - other.Distance); }
        public void Deconstruct(out Block adjBlock, out Vertex vertex1, out Vertex vertex2, out double dist)
        {
            adjBlock = AdjBlock;
            vertex1 = Vertex1;
            vertex2 = Vertex2;
            dist = Distance;
        }
    }
}