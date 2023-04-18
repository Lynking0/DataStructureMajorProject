using Godot;
using System;
using Shared.Extensions.DoubleVector2Extensions;
using static Shared.RandomMethods;

namespace GraphMoudle.DataStructureAndAlgorithm.OptimalCombinationAlgorithm
{
    [Obsolete("因结果不稳定且开销较大，该方案已被弃用")]
    public class VertexEdgeDistCalculator : Abstract.SimulatedAnnealing<double>
    {
        public static VertexEdgeDistCalculator Instance = new VertexEdgeDistCalculator();
        public Vector2D A;
        public Vector2D B;
        public Vector2D C;
        public Vector2D D;
        public Vector2D P;
        public double MinDistance;
        public double MinStatus;
        public double MaxStatus;
        public VertexEdgeDistCalculator() : base(0.997, 5000, 1200, 1, 15) { }
        protected override double GetEnergy(double t)
        {
            Vector2D v = A * Mathf.Pow(1 - t, 3) + B * 3 * (Mathf.Pow(1 - t, 2) * t) + C * 3 * ((1 - t) * t * t) + D * (t * t * t);
            return P.DistanceToD(v);
        }
        protected override double GetInitStatus()
        {
            return GD.RandRange(MinStatus, MaxStatus);
        }
        protected override double GetNearStatus(double curStatus, double temperature)
        {
            double range = Math.Pow(temperature / InitTemperature, 3);
            return NormalRandom(curStatus, range, MinStatus, MaxStatus);
        }
        protected override bool CanEndAnnealing(double energy)
        {
            return energy < MinDistance;
        }
        protected override double GetAcceptPR(double curEnergy, double nextEnergy, double temperature)
        {
            if (nextEnergy < curEnergy)
                return 1;
            return Math.Exp((curEnergy - nextEnergy) / (temperature / 2400000));
        }
        protected override void UpdateResult(ref (double status, double energy) result, double status, double energy)
        {
            if (energy < result.energy)
                result = (status, energy);
        }
    }
}