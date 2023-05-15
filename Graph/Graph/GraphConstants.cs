using System;
using Godot;

namespace GraphMoudle
{
    public partial class Graph
    {
        public const double MinX = -DirectorMoudle.Constants.Width / 2;
        public const double MaxX = DirectorMoudle.Constants.Width / 2;
        public const double MinY = -DirectorMoudle.Constants.Height / 2;
        public const double MaxY = DirectorMoudle.Constants.Height / 2;
        public const double CtrlPointDistance = 30 * DirectorMoudle.Constants.Magnification;
        public const double VerticesDistance = 57 * DirectorMoudle.Constants.Magnification;
        /// <summary>
        ///   碰撞检测时的最小距离，需保证小于VerticesDistance
        /// </summary>
        public const double EdgesDistance = 10 * DirectorMoudle.Constants.Magnification;
        public const double EdgesDistanceSquared = EdgesDistance * EdgesDistance;
        /// <summary>
        ///   当两边共顶点时碰撞检测的忽略范围
        /// </summary>
        public const double EdgeIgnoreDist = 25 * DirectorMoudle.Constants.Magnification;
        public const double MaxVertexAltitude = 0.1;
        public const double MaxSampleTryTimes = 30;
        public const int MinBlockVerticesCount = 5;
    }
}