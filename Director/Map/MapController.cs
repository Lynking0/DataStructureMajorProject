using Godot;

namespace DirectorMoudle
{
    public partial class MapController : SubViewportContainer
    {
        [Signal]
        public delegate void MapChangedEventHandler(Vector2 transform, double scale);

        private double MapScale = 1;
        private Vector2 MapTransform = new Vector2(0, 0);
        private Vector2 WindowSize;
        private static Timer? WindowSizeUpdateToShaderTimer;

        private void OnWindowSizeChanged()
        {
            WindowSize = GetWindow().Size;
            if (WindowSizeUpdateToShaderTimer is not null)
                WindowSizeUpdateToShaderTimer.Start();
        }

        public override void _Ready()
        {
            GetWindow().SizeChanged += OnWindowSizeChanged;
            (Material as ShaderMaterial)!.SetShaderParameter("windowSize", GetWindow().Size);
            UpdateMapShader();
            WindowSizeUpdateToShaderTimer = new Timer();
            WindowSizeUpdateToShaderTimer.OneShot = true;
            WindowSizeUpdateToShaderTimer.WaitTime = 0.25f;
            AddChild(WindowSizeUpdateToShaderTimer);
            WindowSizeUpdateToShaderTimer.Timeout += () =>
            {
                (Material as ShaderMaterial)!.SetShaderParameter("windowSize", WindowSize);
                EmitSignal(SignalName.MapChanged);
            };
        }

        private void UpdateMapShader()
        {
            (Material as ShaderMaterial)!.SetShaderParameter("transform", MapTransform);
            (Material as ShaderMaterial)!.SetShaderParameter("scale", MapScale);
            EmitSignal(SignalName.MapChanged, MapTransform, MapScale);
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