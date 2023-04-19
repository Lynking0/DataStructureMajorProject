using Godot;
using System.Linq;
using IndustryMoudle;

namespace Director
{
    public partial class MapRender : Node2D
    {
        private FontVariation? Font;

        public override void _Ready()
        {
            Font = new FontVariation();
            Font.BaseFont = ResourceLoader.Load<Font>("res://Render/PingFang-SC-Regular.ttf");
            GetNode<MapController>("../GameViewportContainer").MapChanged += Update;
            Update(Vector2.Zero, 1);
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
            DrawCircle(factory.Position, 6, Colors.WebGray);
            DrawString(Font, factory.Position, factory.Recipe, fontSize: 12);
        }
        private void DrawLink(ProduceLink link)
        {
            DrawArrow(link.from.Position, link.to.Position, Colors.WebGray, width: 2);
            DrawString(Font, (link.from.Position + link.to.Position) / 2, link, fontSize: 10, modulate: Colors.Red);
        }

        public override void _Draw()
        {
            Factory.Factories.ToList().ForEach(DrawFactor);
            ProduceLink.Links.ForEach(DrawLink);
        }
    }
}