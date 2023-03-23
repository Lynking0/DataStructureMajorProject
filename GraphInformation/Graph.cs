using System;
using Godot;
using GraphInformation.SpatialIndexer;
using GraphInformation.DoubleVector2Extensions;
using System.Collections.Generic;
using Topography;

namespace GraphInformation
{
    public partial class Graph
    {
        public static Graph Instance = new Graph();
        private VertexSpatialIndexer VerticesContainer;
        /// <summary>
        ///   包含图中所有节点的可迭代对象
        /// </summary>
        public IReadOnlyCollection<Vertex> vertices => VerticesContainer;

        public Graph()
        {
            VerticesContainer = new VertexSpatialIndexer();
        }
        public void PossionDiskSample()
        {
            VerticesContainer.Clear();
            List<Vector2D> options = new List<Vector2D>();
            List<Vector2D> selects = new List<Vector2D>();
            Vector2D start = new Vector2D(GD.RandRange(Graph.MinX, Graph.MaxX), GD.RandRange(Graph.MinY, Graph.MaxY));
            options.Add(start);
            selects.Add(start);
            VerticesContainer.Add(new Vertex(start));
            while (options.Count > 0)
            {
                int idx = GD.RandRange(0, options.Count - 1);
                Vector2D basicPos = options[idx];
                int cnt = 0;
                for (; cnt < Graph.MaxSampleTryTimes; ++cnt)
                {
                    double length = GD.RandRange(Graph.VerticesDistance, Graph.VerticesDistance * 2);
                    double angle = GD.RandRange(0, Math.Tau);
                    Vector2D extendedPos = new Vector2D(
                        basicPos.X + length * Mathf.Cos(angle), basicPos.Y + length * Mathf.Sin(angle));
                    if (!extendedPos.IsInRect(
                        Graph.MinX + Graph.VerticesDistance / 2,
                        Graph.MinY + Graph.VerticesDistance / 2,
                        Graph.MaxX - Graph.VerticesDistance / 2,
                        Graph.MaxY - Graph.VerticesDistance / 2))
                        continue;
                    if (VerticesContainer.HasAdjacency(extendedPos))
                        continue;
                    if (FractalNoiseGenerator.GetFractalNoise(extendedPos.X, extendedPos.Y) >= Graph.MaxVertexAltitude)
                        continue;
                    options.Add(extendedPos);
                    selects.Add(extendedPos);
                    VerticesContainer.Add(new Vertex(extendedPos));
                    break;
                }
                if (cnt == Graph.MaxSampleTryTimes)
                {
                    options[idx] = options[options.Count - 1];
                    options.RemoveAt(options.Count - 1);
                }
            }
            foreach (Vertex vertex in VerticesContainer)
            {
                double gradX, gradY;
                FractalNoiseGenerator.GetFractalNoise(vertex.Position.X, vertex.Position.Y, out gradX, out gradY);
                vertex.Gradient = new Vector2D(gradX, gradY);
            }
        }
    }
}