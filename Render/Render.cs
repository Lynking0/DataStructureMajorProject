using Godot;
using GraphInformation;
using GraphInformation.DoubleVector2Extensions;

namespace Render
{
    public partial class Render : Node2D
    {
        [Export]
        private Font? _font;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            GD.Print(_font);
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }

#if DEBUG
        public override void _Draw()
        {
            foreach (Vertex vertex in Graph.Instance.vertices)
            {
                if (vertex.Position.IsInRect(0, 0, 1152, 648))
                {
                    DrawCircle((Vector2)vertex.Position, 2, new Color(0.2f, 0.2f, 0.2f, 1));
                    Vector2D v = vertex.Gradient.OrthogonalD().NormalizedD() * 15;
                    DrawLine((Vector2)(vertex.Position - v), (Vector2)(vertex.Position + v), new Color(0.25f, 0.25f, 0.25f, 1), 1);

                    DrawChar(_font, (Vector2)vertex.Position, "$");
                }
            }
        }
#endif
    }
}
