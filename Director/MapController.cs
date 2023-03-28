using Godot;

namespace Director
{
    partial class MapController : SubViewportContainer
    {
        private double MapScale = 1;
        private Vector2 MapTransform = new Vector2(0, 0);

        public override void _Ready()
        {
            (Material as ShaderMaterial)!.SetShaderParameter("screenSize", GetWindow().Size);
        }

        public override void _Process(double delta)
        {
            (Material as ShaderMaterial)!.SetShaderParameter("transform", MapTransform);
            (Material as ShaderMaterial)!.SetShaderParameter("scale", MapScale);
        }
    }
}