using Godot;
using UserControl;
using TopographyMoudle;
using IndustryMoudle;

namespace Director
{
    public partial class Director : Control
    {
        private MapController? MapController;
        private MapRender? MapRender;
        private MouselInput? MouselInput;

        private double TickLength = 0.024; // 24ms
        private double DeltaCount = 0;

        public delegate void TickHandler();
        public event TickHandler? Tick;

        public override void _Ready()
        {
            MapController = GetNode<MapController>("../MouseInput/GameViewportContainer");
            MapRender = GetNode<MapRender>("../MouseInput/MapRender");
            MouselInput = GetNode<MouselInput>("../MouseInput");

            MouselInput.MapMoveTo += MapController!.SetMapPosition;
            MouselInput.MapZoomIn += MapController.MapZoomIn;
            MouselInput.MapZoomOut += MapController.MapZoomOut;

            Topography.InitParams();
            Topography.Generate();

            // var factoryInitStopWatch = new System.Diagnostics.Stopwatch();
            // factoryInitStopWatch.Start();
            Industry.BuildFactories();
            Industry.BuildFactoryLinks();
            // factoryInitStopWatch.Stop();
            // GD.Print("Factory build in ", factoryInitStopWatch.ElapsedMilliseconds, " ms");
            // Factory.FactoriesQuadTree.Detail();
        }

        public override void _Process(double delta)
        {
            DeltaCount += delta;
            while (DeltaCount > TickLength)
            {
                Tick!.Invoke();
                DeltaCount -= TickLength;
            }
        }
    }
}