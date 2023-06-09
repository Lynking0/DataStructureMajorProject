using Godot;
using System.Collections.Generic;
using System.Linq;
using IndustryMoudle;
using IndustryMoudle.Link;
using GraphMoudle;
using TransportMoudle;
using TransportMoudle.Extensions;

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
                var window = GetNode<Window>("/root/Main/FactroyViewWindow");
                window.Popup();
                window.GetNode<FactroyView>("FactroyView").Refresh(Factory!);
            };
        }

        private void DrawLink(ProduceLink link)
        {
            var from = Vector2.Zero;
            var to = link.To.Position - link.From.Position;
            DrawArrow(from, to, link.Chain.Color, width: 2);
            DrawString(Font, (from + to) / 2, link, fontSize: 10, modulate: Colors.Red);
        }

        private void DrawEdge(Edge edge, float width, Color color, string? text = null)
        {
            var start = (Vector2)edge.A.Position;
            var points = edge.Curve.Tessellate().Select(v => v - start).ToArray();
            DrawPolyline(points, (Color)color, width);
            var arrow = (points.Last() - points.First()).Normalized().Rotated(Mathf.Pi / 2);
            var center = (points.First() + points.Last()) / 2;
            var nearestPoint = points.Aggregate((a, b) => a.DistanceSquaredTo(center) < b.DistanceSquaredTo(center) ? a : b);

            // DrawString(Font, nearestPoint + arrow * -10,
            //     loadInfo.ForwardLoad.ToString(), fontSize: 10, modulate: Colors.Blue);

            // DrawString(Font, nearestPoint + arrow * 10,
            //     loadInfo.ReverseLoad.ToString(), fontSize: 10, modulate: Colors.Blue);
            if (text is not null)
            {
                DrawString(Font, nearestPoint, text, fontSize: 10, modulate: Colors.Blue);
            }
        }

        private void DrawRoad(Edge edge)
        {
            var mapRender = GetParent().GetParent<MapRender>();
            var color = edge.IsBridge ? Colors.Red : Colors.Gray;

            var trainLine = edge.GetTrainLines()
                .GroupBy(l => l.Level);
            if (trainLine.Count() == 0)
            {
                // this edge is a road
                if (!mapRender.RoadDisplay)
                {
                    return;
                }
            }
            else
            {
                // this edge is a train line
                foreach (var line in trainLine)
                {
                    if (line.Key == TrainLineLevel.MainLine && mapRender.TrainLine1Display
                        || line.Key == TrainLineLevel.SideLine && mapRender.TrainLine2Display
                        || line.Key == TrainLineLevel.FootPath && mapRender.TrainLine3Display)
                    {
                        color = line.First().Color;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            var loadInfo = ProduceLink.GetEdgeLoad(edge);
            if (loadInfo.TotalLoad == 0)
            {
                return;
            }
            var width = MapRender.Instance!.GetRoadWidth(loadInfo.TotalLoad);
            DrawEdge(edge, width, color, loadInfo.TotalLoad.ToString());
        }

        private void DrawLine(TrainLine line)
        {
            foreach (var edge in line.Edges)
            {
                var loadInfo = ProduceLink.GetEdgeLoad(edge);
                if (loadInfo.TotalLoad == 0)
                {
                    continue;
                }
                var width = MapRender.Instance!.GetRoadWidth(loadInfo.TotalLoad);
                DrawEdge(edge, width, line.Color);
            }
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
            var mapRender = GetParent().GetParent<MapRender>();
            if (mapRender.FactoryDisplay)
            {
                Color color = default;
                switch (Factory.Recipe.Group)
                {
                    case IndustryMoudle.Entry.RecipeGroup.Raw:
                        color = Colors.WebGray;
                        break;
                    case IndustryMoudle.Entry.RecipeGroup.Factory:
                        color = Colors.WebGreen;
                        break;
                    case IndustryMoudle.Entry.RecipeGroup.City:
                        color = Colors.WebMaroon;
                        break;
                }
                DrawCircle(Vector2.Zero, 6, color);
                DrawString(Font, Vector2.Zero, Factory.Recipe, fontSize: 12);
            }
            if (mapRender.LinkDisplay)
            {
                Links.ForEach(DrawLink);
            }
            if (mapRender.RoadDisplay || mapRender.TrainLine1Display || mapRender.TrainLine2Display || mapRender.TrainLine3Display)
            {
                foreach (var edge in Edges)
                {
                    DrawRoad(edge);
                }
            }
        }
    }
}
