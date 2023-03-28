using Godot;
using System;

namespace TopographyMoudle
{
    public static partial class FractalNoiseGenerator
    {
        public static class PseudorandomGenerator
        {
            public struct PseudorandomData
            {
                public double[,] data;
                public int xOffset;
                public int yOffset;
            }
            /// <summary>
            ///   预先计算好的伪随机数
            /// </summary>
            public static PseudorandomData[]? Precalculations;
            private const int PrimeX = 501125321;
            private const int PrimeY = 1136930381;
            private static Int32 Hash(Int32 seed, Int32 x, Int32 y)
            {
                Int32 hash = seed ^ x * PrimeX ^ y * PrimeY;
                hash *= 0x27d4eb2d;
                return hash;
            }
            /// <summary>
            ///   伪随机数生成器
            /// </summary>
            /// <returns>[-1,1)的伪随机数</returns>
            private static double Pseudorandom(Int32 seed, Int32 x, Int32 y)
            {
                Int32 hash = Hash(seed, x, y);
                hash *= hash;
                hash ^= hash << 19;
                return hash * (1 / 2147483648.0);
            }
            public static void Precalculate(double minX, double minY, double maxX, double maxY)
            {
                minX *= Frequency;
                minY *= Frequency;
                maxX *= Frequency;
                maxY *= Frequency;
                Precalculations = new PseudorandomData[Octaves];
                // 每边各多算一位以防万一
                for (int oct = 0; oct < Octaves; ++oct)
                {
                    int minXI = Mathf.FloorToInt(minX);
                    int minYI = Mathf.FloorToInt(minY);
                    int maxXI = Mathf.FloorToInt(maxX);
                    int maxYI = Mathf.FloorToInt(maxY);
                    Precalculations[oct].xOffset = 2 - minXI;
                    Precalculations[oct].yOffset = 2 - minYI;
                    Precalculations[oct].data = new double[
                        maxXI - minXI + 6,
                        maxYI - minYI + 6
                    ];
                    for (int i = minXI - 2; i <= maxXI + 3; ++i)
                    {
                        for (int j = minYI - 2; j <= maxYI + 3; ++j)
                        {
                            Precalculations[oct].data[
                                i - minXI + 2, j - minYI + 2
                            ] = Pseudorandom(Seed + oct, i, j);
                        }
                    }
                    minX *= Lacunarity;
                    minY *= Lacunarity;
                    maxX *= Lacunarity;
                    maxY *= Lacunarity;
                }
            }
        }
    }
}