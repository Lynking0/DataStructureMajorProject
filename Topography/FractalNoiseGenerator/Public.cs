using System;
using Godot;

namespace Topography
{
    public static partial class FractalNoiseGenerator
    {
        /// <summary>
        ///   随机种子（默认为0）。
        /// </summary>
        public static int Seed = 0;
        /// <summary>
        ///   频率（放大倍数的倒数）。（默认为0.01）
        /// </summary>
        public static double Frequency = 0.01;
        /// <summary>
        ///   振幅（分形时的初始振幅）。（默认为1 / 1.75）
        /// </summary>
        public static double Amplitude = 1 / 1.75;
        /// <summary>
        ///   分形时叠加的噪声个数。（默认为1）
        /// </summary>
        public static int Octaves = 1;
        /// <summary>
        ///   PingPong强度。（默认为2.0）
        /// </summary>
        public static double PingPongStrength = 2.0;
        /// <summary>
        ///   更高的WeightedStrength代表着当较低的octave影响较大（噪声值较低）时，则较高的octave影响变小。（默认为0）
        /// </summary>
        public static double WeightedStrength = 0;
        /// <summary>
        ///   octave每高一级，采样点坐标乘以Lacunarity。
        ///   当Lacunarity>1，相当于octave每高一级，要叠加的噪声图像缩小Lacunarity倍。（默认为2.0）
        /// </summary>
        public static double Lacunarity = 2.0;
        /// <summary>
        ///   octave每高一级，振幅乘以Gain。
        ///   当Gain<1，相当于octave每高一级，要叠加的噪声值影响变小。（默认为0.5）
        /// </summary>
        public static double Gain = 0.5;
        /// <summary>
        ///   算出噪声值后取指数时的底数。（默认为2）
        /// </summary>
        public static double BottomNumber = 2;

        /// <summary>
        ///   分形噪声
        /// </summary>
        public static double GetFractalNoise(double x, double y)
        {
            x *= Frequency;
            y *= Frequency;
            int seed = Seed;

            double sum = 0;
            double amp = Amplitude;
            for (int i = 0; i < Octaves; ++i)
            {
                // seed++ —— 改变种子以生成不同噪声
                double singleNoise = BicubicInterpolation(seed++, x, y);
                // 使用PingPong函数进行标准化
                double normalizedNoise = PingPong((singleNoise + 1) * PingPongStrength);
                // 叠加
                // (normalizedNoise - 0.5) * 2 —— 从[0, 1]映射到[-1, 1]
                // * amp —— amp越大，当前噪声对结果影响越大
                sum += (normalizedNoise - 0.5) * 2 * amp;
                // 当WeightedStrength>0，若normalizedNoise值较小，amp也相应的会衰减的更快
                amp *= LinearInterpolation(1.0, normalizedNoise, WeightedStrength);

                x *= Lacunarity;
                y *= Lacunarity;
                amp *= Gain;
            }
            return TransformResult(sum);
        }
        /// <summary>
        ///   获取某点处噪声值及梯度。
        ///   只有当FractalNoiseGenerator.WeightedStrength为0时才能调用本函数。
        /// </summary>
        public static double GetFractalNoise(double x, double y, out double gradX, out double gradY)
        {
#if SECURITY
            if (FractalNoiseGenerator.WeightedStrength != 0) {
                throw new Exception(@"FractalNoiseGenerator.GetGradient(): 
                只有当FractalNoiseGenerator.WeightedStrength为0时才能调用本函数.");
            }
#endif
            x *= Frequency;
            y *= Frequency;
            int seed = Seed;

            double sum = 0;
            double amp = Amplitude;
            double tempGradX, tempGradY;
            gradX = 0;
            gradY = 0;
            for (int i = 0; i < Octaves; ++i)
            {
                double singleNoise = BicubicInterpolation(seed++, x, y, out tempGradX, out tempGradY);
                double normalizedNoise = PingPong((singleNoise + 1) * PingPongStrength, ref tempGradX, ref tempGradY);
                sum += (normalizedNoise - 0.5) * 2 * amp;
                gradX += tempGradX * 2 * amp;
                gradY += tempGradY * 2 * amp;

                x *= Lacunarity;
                y *= Lacunarity;
                amp *= Gain;
            }
            return TransformResult(sum, ref gradX, ref gradY);
        }
    }
}