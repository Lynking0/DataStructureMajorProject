using System;
using Godot;

namespace Topography
{
    public static partial class FractalNoiseGenerator
    {
        /// <summary>
        ///   获取预生成的伪随机数
        /// </summary>
        private static double Pseudorandom(Int32 octave, Int32 x, Int32 y)
        {
            PseudorandomGenerator.PseudorandomData Precalculation = 
                PseudorandomGenerator.Precalculations![octave];
#if SECURITY
            if (PseudorandomGenerator.Precalculations is null)
                throw new Exception("Topography.FractalNoiseGenerator.Pseudorandom(): 需先执行预计算.");
            if (octave < 0 || octave >= PseudorandomGenerator.Precalculations!.Length)
                throw new Exception("Topography.FractalNoiseGenerator.Pseudorandom(): octave下标越界.");
            if (PseudorandomGenerator.Precalculations[octave].data is null)
                throw new Exception("Topography.FractalNoiseGenerator.Pseudorandom(): 数据异常.");
            if (x + Precalculation.xOffset < 0 || x + Precalculation.xOffset >= PseudorandomGenerator.Precalculations![octave].data.GetLength(0))
                throw new Exception("Topography.FractalNoiseGenerator.Pseudorandom(): x下标越界.");
            if (y + Precalculation.yOffset < 0 || y + Precalculation.yOffset >= PseudorandomGenerator.Precalculations![octave].data.GetLength(1))
                throw new Exception("Topography.FractalNoiseGenerator.Pseudorandom(): y下标越界.");
#endif
            return Precalculation.data[
                x + Precalculation.xOffset,
                y + Precalculation.yOffset
            ];
        }
        /// <summary>
        /// 1       ^       ^
        ///        / \     / \
        ///       /   \   /   \
        ///      /     \ /     \
        /// 0   /       v       \
        ///     0   1   2   3   4 …
        /// </summary>
        private static double PingPong(double t)
        {
            t -= (int)(t * 0.5f) * 2;
            return t < 1 ? t : 2 - t;
        }

        /// <summary>
        ///   若落在PingPong函数下降段，则将梯度取反。
        /// </summary>
        private static double PingPong(double t, ref double gradX, ref double gradY)
        {
            t -= (int)(t * 0.5f) * 2;
            if (t < 1)
                return t;
            else
            {
                gradX = -gradX;
                gradY = -gradY;
                return 2 - t;
            }
        }

        private static double LinearInterpolation(double a, double b, double t)
        {
            return a + t * (b - a);
        }

        /// <summary>
        ///   曲线是过(0, b)和(1, c)的三次曲线
        /// </summary>
        private static double CubicInterpolation(double a, double b, double c, double d, double t)
        {
            double p = (d - c) - (a - b);
            return t * t * t * p + t * t * ((a - b) - p) + t * (c - a) + b;
        }
        /// <summary>
        ///   CubicInterpolation()求导
        /// </summary>
        private static double CubicDerivative(double a, double b, double c, double d, double t)
        {
            double p = (d - c) - (a - b);
            return 3 * t * t * p + 2 * t * ((a - b) - p) + (c - a);
        }

        /// <summary>
        ///   将最后的结果映射到[0, 1]区间上
        /// </summary>
        private static double TransformResult(double result)
        {
            return (Mathf.Pow(BottomNumber, -result) - 1.0 / BottomNumber) / (BottomNumber - 1.0 / BottomNumber);
        }

        /// <summary>
        ///   将最后的结果映射到[0, 1]区间上，并根据链式法则修改梯度
        /// </summary>
        private static double TransformResult(double result, ref double gradX, ref double gradY)
        {
            double multiplier = -Mathf.Pow(BottomNumber, -result) * Mathf.Log(BottomNumber) / (BottomNumber - 1.0 / BottomNumber);
            gradX *= multiplier;
            gradY *= multiplier;
            return (Mathf.Pow(BottomNumber, -result) - 1.0 / BottomNumber) / (BottomNumber - 1.0 / BottomNumber);
        }
    }
}