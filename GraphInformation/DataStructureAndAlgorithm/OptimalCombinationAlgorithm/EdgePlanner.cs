using Godot;
using System;
using static Shared.RandomMethods;

namespace GraphInformation.DataStructureAndAlgorithm.OptimalCombinationAlgorithm
{
    public class EdgePlanner : Abstract.SimulatedAnnealing<(double, double)>
    {
        public static EdgePlanner Instance = new EdgePlanner();
        public Vertex? A;
        public Vertex? B;
        public EdgePlanner() : base(0.998, 10000, 1, 1, 10) { }
        protected override double GetEnergy((double, double) t)
        {
            
            return 0;
        }
        protected override (double, double) GetInitStatus()
        {
            return (GD.Randf(), GD.Randf());
        }
        protected override (double, double) GetNearStatus((double, double) curStatus, double temperature)
        {
            double range = Math.Pow(temperature / this.InitTemperature, 1.5);
            // x = NormalRandom(curStatus.Item1, range, curStatus.Item2, range);
            return (0.0, 0.0);
        }
        protected override double GetAcceptPR(double curEnergy, double nextEnergy, double temperature)
        {
            if (nextEnergy > curEnergy)
                return 1;
            return Math.Exp(-(curEnergy / nextEnergy) / (temperature / 8000));
        }
#if SECURITY
        public override ((double, double) status, double energy) Annealing()
        {
            if (A is null || B is null)
                throw new Exception("GraphInformation.OptimalCombinationAlgorithm.EdgeEvaluator.EdgePlanner(): A, B 不能为空.");
            return base.Annealing();
        }
#endif
    }
}