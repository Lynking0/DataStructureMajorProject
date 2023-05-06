using Godot;
using System.Collections.Generic;
using System.Linq;
using IndustryMoudle;
using IndustryMoudle.Link;
using GraphMoudle;

namespace DirectorMoudle
{
    public partial class MapRender : Node2D
    {
        public static MapRender? Instance;

        private FontVariation? Font;

        private Node2D? FactoryContainer;
        private List<FactoryRender> Renders = new List<FactoryRender>();
        private Dictionary<Factory, FactoryRender> FactoryToRender = new Dictionary<Factory, FactoryRender>();
        private Dictionary<Vertex, FactoryRender> VertexToRender = new Dictionary<Vertex, FactoryRender>();

        public bool RoadDisplay { get; private set; } = true;
        public bool FactoryDisplay { get; private set; } = true;
        public bool LinkDisplay { get; private set; } = true;
        public bool TrainLine1Display { get; private set; } = true;
        public bool TrainLine2Display { get; private set; } = false;
        public bool TrainLine3Display { get; private set; } = false;

        private bool MaxEdgeLoadDirty = true;
        public int MaxEdgeLoad { get; private set; } = 0;
        public const int MaxRoadWidth = 6;

        private int LogicFrameCount = 0;

        public MapRender()
        {
            if (Instance is not null)
            {
                Logger.error("MapRender is a singleton class.");
                throw new System.Exception("MapRender is a singleton class.");
            }
            Instance = this;
        }
        private void FactoryRenderRedraw()
        {
            foreach (var render in Renders)
            {
                render.QueueRedraw();
            }
        }

        public void RoadDisplayChange(bool status)
        {
            RoadDisplay = status;
            FactoryRenderRedraw();
        }
        public void FactoryDisplayChange(bool status)
        {
            FactoryDisplay = status;
            FactoryRenderRedraw();
        }
        public void LinkDisplayChange(bool status)
        {
            LinkDisplay = status;
            FactoryRenderRedraw();
        }
        public void TrainLine1DisplayChange(bool status)
        {
            TrainLine1Display = status;
            FactoryRenderRedraw();
        }
        public void TrainLine2DisplayChange(bool status)
        {
            TrainLine2Display = status;
            FactoryRenderRedraw();
        }
        public void TrainLine3DisplayChange(bool status)
        {
            TrainLine3Display = status;
            FactoryRenderRedraw();
        }

        public override void _Ready()
        {
            Font = new FontVariation();
            Font.BaseFont = ResourceLoader.Load<Font>("res://Render/PingFang-SC-Regular.ttf");
            GetNode<MapController>("%GameViewportContainer").MapChanged += Update;
            GetNode<Director>("%Director").Tick += () => { GetNode<Label>("%LogicFrame").Text = "LogicFrame: " + (LogicFrameCount++).ToString(); };
            Update(Vector2.Zero, 1);

            FactoryContainer = new Node2D();
            FactoryContainer.Name = "FactoryContainer";
            AddChild(FactoryContainer);
        }

        private void Update(Vector2 transform, double scale)
        {
            Position = transform + GetWindow().Size / 2;
            Scale = new Vector2((float)scale, (float)scale);
        }

        private void DrawFactor(Factory factory)
        {
            var render = (FactoryRender)GD.Load<PackedScene>("res://Director/Map/FactoryRender/FactoryRender.tscn").Instantiate();
            Renders.Add(render);
            FactoryToRender[factory] = render;
            VertexToRender[factory.Vertex] = render;
            FactoryContainer?.AddChild(render);
            render.Refresh(factory);
        }

        public float GetRoadWidth(int load)
        {
            if (MaxEdgeLoadDirty)
            {
                var edgeLoad = new Dictionary<Edge, int>();
                foreach (var link in ProduceLink.Links)
                {
                    foreach (var (curEdge, _) in link.EdgeInfos)
                    {
                        if (curEdge is null)
                            continue;
                        if (!edgeLoad.ContainsKey(curEdge))
                        {
                            edgeLoad[curEdge] = 0;
                        }
                        edgeLoad[curEdge] += link.Item.Number;
                    }
                }
                MaxEdgeLoad = edgeLoad.Values.Max();
                MaxEdgeLoadDirty = false;
            }

            // by Exp
            if (load == 0)
                return 0;
            return (Mathf.Exp((float)load / MaxEdgeLoad) * MaxRoadWidth / Mathf.E);
        }

        private void AddLink(ProduceLink link)
        {
            FactoryToRender[link.From].AddLink(link);
            MaxEdgeLoadDirty = true;
        }

        private void AddRoad(Edge edge)
        {
            VertexToRender[edge.A].AddRoad(edge);
        }

        public override void _Draw()
        {
            foreach (var render in Renders)
            {
                render.Clear();
            }
            if (FactoryDisplay)
                Factory.Factories.ToList().ForEach(DrawFactor);
            if (RoadDisplay)
                Graph.Instance.Edges.ToList().ForEach(AddRoad);
            if (LinkDisplay)
                ProduceLink.Links.ForEach(AddLink);
            foreach (var render in Renders)
            {
                render.QueueRedraw();
            }
        }
    }
}