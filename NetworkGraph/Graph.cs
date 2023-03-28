using System;
using Godot;
using NetworkGraph.SpatialIndexer;
using Shared.Extensions.DoubleVector2Extensions;
using System.Collections.Generic;
using Topography;
using NetworkGraph.DataStructureAndAlgorithm.DisjointSet;
using NetworkGraph.DataStructureAndAlgorithm.OptimalCombinationAlgorithm;
using static Shared.RandomMethods;

namespace NetworkGraph
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
            foreach ((Vertex a, Vertex b) in pairs)
            {
                Edge edge = new Edge(a, b, new Curve2D());
                EdgeEvaluator.Instance.MaxEnergy = Graph.MaxVertexAltitude;
                EdgeEvaluator.Instance.A = a.Position;
                EdgeEvaluator.Instance.D = b.Position;
                if (Mathf.Abs(a.Gradient.OrthogonalD().AngleToD(b.Position - a.Position)) < Math.PI / 2)
                {
                    EdgeEvaluator.Instance.B = a.Position + a.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance;
                    edge.Curve.AddPoint((Vector2)a.Position,
                        @out: (Vector2)(a.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance));
                }
                else
                {
                    EdgeEvaluator.Instance.B = a.Position - a.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance;
                    edge.Curve.AddPoint((Vector2)a.Position,
                        @out: -(Vector2)(a.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance));
                }
                if (Mathf.Abs(b.Gradient.OrthogonalD().AngleToD(a.Position - b.Position)) < Math.PI / 2)
                {
                    EdgeEvaluator.Instance.C = b.Position + b.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance;
                    edge.Curve.AddPoint((Vector2)b.Position,
                        @in: (Vector2)(b.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance));
                }
                else
                {
                    EdgeEvaluator.Instance.C = b.Position - b.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance;
                    edge.Curve.AddPoint((Vector2)b.Position,
                        @in: -(Vector2)(b.Gradient.OrthogonalD().NormalizedD() * Graph.CtrlPointDistance));
                }
                // GD.Print(EdgeEvaluator.Instance.Annealing());
                // GD.Print(EdgeEvaluator.Instance.Annealing());
                // GD.Print(EdgeEvaluator.Instance.Annealing());
                // GD.Print();

                double v = EdgeEvaluator.Instance.Annealing().energy;
                if (v < EdgeEvaluator.Instance.MaxEnergy)
                {
                    a.Adjacencies.Add(edge);
                    b.Adjacencies.Add(edge);
                }
            }
            foreach (Vertex v in Vertices)
                v.Type = v.Adjacencies.Count == 0 ? Vertex.VertexType.Terminal : Vertex.VertexType.Intermediate;
        }
    }
}