using Godot;

namespace DirectorMoudle
{
    public partial class MapController : SubViewportContainer
    {
        private double MapScale = 1;
        private Vector2 MapTransform = new Vector2(0, 0);
        private Vector2 WindowSize;
        private static Timer? WindowSizeUpdateToShaderTimer;
        private Camera2D? MainCamera;

        private void OnWindowSizeChanged()
        {
            WindowSize = GetWindow().Size;
            if (WindowSizeUpdateToShaderTimer is not null)
                WindowSizeUpdateToShaderTimer.Start();
        }

        public override void _Ready()
        {
            Visible = true;
            MainCamera = GetNode<Camera2D>("%MainCamera");
            GetWindow().SizeChanged += OnWindowSizeChanged;
            (Material as ShaderMaterial)!.SetShaderParameter("windowSize", GetWindow().Size);
            (Material as ShaderMaterial)!.SetShaderParameter("vp", GetNode<SubViewport>("%SubViewport").GetTexture());
            UpdateMap();
            WindowSizeUpdateToShaderTimer = new Timer();
            WindowSizeUpdateToShaderTimer.OneShot = true;
            WindowSizeUpdateToShaderTimer.WaitTime = 0.25f;
            AddChild(WindowSizeUpdateToShaderTimer);
            WindowSizeUpdateToShaderTimer.Timeout += () =>
            {
                (Material as ShaderMaterial)!.SetShaderParameter("windowSize", WindowSize);
            };
        }

        private void UpdateBackgroundShader()
        {
            (Material as ShaderMaterial)!.SetShaderParameter("transform", MapTransform);
            (Material as ShaderMaterial)!.SetShaderParameter("scale", MapScale);
        }

        private void UpdateMap()
        {
            UpdateBackgroundShader();
        }

        public void SetMapPosition(Vector2 positionDelta)
        {
            MapTransform += positionDelta;
            MainCamera!.Position -= positionDelta;
            UpdateMap();
        }

        public void MapZoomIn(Vector2 mousePosition)
        {
            MapScale *= 1.1;
            MapTransform *= 1.1f;
            MainCamera!.Zoom *= 1.1f;
            UpdateMap();
        }

        public void MapZoomOut(Vector2 mousePosition)
        {
            MapScale /= 1.1;
            MapTransform /= 1.1f;
            MainCamera!.Zoom /= 1.1f;
            UpdateMap();
        }
    }
}