using System;
using Shared.Extensions.DoubleVector2Extensions;

namespace GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTreeStructure
{
    /// <summary>
    ///   存入RTree的数据
    /// </summary>
    public interface IRTreeData : IShape
    {
        /// <summary>
        ///   判断该几何物体是否与另一几何物体碰撞
        /// </summary>
        public bool IsOverlap(IRTreeData other);
    }
}