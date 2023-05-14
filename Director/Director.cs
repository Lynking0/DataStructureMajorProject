using Godot;
using System;
using UserControl;
using TopographyMoudle;
using IndustryMoudle;
using TransportMoudle;
using System.Linq;
using IndustryMoudle.Link;

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

            MouselInput.MapMoveTo += MapController!.SetMapRelativePosition;
            MouselInput.MapZoomIn += MapController.MapZoomIn;
            MouselInput.MapZoomOut += MapController.MapZoomOut;
            Logger.trace("Director绑定完成");
            Topography.InitParams();
            Topography.Generate();
            Logger.trace("Topography生成完成");
            // var factoryInitStopWatch = new System.Diagnostics.Stopwatch();
            // factoryInitStopWatch.Start();
            Industry.BuildFactories();
            Logger.trace("工厂生成完成");
            Industry.BuildFactoryChains();
            Logger.trace("产业链生成完成");
            var totalDeficit = Factory.TotalDeficit.ToArray();
            Logger.error($"未配平工厂： {totalDeficit.Count()}");
            foreach (var (factory, deficit) in totalDeficit)
            {
                var def = deficit.Select(item => ((string)item.Key, item.Value));
                var affectChains = ProduceChain.Chains
                    .Where(c => c.Links.SelectMany(l => new[] { l.From, l.To }).Contains(factory))
                    .Select(c => c.ID);
                Logger.error($"{factory.ID} {string.Join(" ", def)} affect chain {string.Join(" ", affectChains)}");
            }
            var cs = ProduceChain.Chains
                .Where(
                    c => totalDeficit.Select(item => item.factory)
                    .Intersect(
                        c.Links.SelectMany(l => new[] { l.From, l.To }))
                        .Count() > 0
                    );
            Logger.error($"未配平产业链： {cs.Count()}");
            // factoryInitStopWatch.Stop();
            // GD.Print("Factory build in ", factoryInitStopWatch.ElapsedMilliseconds, " ms");
            // Factory.FactoriesQuadTree.Detail();

            Logger.trace("开始生成交通网络");
            Transport.BuildTrainLines();
            Logger.trace("交通网络生成完成");

            foreach (var line in TrainLine.TrainLines)
            {
                line.GenerateCurve();
                GetNode("%MapRender").GetNode("TrainLineContainer").AddChild(line.Path);
                GetNode("%MapRender").GetNode("TrainContainer").AddChild(new Train(line));
            }
            BindEverything();
        }

        private void BindEverything()
        {
            Factory.Factories.ForEach(f => { Director.Instance!.Tick += f.Tick; });
            Director.Instance!.Tick += FactroyView.Instance!.Refresh;
            Train.Trains.ForEach(t => { Director.Instance!.Tick += t.Tick; });
        }

        private void FocusOn(Vector2 position)
        {
            MapController!.SetMapPosition(position);
        }
        public override void _Process(double delta)
        {
            DeltaCount += delta;
            while (DeltaCount > TickLength)
            {
                Tick!.Invoke();
                DeltaCount -= TickLength;
            }
            var t = Train.Trains.Where(t => t.TrainLine.Level == TrainLineLevel.MainLine).First();
            t.Size = 100;
            FocusOn(t.TrainPosition);
        }
    }
}