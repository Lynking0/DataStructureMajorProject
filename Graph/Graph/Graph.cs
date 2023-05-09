using System;
using Godot;
using GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer;
using GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTreeStructure;
using Shared.Extensions.ICollectionExtensions;
using Shared.Extensions.DoubleVector2Extensions;
using System.Collections.Generic;
using TopographyMoudle;
using GraphMoudle.DataStructureAndAlgorithm.OptimalCombinationAlgorithm;
using GraphMoudle.DataStructureAndAlgorithm.OptimalCombinationAlgorithm.ComputeShader;
using static Shared.RandomMethods;

namespace GraphMoudle
{
    public partial class Graph
    {
        public static Graph Instance = new Graph();
        private VertexSpatialIndexer VerticesContainer;
        public RTree GISInfoStorer;
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
        public List<Block> Blocks;

        public Graph()
        {
            VerticesContainer = new VertexSpatialIndexer();
            GISInfoStorer = new RTree(5);
            Blocks = new List<Block>();
        }
        /// <summary>
        ///   获取到指定节点vertex的距离最近的点
        /// </summary>
        public Vertex GetNearestVertex(Vertex vertex)
        {
            return DistInfo![vertex][0];
        }
        /// <summary>
        ///   获取一个节点数组，数组中的节点按到指定节点vertex的距离由近到远排列（vertex不在数组中）
        /// </summary>
        public Vertex[] GetNearVertices(Vertex vertex)
        {
            return DistInfo![vertex];
        }
        public void CreateVertices()
        {
            VerticesContainer.Clear();

            // 泊松圆盘采样
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
                    Vector2D expandedPos = new Vector2D(
                        basicPos.X + length * Mathf.Cos(angle), basicPos.Y + length * Mathf.Sin(angle));
                    if (!expandedPos.IsInRect(
                        Graph.MinX + Graph.CtrlPointDistance,
                        Graph.MinY + Graph.CtrlPointDistance,
                        Graph.MaxX - Graph.CtrlPointDistance,
                        Graph.MaxY - Graph.CtrlPointDistance))
                        continue;
                    if (VerticesContainer.HasAdjacency(expandedPos))
                        continue;
                    if (FractalNoiseGenerator.GetFractalNoise(expandedPos.X, expandedPos.Y) >= Graph.MaxVertexAltitude)
                        continue;
                    options.Add(expandedPos);
                    selects.Add(expandedPos);
                    VerticesContainer.Add(new Vertex(expandedPos));
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
            ulong t = Time.GetTicksUsec();
            // 初始化标记
            foreach (Vertex vertex in Vertices)
                vertex.Type = Vertex.VertexType.Isolated;

            // 找出邻近点对
            List<(Vertex, Vertex)> pairs = VerticesContainer.GetNearbyPairs();

            // 通过海拔高度初步筛选出可选边
            List<Edge> alternativeEdges = new List<Edge>();
            EdgeEvaluatorInvoker.Init();
            FirstTimeFilter(pairs, alternativeEdges);
            SecondTimeFilter(pairs, alternativeEdges);

            // 初始化RTree，并加入各个Vertex
            GISInfoStorer.Clear();
            foreach (Vertex vertex in Vertices)
                if (vertex.Type != Vertex.VertexType.Isolated) // 已经确认没有连边的点不需要参与后面的运算
                    GISInfoStorer.Add(vertex); // 此时不存在vertex无法添加的可能性，故不调用CanAdd()函数

            // 从初步筛出的边中选择出最终要生成的边
            BuildEdges(alternativeEdges);

            // 此时边已初步生成，删除此时还没有连边的点
            foreach (Vertex vertex in VerticesContainer)
            {
                if (vertex.Type != Vertex.VertexType.Isolated) // 删除R树中所有的vertex
                    GISInfoStorer.Remove(vertex);
                if (vertex.Adjacencies.Count == 0)
                    VerticesContainer.Remove(vertex);
            }

            // 生成分块信息
            DivideBlocks();

            // 预计算并存储距离信息
            CalcDistInfo();

            // 生成桥
            CreateBridges();

            AdjustEdges();
            GD.Print(Time.GetTicksUsec() - t);
        }
        public void AdjustEdges()
        {
            foreach (Vertex vertex in Vertices)
            {
                if (vertex.Type != Vertex.VertexType.Intermediate || vertex.Adjacencies.Count < 2)
                    continue;
                bool flag = true;
                foreach ((Edge a, Edge b) in vertex.Adjacencies.ToPairs())
                {
                    if (!Mathf.IsEqualApprox((float)a.GetCtrlAngle(vertex)!, (float)b.GetCtrlAngle(vertex)!))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    Edge[] adjEdges = new Edge[vertex.Adjacencies.Count];
                    vertex.Adjacencies.CopyTo(adjEdges);
                    RandomDislocate(adjEdges);
                    foreach (Edge edge in adjEdges)
                    {
                        if (edge.IsBridge)
                            continue;
                        Curve2D temp = edge.GetCurveCopy();
                        edge.RotateCtrlPoint(vertex, MathF.PI);
                        // edge不是桥，所以只有一段三阶贝塞尔
                        EdgeEvaluator.Instance.MaxEnergy = MaxVertexAltitude;
                        EdgeEvaluator.Instance.A = edge.Curve.GetPointPosition(0);
                        EdgeEvaluator.Instance.B = edge.Curve.GetPointPosition(0) + edge.Curve.GetPointOut(0);
                        EdgeEvaluator.Instance.C = edge.Curve.GetPointPosition(1) + edge.Curve.GetPointIn(1);
                        EdgeEvaluator.Instance.D = edge.Curve.GetPointPosition(1);
                        if (EdgeEvaluator.Instance.Annealing().energy >= MaxVertexAltitude)
                        {
                            edge.SetCurveCopy(temp);
                            continue;
                        }
                        GISInfoStorer.Remove(edge);
                        if (!GISInfoStorer.CanAdd(edge))
                        {
                            edge.SetCurveCopy(temp);
                            GISInfoStorer.Add(edge);
                        }
                        else
                        {
                            GISInfoStorer.Add(edge);
                            break;
                        }
                    }
                }
            }
        }
    }
}