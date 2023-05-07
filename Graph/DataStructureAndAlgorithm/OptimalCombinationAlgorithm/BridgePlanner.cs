using Godot;
using System;
using System.Collections.Specialized;
using Shared.Extensions.DoubleVector2Extensions;
using TopographyMoudle;

namespace GraphMoudle.DataStructureAndAlgorithm.OptimalCombinationAlgorithm
{
    public class BridgePlanner : Abstract.GeneticAlgorithm<BridgePlanner.BridgeData, uint>
    {
        public static BridgePlanner Instance = new BridgePlanner();
        public struct BridgeData
        {
            Vector2D Pos;
            Vector2D CtrlOffset;
            public BridgeData(Vector2D pos, Vector2D ctrlOffset)
            {
                Pos = pos;
                CtrlOffset = ctrlOffset;
            }
            public void Deconstruct(out Vector2D pos, out Vector2D ctrlOffset)
            {
                pos = Pos;
                ctrlOffset = CtrlOffset;
            }
        }
        public Vector2D A;
        /// <summary>
        ///   绝对坐标
        /// </summary>
        public Vector2D ACtrl;
        public Vector2D B;
        /// <summary>
        ///   绝对坐标
        /// </summary>
        public Vector2D BCtrl;
        public Vector2D CentralPosition;
        public double MaxSemiMajorAxis;
        public double MaxSemiMinorAxis;
        private const int RadiusBitCount = 12;
        private const uint RadiusSelector = 0b_1111_1111_1111_0000_0000_0000_0000_0000;
        private const int RadianBitCount = 12;
        private const uint RadianSelector = 0b_0000_0000_0000_1111_1111_1111_0000_0000;
        private const int DirectionBitCount = 8;
        private const uint DirectionSelector = 0b_0000_0000_0000_0000_0000_0000_1111_1111;
        public BridgePlanner() : base(60, 300, 0.6, 0.02)
        {
#if SECURITY
            // 检验各常量正确性
            if ((Utils.FullOneNumbers[RadiusBitCount] << (RadianBitCount + DirectionBitCount)) != RadiusSelector)
                throw new Exception($"{GetType()}.BridgePlanner(): Selector value error.");
            if ((Utils.FullOneNumbers[RadianBitCount] << DirectionBitCount) != RadianSelector)
                throw new Exception($"{GetType()}.BridgePlanner(): Selector value error.");
            if (Utils.FullOneNumbers[DirectionBitCount] != DirectionSelector)
                throw new Exception($"{GetType()}.BridgePlanner(): Selector value error.");
            if (
                (RadiusSelector & RadianSelector & DirectionSelector) != 0 ||
                (RadiusSelector | RadianSelector | DirectionSelector) != 0xffffffff
            )
                throw new Exception($"{GetType()}.BridgePlanner(): Selector value error.");
#endif
        }

        protected override BridgeData ConvertToData(in uint individual)
        {
            uint _radius = (individual & RadiusSelector) >> (RadianBitCount + DirectionBitCount);
            uint _radian = (individual & RadianSelector) >> DirectionBitCount;
            uint _direction = individual & DirectionSelector;
            double semiMajorAxis = _radius * MaxSemiMajorAxis / (1u << RadiusBitCount);
            double semiMinorAxis = _radius * MaxSemiMinorAxis / (1u << RadiusBitCount);
            double radian = _radian * Math.Tau / (1u << RadianBitCount);
            double direction = (_direction * Math.PI / (1u << DirectionBitCount)) * 0.8;
            Vector2D posOffset = new Vector2D(semiMinorAxis * Math.Cos(radian), semiMajorAxis * Math.Sin(radian));
            posOffset = posOffset.RotatedD((B - A).AngleD());
            Vector2D pos = CentralPosition + posOffset;
            Vector2D ctrlOffset = new Vector2D(Graph.CtrlPointDistance * 0.6 * Math.Cos(direction), Graph.CtrlPointDistance * 0.6 * Math.Sin(direction));
            if (Mathf.Abs(ctrlOffset.AngleToD(A - pos)) >= Math.PI / 2)
                ctrlOffset = -ctrlOffset;
            return new BridgeData(pos, ctrlOffset);
        }
        protected override void InitPopulation()
        {
            for (int i = 0; i < Population.Length; ++i)
            {
                Population[i].individual = GD.Randi();
                Population[i].fitness = CalcFitness(Population[i].individual);
            }
        }
        protected override double CalcFitness(in uint individual)
        {
            (Vector2D pos, Vector2D ctrlOffset) = ConvertToData(individual);
            Curve2D curve = new Curve2D();
            curve.AddPoint((Vector2)A, @out: (Vector2)(ACtrl - A));
            curve.AddPoint((Vector2)pos, @in: (Vector2)ctrlOffset, @out: -(Vector2)ctrlOffset);
            curve.AddPoint((Vector2)B, @in: (Vector2)(BCtrl - B));
            Vector2[] points = curve.TessellateEvenLength(4, 6);
            double ans = 0;
            foreach (Vector2 point in points)
            {
                double temp = FractalNoiseGenerator.GetFractalNoise(point.X, point.Y) - Graph.MaxVertexAltitude;
                ans += temp < 0 ? 0 : temp * temp;
            }
            ans /= points.Length;
            if (BestIndividual is (uint _, double fitness))
            {
                if (ans < fitness)
                    BestIndividual = (individual, ans);
            }
            else
                BestIndividual = (individual, ans);
            return ans;
        }
        protected override void Select(out uint individual1, out uint individual2)
        {
            double[] cumulativeFitness = new double[Population.Length];
            // 计算累计适应度
            cumulativeFitness[0] = Population[0].fitness;
            for (int i = 1; i < Population.Length; ++i)
                cumulativeFitness[i] = cumulativeFitness[i - 1] + Population[i].fitness;
            // 转轮盘
            double value = GD.RandRange(0, cumulativeFitness[Population.Length - 1]);
            for (int i = 0; ; ++i)
            {
                if (value < cumulativeFitness[i])
                {
                    individual1 = Population[i].individual;
                    // 对应个体概率减为0，防止重复选择
                    for (int j = i; j < Population.Length; ++j)
                        cumulativeFitness[j] -= Population[i].fitness;
                    break;
                }
            }
            // 再转一次
            value = GD.RandRange(0, cumulativeFitness[Population.Length - 1]);
            for (int i = 0; ; ++i)
            {
                if (value < cumulativeFitness[i])
                {
                    individual2 = Population[i].individual;
                    break;
                }
            }
        }
        protected override void Cross(ref uint individual1, ref uint individual2)
        {
            int startPos = GD.RandRange(1, 32);
            int endPos = GD.RandRange(1, 32);
            if (startPos > endPos)
                Utils.Swap(ref startPos, ref endPos);
            uint sliceSelector = Utils.FullOneNumbers[startPos - 1] ^ Utils.FullOneNumbers[endPos];
            // 交换对应段
            uint a1 = individual1 & sliceSelector;
            uint a2 = individual2 & ~sliceSelector;
            uint b1 = individual2 & sliceSelector;
            uint b2 = individual1 & ~sliceSelector;
            individual1 = a1 | a2;
            individual2 = b1 | b2;
        }
        protected override void Mutate(ref uint individual)
        {
            individual ^= 1u << GD.RandRange(0, 31);
        }
        protected override bool CanEndEarly()
        {
            if (BestIndividual is (uint _, double fitness))
                return fitness < Graph.MaxVertexAltitude * Graph.MaxVertexAltitude * 0.25;
            return false;
        }
    }
}