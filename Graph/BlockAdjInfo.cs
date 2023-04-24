using System;

namespace GraphMoudle
{
    public struct BlockAdjInfo : IComparable<BlockAdjInfo>
    {
        /// <summary>
        ///   邻近的区块
        /// </summary>
        Block AdjBlock;
        /// <summary>
        ///   属于当前区块，和Vertex2一起构成两区块最近的Vertex对
        /// </summary>
        Vertex Vertex1;
        /// <summary>
        ///   属于邻近区块，和Vertex1一起构成两区块最近的Vertex对
        /// </summary>
        Vertex Vertex2;
        /// <summary>
        ///   两区块最近的Vertex对的距离
        /// </summary>
        double Distance;
        public BlockAdjInfo(Block adjBlock, Vertex vertex1, Vertex vertex2, double distance)
        {
            AdjBlock = adjBlock;
            Vertex1 = vertex1;
            Vertex2 = vertex2;
            Distance = distance;
        }
        public int CompareTo(BlockAdjInfo other) { return Math.Sign(Distance - other.Distance); }
    }
}