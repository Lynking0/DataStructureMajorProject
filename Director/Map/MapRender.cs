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

        public override void _Ready()
        {
            Font = new FontVariation();
            Font.BaseFont = ResourceLoader.Load<Font>("res://Render/PingFang-SC-Regular.ttf");
            GetNode<MapController>("../GameViewportContainer").MapChanged += Update;
            GetNode<Director>("../../Director").Tick += () => { GetNode<Label>("../LogicFrame").Text = "LogicFrame: " + (LogicFrameCount++).ToString(); };
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
        private void DrawLink(ProduceLink link)
        {
            FactoryToRender[link.From].AddLink(link);
        }

        private void DrawRoad(Edge edge)
        {
            VertexToRender[edge.A].AddRoad(edge);
        }

        public override void _Draw()
        {
            foreach (var render in Renders)
            {
                render.Clear();
            }
            Factory.Factories.ToList().ForEach(DrawFactor);
            ProduceLink.Links.ForEach(DrawLink);
            Graph.Instance.Edges.ToList().ForEach(DrawRoad);
            foreach (var render in Renders)
            {
                render.QueueRedraw();
            }
        }
    }
}