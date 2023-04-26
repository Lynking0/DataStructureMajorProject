using Godot;
using System.Collections.Generic;
using IndustryMoudle;
using IndustryMoudle.Link;

namespace DirectorMoudle
{
    public partial class FactoryRender : Node2D
    {
        private Factory? Factory;
        private FontVariation? Font;

        private List<ProduceLink> Links = new List<ProduceLink>();

        private void DrawArrow(Vector2 from, Vector2 to, Color color, float width = -1, bool antialiased = false)
        {
            DrawLine(from, to, color, width, antialiased);
            var arrow = to - from;
            var arrowLength = arrow.Length();
            var arrowHead = arrow.Normalized() * 10;
            var arrowLeft = arrowHead.Rotated(Mathf.Pi / 6);
            var arrowRight = arrowHead.Rotated(-Mathf.Pi / 6);
            var center = (from + to) / 2;
            DrawLine(center, center - arrowLeft, color, width, antialiased);
            DrawLine(center, center - arrowRight, color, width, antialiased);
        }

        public override void _Ready()
        {
            Font = new FontVariation();
            Font.BaseFont = ResourceLoader.Load<Font>("res://Render/PingFang-SC-Regular.ttf");
        }

        private void DrawLink(ProduceLink link)
        {
            var from = Vector2.Zero;
            var to = link.To.Position - link.From.Position;
            DrawArrow(from, to, link.Chain.Color, width: 2);
            DrawString(Font, (from + to) / 2, link, fontSize: 10, modulate: Colors.Red);
        }

        public void Refresh(Factory factory)
        {
            Factory = factory;
            QueueRedraw();
        }

        public void AddLink(ProduceLink link)
        {
            Links.Add(link);
            QueueRedraw();
        }

        public override void _Draw()
        {
            if (Factory is null)
            {
                return;
            }
            Position = Factory.Position;
            DrawCircle(Vector2.Zero, 6, Colors.WebGray);
            DrawString(Font, Vector2.Zero, Factory.Recipe, fontSize: 12);

            Links.ForEach(DrawLink);
        }
    }
}
