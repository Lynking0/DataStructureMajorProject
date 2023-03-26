using Godot;
using GraphInformation;

namespace Industry
{
    public partial class Industry : Node2D
    {
        private FontVariation Font;

        Industry()
        {
            Font = new FontVariation();
            Font.BaseFont = ResourceLoader.Load<Font>("res://Render/PingFang-SC-Regular.ttf");

        }
        public override void _Ready()
        {
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }

#if DEBUG
        public override void _Draw()
        {
            foreach (Vertex vertex in Graph.Instance.Vertices)
            {
                if (vertex.Position.IsInRect(0, 0, 1152, 648))
                {
                    var factory = new Factory(Loader.Instance.GetRandomRecipe(), vertex.Position);
                    DrawString(Font, (Vector2)vertex.Position, factory.Recipe, fontSize: 12);
                }
            }
        }
#endif
    }
}


