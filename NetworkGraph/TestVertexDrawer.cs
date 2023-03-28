using Godot;
using System;
using NetworkGraph;
using Shared.Extensions.DoubleVector2Extensions;

namespace NetworkGraph
{
    public partial class TestVertexDrawer : Node2D
    {
        public override void _Draw()
        {
            foreach (Edge edge in Graph.Instance.Edges)
            {
                Vector2? lastP = null;
                foreach (Vector2 p in edge.Curve.Tessellate())
                {
                    if (lastP is Vector2 p_)
                        DrawLine(p_, p, new Color(0.5f, 0.5f, 0.5f, 1), 1);
                    lastP = p;
                }
            }
            foreach (Vertex vertex in Graph.Instance.Vertices)
            {
                if (vertex.Position.IsInRect(0, 0, 1152, 648))
                {
                    DrawCircle((Vector2)vertex.Position, 2, new Color(0.2f, 0.2f, 0.2f, 1));
                    Vector2D v = vertex.Gradient.OrthogonalD().NormalizedD() * 15;
                    DrawLine((Vector2)(vertex.Position - v), (Vector2)(vertex.Position + v), new Color(0.25f, 0.25f, 0.25f, 1), 1);
                    DrawLine((Vector2)(vertex.Position), (Vector2)(vertex.Position - vertex.Gradient.NormalizedD() * 20), new Color(0.25f, 0.25f, 0.25f, 1), 1);
                }
            }
        }
    }
}
