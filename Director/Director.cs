using Godot;
using System;
using UserControl;
using TopographyMoudle;
using IndustryMoudle;

namespace DirectorMoudle
{
    public partial class Director : Control
    {
        public static Director? Instance = null;

        private MapController? MapController;
        private MapRender? MapRender;
        private MouselInput? MouselInput;

        private double TickLength = 0.024; // 24ms
        private double DeltaCount = 0;

        public delegate void TickHandler();
        public event TickHandler? Tick;

        public Director()
        {
            if (Instance is not null)
            {
                throw new NotSupportedException("Director重复实例化");
            }
            Instance = this;
        }

        public override void _Ready()
        {
            Logger.trace(this, "Director Ready");
            MapController = GetNode<MapController>("../MouseInput/GameViewportContainer");
            MapRender = GetNode<MapRender>("../MouseInput/MapRender");
            MouselInput = GetNode<MouselInput>("../MouseInput");

            MouselInput.MapMoveTo += MapController!.SetMapPosition;
            MouselInput.MapZoomIn += MapController.MapZoomIn;
            MouselInput.MapZoomOut += MapController.MapZoomOut;
            Logger.trace(this, "Director绑定完成");
            Topography.InitParams();
            Topography.Generate();
            Logger.trace(this, "Topography生成完成");
            // var factoryInitStopWatch = new System.Diagnostics.Stopwatch();
            // factoryInitStopWatch.Start();
            Industry.BuildFactories();
            Logger.trace(this, "工厂生成完成");
            Industry.BuildFactoryLinks();
            Logger.trace(this, "产业链生成完成");

            // factoryInitStopWatch.Stop();
            // GD.Print("Factory build in ", factoryInitStopWatch.ElapsedMilliseconds, " ms");
            // Factory.FactoriesQuadTree.Detail();

            BindEverything();
        }

        private void BindEverything()
        {
            Factory.Factories.ForEach(f => { Director.Instance!.Tick += f.Tick; });
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