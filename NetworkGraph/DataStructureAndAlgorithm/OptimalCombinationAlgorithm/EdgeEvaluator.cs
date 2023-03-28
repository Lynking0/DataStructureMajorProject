using Godot;
using System;
using Shared.Extensions.DoubleVector2Extensions;
using static Shared.RandomMethods;

namespace NetworkGraph.DataStructureAndAlgorithm.OptimalCombinationAlgorithm
{
    [Obsolete("相关功能已移至EdgeEvaluator.glsl中")]
    public class EdgeEvaluator : Abstract.SimulatedAnnealing<double>
    {
        public static EdgeEvaluator Instance = new EdgeEvaluator();
        public Vector2D A;
        public Vector2D B;
        public Vector2D C;
        public Vector2D D;
        public double MaxEnergy;
        public EdgeEvaluator() : base(0.997, 5000, 2000, 1, 10) { }
        protected override double GetEnergy(double t)
        {
            Vector2D v = A * Mathf.Pow(1 - t, 3) + B * 3 * (Mathf.Pow(1 - t, 2) * t) + C * 3 * ((1 - t) * t * t) + D * (t * t * t);
            return Topography.FractalNoiseGenerator.GetFractalNoise(v.X, v.Y);
        }
        protected override double GetInitStatus()
        {
            return GD.Randf();
        }
        protected override double GetNearStatus(double curStatus, double temperature)
        {
            double range = 1 - Mathf.Exp(-10 * (temperature / InitTemperature - (LowestTemperature - 100) / InitTemperature));
            return NormalRandom(curStatus, range, 0, 1);
        }
        protected override bool CanEndAnnealing(double energy)
        {
            return energy >= MaxEnergy;
        }
        protected override double GetAcceptPR(double curEnergy, double nextEnergy, double temperature)
        {
            if (nextEnergy > curEnergy)
                return 1;
            return Math.Exp((nextEnergy - curEnergy) / (temperature / 2400000));
        }
        protected override void UpdateResult(ref (double status, double energy) result, double status, double energy)
        {
            if (energy > result.energy)
                result = (status, energy);
        }
    }
}