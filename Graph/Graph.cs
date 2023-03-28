using System;
using Godot;
using GraphMoudle.SpatialIndexer;
using Shared.Extensions.DoubleVector2Extensions;
using System.Collections.Generic;
using TopographyMoudle;
using GraphMoudle.DataStructureAndAlgorithm.DisjointSet;
using GraphMoudle.DataStructureAndAlgorithm.OptimalCombinationAlgorithm;
using static Shared.RandomMethods;
using GraphMoudle.DataStructureAndAlgorithm.OptimalCombinationAlgorithm.ComputeShader;

namespace GraphMoudle
{
    public partial class Graph
    {
        public static Graph Instance = new Graph();
        private VertexSpatialIndexer VerticesContainer;
        public IReadOnlyCollection<Vertex> Vertices => VerticesContainer;
        public IEnumerable<Edge> Edges
        {
            get
            {
                foreach (Vertex v in Vertices)
                    foreach (Edge e in v.Adjacencies)
                        if (e.A == v)
                            yield return e;
            }
        }

        public Graph()
        {
            VerticesContainer = new VertexSpatialIndexer();
        }
        /// <summary>
        ///   生成各个点
        /// </summary>
        public void CreateVertices()
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
                        Graph.MinX + Graph.CtrlPointDistance,
                        Graph.MinY + Graph.CtrlPointDistance,
                        Graph.MaxX - Graph.CtrlPointDistance,
                        Graph.MaxY - Graph.CtrlPointDistance))
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
        public void CreateEdges()
        {
            // UnionFindDisjointSet<Vertex> unionFindSet = new UnionFindDisjointSet<Vertex>(Vertices);
            List<(Vertex, Vertex)> pairs = VerticesContainer.GetNearbyPairs();
            RandomDislocate(pairs);
            FirstCreation(pairs);
        }
        /// <summary>
        ///   首次尝试建边，将所有节点当作Intermediate看待并尝试生成Edge，并将最后未建成边的节点标记为Terminal。
        /// </summary>
        private void FirstCreation(List<(Vertex, Vertex)> pairs)
        {
            EdgeEvaluatorInvoker.Init();
            EdgeEvaluatorInvoker.Data = new List<(Vector2D a, Vector2D b, Vector2D c, Vector2D d)>();
            foreach ((Vertex a, Vertex b) in pairs)
            {
                Vector2D aCtrl, bCtrl;
                if (Mathf.Abs(a.Gradient.OrthogonalD().AngleToD(b.Position - a.Position)) < Math.PI / 2)
                    aCtrl = a.Position + a.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance;
                else
                    aCtrl = a.Position - a.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance;
                if (Mathf.Abs(b.Gradient.OrthogonalD().AngleToD(a.Position - b.Position)) < Math.PI / 2)
                    bCtrl = b.Position + b.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance;
                else
                    bCtrl = b.Position - b.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance;
                EdgeEvaluatorInvoker.Data.Add((a.Position, aCtrl, bCtrl, b.Position));
            }
            EdgeEvaluatorInvoker.Invoke();
            bool[] isVaild = EdgeEvaluatorInvoker.Receive();
            // foreach (Vertex v in Vertices)
            //     v.Type = v.Adjacencies.Count == 0 ? Vertex.VertexType.Terminal : Vertex.VertexType.Intermediate;
        }
    }
}