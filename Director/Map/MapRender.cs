using Godot;
using System.Collections.Generic;
using System.Linq;
using IndustryMoudle;

namespace DirectorMoudle
{
    public partial class MapRender : Node2D
    {
        private FontVariation? Font;

        private Node2D? FactoryContainer;
        private Dictionary<Factory, FactoryRender> FactoryToRender = new Dictionary<Factory, FactoryRender>();

        private int LogicFrameCount = 0;

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

        private void DrawArrow(Vector2 from, Vector2 to, Color color, float width = -1, bool antialiased = false)
        {
            DrawLine(from, to, color, width, antialiased);
            var arrow = to - from;
            var arrowLength = arrow.Length();
            var arrowHead = arrow.Normalized() * 10;
            var arrowLeft = arrowHead.Rotated(Mathf.Pi / 6);
            var arrowRight = arrowHead.Rotated(-Mathf.Pi / 6);
            DrawLine(to, to - arrowLeft, color, width, antialiased);
            DrawLine(to, to - arrowRight, color, width, antialiased);
        }
        private void DrawFactor(Factory factory)
        {
            var render = (FactoryRender)GD.Load<PackedScene>("res://Director/Map/FactoryRender/FactoryRender.tscn").Instantiate();
            FactoryToRender[factory] = render;
            FactoryContainer?.AddChild(render);
            render.Refresh(factory);
        }
        private void DrawLink(ProduceLink link)
        {
            FactoryToRender[link.from].AddLink(link);
        }

        public override void _Draw()
        {
            Factory.Factories.ToList().ForEach(DrawFactor);
            ProduceLink.Links.ForEach(DrawLink);
        }
    }
}