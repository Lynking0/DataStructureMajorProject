using System;
using Godot;

namespace GraphMoudle
{
    public partial class Graph
    {
        public const double MinX = -5000;
        public const double MaxX = 5000;
        public const double MinY = -3000;
        public const double MaxY = 3000;
        public const double CtrlPointDistance = 24;
        public const double VerticesDistance = 57;
        /// <summary>
        ///   碰撞检测时的最小距离，需保证小于VerticesDistance
        /// </summary>
        public const double EdgesDistance = 10;
        /// <summary>
        ///   当两边共顶点时碰撞检测的忽略范围
        /// </summary>
        public const double EdgeIgnoreRatio = 0.15;
        public const double MaxVertexAltitude = 0.1;
        public const double MaxSampleTryTimes = 30;
    }
}