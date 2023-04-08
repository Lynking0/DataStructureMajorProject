using Godot;
using System;
using Shared.Extensions.DoubleVector2Extensions;
using static Shared.RandomMethods;

namespace GraphMoudle.DataStructureAndAlgorithm.OptimalCombinationAlgorithm
{
    /// <summary>
    ///   以Line1上一点p1对应的t值作为status，以p1处法线与Line2交点中距p1最近的点p2到p1的距离为energy。
    ///   若Line2上存在若干点，它们的到Line1的最短线段不是Line1的法线（即不在本退火解空间内），说明Line1上离它们最近的点为其中一个端点。
    ///   所以计算本次退火的结果后，还会与Line1两端点分别到Line2的最短距离做比较。
    /// </summary>
    public class EdgeEdgeDistCalculator : Abstract.SimulatedAnnealing<double>
    {
        /// <summary>
        ///   为避免更改VertexEdgeDistCalculator实例的成员变量，本类单独存放一个实例。
        /// </summary>
        private static VertexEdgeDistCalculator EndPointCalculator = new VertexEdgeDistCalculator();
        public static EdgeEdgeDistCalculator Instance = new EdgeEdgeDistCalculator();
        public Vector2D A1;
        public Vector2D B1;
        public Vector2D C1;
        public Vector2D D1;
        public Vector2D A2;
        public Vector2D B2;
        public Vector2D C2;
        public Vector2D D2;
        private Vector2D L1;
        private Vector2D R1;
        private Vector2D L2;
        private Vector2D R2;
        public double MinDistance;
        public double MinStatus;
        public double MaxStatus;
        public EdgeEdgeDistCalculator() : base(0.9966, 5000, 1500, 1, 10) { }

        /// <summary>
        ///   仅用于减少GetEnergy函数中重复代码，本身无意义
        /// </summary>
        private double _getEnergy(Vector2D a, Vector2D b, Vector2D c, Vector2D d, double? t_, Vector2D p)
        {
            if (t_ is double t)
            {
                Vector2D v = a * (t * t * t) + b * (t * t) + c * t + d;
                double result = p.DistanceToD(v);
                // 为防止退火收敛到次优解，当t超出范围时不能赋极大值。
                // 通过两边之和大于第三边，在不影响最终结果条件下尽可能避免energy突变。
                if (t < MinStatus)
                    result += v.DistanceToD(L2);
                else if (t > MaxStatus)
                    result += v.DistanceToD(R2);
                return result;
            }
            return System.Double.PositiveInfinity;
        }
        protected override double GetEnergy(double t)
        {
            // x = (-x0+3x1-3x2+x3)t^3 + (3x0-6x1+3x2)t^2 + (-3x0+3x1)t + x0
            // x'= 3(-x0+3x1-3x2+x3)t^2 + 2(3x0-6x1+3x2)t + (-3x0+3x1)
            // 求法线px+qy+r=0
            Vector2D a, b, c, d;
            a = -A1 + 3 * B1 - 3 * C1 + D1;
            b = 3 * A1 - 6 * B1 + 3 * C1;
            c = -3 * A1 + 3 * B1;
            d = A1;
            Vector2D pq = (3 * t * t) * a + (2 * t) * b + c;
            Vector2D xy = a * (t * t * t) + b * (t * t) + c * t + d;
            // r=-p*x-q*y
            double r = -pq.DotD(xy);
            a = -A2 + 3 * B2 - 3 * C2 + D2;
            b = 3 * A2 - 6 * B2 + 3 * C2;
            c = -3 * A2 + 3 * B2;
            d = A2;
            // (p*Xa+q*Ya)t^3+(p*Xb+q*Yb)t^2+(p*Xc+q*Yc)t+(p*Xd+q*Yd)+r
            double a_ = a.DotD(pq);
            double b_ = b.DotD(pq);
            double c_ = c.DotD(pq);
            double d_ = d.DotD(pq) + r;
            // 盛金公式解三次方程
            (double? t1, double? t2, double? t3) = Utils.SolveCubicEquation(a_, b_, c_, d_);
            return Mathf.Min(
                _getEnergy(a, b, c, d, t1, xy),
                Mathf.Min(
                    _getEnergy(a, b, c, d, t2, xy),
                    _getEnergy(a, b, c, d, t3, xy)
                )
            );
        }
        protected override double GetInitStatus()
        {
            return GD.RandRange(MinStatus, MaxStatus);
        }
        protected override double GetNearStatus(double curStatus, double temperature)
        {
            double range = Math.Pow(temperature / InitTemperature, 3.5);
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
            return Math.Exp((curEnergy - nextEnergy) / (temperature / 2500000));
        }
        protected override void UpdateResult(ref (double status, double energy) result, double status, double energy)
        {
            if (energy < result.energy)
                result = (status, energy);
        }
        /// <summary>
        ///   重写函数，先用VertexEdgeDistCalculator求曲线1端点到曲线2最短距离，再调用本次退火
        /// </summary>
        public override (double status, double energy) Annealing()
        {
            double t, t_;
            t = MinStatus;
            t_ = 1 - MinStatus;
            L1 = A1 * (t_ * t_ * t_) + B1 * (3 * t_ * t_ * t) + C1 * (3 * t_ * t * t) + D1 * (t * t * t);
            L2 = A2 * (t_ * t_ * t_) + B2 * (3 * t_ * t_ * t) + C2 * (3 * t_ * t * t) + D2 * (t * t * t);
            t = MaxStatus;
            t_ = 1 - MaxStatus;
            R1 = A1 * (t_ * t_ * t_) + B1 * (3 * t_ * t_ * t) + C1 * (3 * t_ * t * t) + D1 * (t * t * t);
            R2 = A2 * (t_ * t_ * t_) + B2 * (3 * t_ * t_ * t) + C2 * (3 * t_ * t * t) + D2 * (t * t * t);

            EndPointCalculator.A = A2;
            EndPointCalculator.B = B2;
            EndPointCalculator.C = C2;
            EndPointCalculator.D = D2;
            EndPointCalculator.MinStatus = MinStatus;
            EndPointCalculator.MaxStatus = MaxStatus;
            EndPointCalculator.MinDistance = MinDistance;

            (double status, double energy) result;
            EndPointCalculator.P = L1;
            result = (MinStatus, EndPointCalculator.Annealing().energy);
            if (CanEndAnnealing(result.energy))
                return result;
            EndPointCalculator.P = R1;
            UpdateResult(ref result, MaxStatus, EndPointCalculator.Annealing().energy);
            if (CanEndAnnealing(result.energy))
                return result;
            (double status, double energy) baseResult = base.Annealing();
            UpdateResult(ref result, baseResult.status, baseResult.energy);
            return result;
        }
    }
}