using Godot;

namespace Director
{
    public partial class MapController : SubViewportContainer
    {
        private double MapScale = 1;
        private Vector2 MapTransform = new Vector2(0, 0);
        private Vector2 WindowSize;

        public override void _Ready()
        {
            WindowSize = GetWindow().Size;
            (Material as ShaderMaterial)!.SetShaderParameter("windowSize", WindowSize);
            UpdateMapShader();
        }

        private void UpdateMapShader()
        {
            (Material as ShaderMaterial)!.SetShaderParameter("transform", MapTransform);
            (Material as ShaderMaterial)!.SetShaderParameter("scale", MapScale);
        }

        public void SetMapPosition(Vector2 positionDelta)
        {
            MapTransform += positionDelta;
            UpdateMapShader();
        }

        public void MapZoomIn(Vector2 mousePosition)
        {
            MapScale *= 1.1;
            MapTransform *= 1.1f;
            UpdateMapShader();
        }

        public void MapZoomOut(Vector2 mousePosition)
        {
            MapScale /= 1.1;
            MapTransform /= 1.1f;
            UpdateMapShader();
        }
    }
}