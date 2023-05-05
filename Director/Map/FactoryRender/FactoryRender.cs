using Godot;
using System.Collections.Generic;
using IndustryMoudle;
using IndustryMoudle.Link;
using GraphMoudle;

namespace DirectorMoudle
{
    public partial class FactoryRender : Node2D
    {
        private Factory? Factory;
        private FontVariation? Font;
        private Node2D? PathContainer;

        private List<ProduceLink> Links = new List<ProduceLink>();
        private List<Edge> Edges = new List<Edge>();

        public void Clear()
        {
            Factory = null;
            Links.Clear();
            Edges.Clear();
            foreach (var child in PathContainer!.GetChildren())
            {
                PathContainer.RemoveChild(child);
            }
        }

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
            PathContainer = GetNode<Node2D>("PathContainer");
            GetNode<Button>("Button").ButtonDown += () =>
            {
                GetNode<FactroyView>("/root/Main/MouseInput/FactroyView").Refresh(Factory!);
                GD.Print(Factory!.Recipe.DEBUG_OUTPUT());
            };
        }

        private void DrawLink(ProduceLink link)
        {
            var from = Vector2.Zero;
            var to = link.To.Position - link.From.Position;
            DrawArrow(from, to, link.Chain.Color, width: 2);
            DrawString(Font, (from + to) / 2, link, fontSize: 10, modulate: Colors.Red);
        }

        private void DrawRoad(Edge edge)
        {
            var path = new Path2D();
            path.Curve = edge.Curve;
            path.Position = -(Vector2)edge.A.Position;
            PathContainer!.AddChild(path);
            // Vector2? lastP = null;
            // var start = (Vector2)edge.A.Position;
            // foreach (Vector2 p in edge.Curve.Tessellate())
            // {
            //     if (lastP is Vector2 p_)
            //         if (edge.IsBridge)
            //         {
            //             DrawString(Font, (Vector2)(edge.A.Position + edge.B.Position) / 2 - start, "桥", fontSize: 10, modulate: Colors.Red);
            //             DrawLine(p_ - start, p - start, new Color(1.0f, 0.0f, 0.0f, 1), MapRender.Instance!.GetRoadWidth(edge));
            //         }
            //         else
            //         {
            //             DrawLine(p_ - start, p - start, new Color(0.5f, 0.5f, 0.5f, 1), MapRender.Instance!.GetRoadWidth(edge));
            //         }
            //     lastP = p;
            // }
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

        public void AddRoad(Edge edge)
        {
            Edges.Add(edge);
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
            Edges.ForEach(DrawRoad);
        }
    }
}
