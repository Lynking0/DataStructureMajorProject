using System;
using Shared.Extensions.DoubleVector2Extensions;

namespace GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTree
{
    /// <summary>
    ///   仅供RTree及其相关函数使用的Rect2
    /// </summary>
    public struct RTRect2
    {
        /// <summary>
        ///   Top Left
        /// </summary>
        public readonly Vector2D TL;
        /// <summary>
        ///   Bottom Right
        /// </summary>
        public readonly Vector2D BR;
        public readonly double Size;
        public RTRect2(Vector2D tl, Vector2D br)
        {
            TL = tl;
            BR = br;
            Size = (BR.X - TL.X) * (BR.Y - TL.Y);
        }
    }
}