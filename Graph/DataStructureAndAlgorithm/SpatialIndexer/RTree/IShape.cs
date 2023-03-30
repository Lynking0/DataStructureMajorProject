using System;
using Shared.Extensions.DoubleVector2Extensions;

namespace GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTree
{
    /// <summary>
    ///   碰撞体
    /// </summary>
    public interface IShape
    {
        /// <summary>
        ///   最小限定矩形MBR(minimum bounding retangle)
        /// </summary>
        public RTRect2 Rectangle { get; }
    }
}