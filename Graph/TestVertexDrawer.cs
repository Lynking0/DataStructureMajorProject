using Godot;
using System;
using GraphMoudle;
using System.Collections.Generic;
using Shared.Extensions.DoubleVector2Extensions;
using GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTreeStructure;

namespace GraphMoudle
{
    public partial class TestVertexDrawer : Node2D
    {
        private int ShowDepth = -1;
        private Vector2D? ShowPosition = null;
        private Vector2D TempPos;
        private HashSet<Edge>? ShowEdges = null;
        public override void _Ready()
        {
            MoveLocalX(GetWindow().Size.X / 2);
            MoveLocalY(GetWindow().Size.Y / 2);
        }
        public override void _Draw()
        {
            foreach (Edge edge in Graph.Instance.Edges)
            {
                if (ShowEdges is null || ShowEdges.Count == 0 || ShowEdges.Contains(edge))
                {
                    Vector2? lastP = null;
                    foreach (Vector2 p in edge.Points)
                    {
                        if (lastP is Vector2 p_)
                            DrawLine(p_, p, new Color(0.5f, 0.5f, 0.5f, 1), 1);
                        lastP = p;
                    }
                }
            }
            foreach (Vertex vertex in Graph.Instance.Vertices)
            {
                DrawCircle((Vector2)vertex.Position, 4, new Color(0.2f, 0.2f, 0.2f, 1));
                // Vector2D v = vertex.Gradient.OrthogonalD().NormalizedD() * 10;
                // DrawLine((Vector2)(vertex.Position - v), (Vector2)(vertex.Position + v), new Color(0.25f, 0.25f, 0.25f, 1), 1);
                // DrawLine((Vector2)(vertex.Position), (Vector2)(vertex.Position - vertex.Gradient.NormalizedD() * 12), new Color(0.25f, 0.25f, 0.25f, 1), 1);
            }
            Color[] colors = {
                Colors.Black,
                Colors.Blue,
                Colors.Brown,
                Colors.Gray,
                Colors.Green,
                Colors.Pink,
                Colors.Purple,
                Colors.Red,
                Colors.Silver,
                Colors.White
            };
            foreach ((int depth, RTRect2 rect) in Graph.Instance.GISInfoStorer.RectangleTraversal())
            {
                if (depth == ShowDepth && (ShowPosition is null || ((Vector2D)ShowPosition).IsInRect(rect.TL.X, rect.TL.Y, rect.BR.X, rect.BR.Y)))
                    DrawRect((Rect2)rect, colors[GD.RandRange(0, 9)], filled: false, width: 1);
            }
        }
        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventKey keyEvent && keyEvent.Pressed)
            {
                if (keyEvent.Keycode == Key.Key0)
                    ShowDepth = 0;
                if (keyEvent.Keycode == Key.Key1)
                    ShowDepth = 1;
                if (keyEvent.Keycode == Key.Key2)
                    ShowDepth = 2;
                if (keyEvent.Keycode == Key.Key3)
                    ShowDepth = 3;
                if (keyEvent.Keycode == Key.Key4)
                    ShowDepth = 4;
                if (keyEvent.Keycode == Key.Key5)
                    ShowDepth = 5;
                if (keyEvent.Keycode == Key.Key6)
                    ShowDepth = 6;
                if (keyEvent.Keycode == Key.Key7)
                    ShowDepth = 7;
                if (keyEvent.Keycode == Key.Key8)
                    ShowDepth = 8;
                if (keyEvent.Keycode == Key.Key9)
                    ShowDepth = 9;
                if (keyEvent.Keycode == Key.A)
                    ShowDepth = 10;
                if (keyEvent.Keycode == Key.B)
                    ShowDepth = 11;
                if (keyEvent.Keycode == Key.C)
                    ShowDepth = 12;
                if (keyEvent.Keycode == Key.D)
                    ShowDepth = 13;
                if (keyEvent.Keycode == Key.E)
                    ShowDepth = 14;
                if (keyEvent.Keycode == Key.F)
                    ShowDepth = 15;
                this.QueueRedraw();
            }
            if (@event is InputEventMouseButton mouseEvent)
            {
                if (mouseEvent.ButtonIndex == MouseButton.Left)
                {
                    ShowPosition = mouseEvent.Pressed ? mouseEvent.Position : null;
                    this.QueueRedraw();
                }
                if (mouseEvent.ButtonIndex == MouseButton.Right)
                {
                    if (mouseEvent.Pressed)
                    {
                        ShowEdges = null;
                        TempPos = mouseEvent.Position;
                    }
                    else
                    {
                        RTRect2 rect = new RTRect2(TempPos, mouseEvent.Position);
                        ShowEdges = new HashSet<Edge>();
                        foreach (Edge edge in Graph.Instance.GISInfoStorer.Search(rect))
                            ShowEdges.Add(edge);
                    }
                    this.QueueRedraw();
                }
            }
        }
    }
}
