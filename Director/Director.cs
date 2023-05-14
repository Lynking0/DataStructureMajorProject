using Godot;
using System;
using System.Linq;
using UserControl;
using TopographyMoudle;
using IndustryMoudle;
using IndustryMoudle.Link;
using IndustryMoudle.Extensions;
using TransportMoudle;
using TransportMoudle.Extensions;
using GraphMoudle;

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
        public event TickHandler? Tick100;

        public Director()
        {
            if (Instance is not null)
            {
                throw new NotSupportedException("Director重复实例化");
            }
            Instance = this;
        }
        public void SetGameSpeed(bool state, int rate)
        {
            if (state)
                TickLength = 0.024 / rate;
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
            }
            Logger.trace("删除孤立点、边");
            foreach (var edge in Graph.Instance.Edges.Where(e => e.GetTrainLines().Count() == 0).ToArray())
            {
                Graph.Instance.RemoveEdge(edge).ForEach(v => Factory.Factories.Remove(v.GetFactory()!));
            }

            DirectorMoudle.MapRender.Instance?.QueueRedraw();
            BindEverything();
        }

        private void BindEverything()
        {
            Factory.Factories.ForEach(f => { Director.Instance!.Tick += f.Tick; });
            Director.Instance!.Tick100 += FactroyView.Instance!.Refresh;
            Train.Trains.ForEach(t => { Director.Instance!.Tick += t.Tick; });
        }

        private void FocusOn(Vector2 position)
        {
            MapController!.SetMapPosition(position);
        }
        int TickCount = 0;
        public override void _Process(double delta)
        {
            DeltaCount += delta;
            while (DeltaCount > TickLength)
            {
                Tick!.Invoke();
                TickCount += 1;
                if (TickCount % 100 == 0)
                    Tick100?.Invoke();
                DeltaCount -= TickLength;
                // GD.Print(Factory.Factories.Where(f => f.ProduceCount == 0).Count());
                // var a = Train.Trains.GroupBy(t => t.GoodsCount / 10).Select(g => $"[{g.Key},{g.Count()}]");
                // GD.Print(string.Join(" ", a));

                if (TickCount < 20000)
                {
                    foreach (var line in TrainLine.TrainLines)
                    {
                        if (line.Level == TrainLineLevel.MainLine && (TickCount == 100 || TickCount == 3000 || TickCount == 6000 || TickCount == 9000 || TickCount == 12000))
                        {
                            var t = new Train(line);
                            Tick += t.Tick;
                            GetNode("%MapRender").GetNode("TrainContainer").AddChild(t);
                        }
                        if (line.Level == TrainLineLevel.SideLine && (TickCount == 100 || TickCount == 3000 || TickCount == 6000))
                        {
                            var t = new Train(line);
                            Tick += t.Tick;
                            GetNode("%MapRender").GetNode("TrainContainer").AddChild(t);
                        }
                        if (line.Level == TrainLineLevel.FootPath && TickCount == 100)
                        {
                            var t = new Train(line);
                            Tick += t.Tick;
                            GetNode("%MapRender").GetNode("TrainContainer").AddChild(t);
                        }
                    }
                }
            }
            // var t = Train.Trains.Where(t => t.TrainLine.Level == TrainLineLevel.MainLine).First();
            // t.Size = 100;
            // FocusOn(t.TrainPosition);
        }
    }
}