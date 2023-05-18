using Godot;
using System;
using UserControl;
using TopographyMoudle;
using IndustryMoudle;
using TransportMoudle;
using System.Linq;

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
            var seed = 58777ul;
            GD.Seed(seed);
            Logger.trace($"随机种子: {seed}");
            Logger.trace("Director Ready");
            MapController = GetNode<MapController>("%GameViewportContainer");
            MapRender = GetNode<MapRender>("%MapRender");
            MouselInput = GetNode<MouselInput>("%MouseInput");

            MouselInput.MapMoveTo += MapController!.SetMapPosition;
            MouselInput.MapZoomIn += MapController.MapZoomIn;
            MouselInput.MapZoomOut += MapController.MapZoomOut;
            Logger.trace("Director绑定完成");
            Topography.InitParams();
            Topography.Generate();
            Logger.trace("Graph生成完成");
            return;
            // var factoryInitStopWatch = new System.Diagnostics.Stopwatch();
            // factoryInitStopWatch.Start();
            Industry.BuildFactories();
            Logger.trace("工厂生成完成");
            Industry.BuildFactoryChains();
            Logger.trace("产业链生成完成");
            // factoryInitStopWatch.Stop();
            // GD.Print("Factory build in ", factoryInitStopWatch.ElapsedMilliseconds, " ms");
            // Factory.FactoriesQuadTree.Detail();

            Logger.trace("开始生成交通网络");
            Transport.BuildTrainLines();
            Logger.trace("交通网络生成完成");

            foreach (var line in TrainLine.TrainLines)
            {
                line.GenerateCurve();
                GetNode("%MapRender").AddChild(line.Path);
                new Train(line);
            }
            BindEverything();
        }

        private void BindEverything()
        {
            Factory.Factories.ForEach(f => { Director.Instance!.Tick += f.Tick; });
            Director.Instance!.Tick += FactroyView.Instance!.Refresh;
            Train.Trains.ForEach(t => { Director.Instance!.Tick += t.Tick; });
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