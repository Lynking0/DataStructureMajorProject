using System;
using Godot;

namespace TopographyMoudle
{
    public static partial class FractalNoiseGenerator
    {
        /// <summary>
        ///   双三次插值
        /// </summary>
        private static double BicubicInterpolation(int seed, double tarX, double tarY)
        {
            int[] x = new int[4];
            int[] y = new int[4];
            x[1] = Mathf.FloorToInt(tarX);
            y[1] = Mathf.FloorToInt(tarY);
            double _x = tarX - x[1];
            double _y = tarY - y[1];
            x[0] = x[1] - 1; y[0] = y[1] - 1;
            x[2] = x[1] + 1; y[2] = y[1] + 1;
            x[3] = x[1] + 2; y[3] = y[1] + 2;
            return CubicInterpolation(
                CubicInterpolation(Pseudorandom(seed, x[0], y[0]), Pseudorandom(seed, x[1], y[0]), Pseudorandom(seed, x[2], y[0]), Pseudorandom(seed, x[3], y[0]),
                          _x),
                CubicInterpolation(Pseudorandom(seed, x[0], y[1]), Pseudorandom(seed, x[1], y[1]), Pseudorandom(seed, x[2], y[1]), Pseudorandom(seed, x[3], y[1]),
                          _x),
                CubicInterpolation(Pseudorandom(seed, x[0], y[2]), Pseudorandom(seed, x[1], y[2]), Pseudorandom(seed, x[2], y[2]), Pseudorandom(seed, x[3], y[2]),
                          _x),
                CubicInterpolation(Pseudorandom(seed, x[0], y[3]), Pseudorandom(seed, x[1], y[3]), Pseudorandom(seed, x[2], y[3]), Pseudorandom(seed, x[3], y[3]),
                          _x),
                _y) * (1 / (1.5 * 1.5));
        }
        /// <summary>
        ///   求双三次插值函数值及在x, y方向上的偏导数。
        /// </summary>
        private static double BicubicInterpolation(int seed, double tarX, double tarY, out double gradX, out double gradY)
        {
            int[] x = new int[4];
            int[] y = new int[4];
            x[1] = Mathf.FloorToInt(tarX);
            y[1] = Mathf.FloorToInt(tarY);
            double _x = tarX - x[1];
            double _y = tarY - y[1];
            x[0] = x[1] - 1; y[0] = y[1] - 1;
            x[2] = x[1] + 1; y[2] = y[1] + 1;
            x[3] = x[1] + 2; y[3] = y[1] + 2;
            gradX = CubicDerivative(
                    CubicInterpolation(Pseudorandom(seed, x[0], y[0]), Pseudorandom(seed, x[0], y[1]), Pseudorandom(seed, x[0], y[2]), Pseudorandom(seed, x[0], y[3]),
                            _y),
                    CubicInterpolation(Pseudorandom(seed, x[1], y[0]), Pseudorandom(seed, x[1], y[1]), Pseudorandom(seed, x[1], y[2]), Pseudorandom(seed, x[1], y[3]),
                            _y),
                    CubicInterpolation(Pseudorandom(seed, x[2], y[0]), Pseudorandom(seed, x[2], y[1]), Pseudorandom(seed, x[2], y[2]), Pseudorandom(seed, x[2], y[3]),
                            _y),
                    CubicInterpolation(Pseudorandom(seed, x[3], y[0]), Pseudorandom(seed, x[3], y[1]), Pseudorandom(seed, x[3], y[2]), Pseudorandom(seed, x[3], y[3]),
                            _y),
                    _x
            ) * (1 / (1.5 * 1.5));
            double tempA = CubicInterpolation(Pseudorandom(seed, x[0], y[0]), Pseudorandom(seed, x[1], y[0]), Pseudorandom(seed, x[2], y[0]), Pseudorandom(seed, x[3], y[0]),
                            _x);
            double tempB = CubicInterpolation(Pseudorandom(seed, x[0], y[1]), Pseudorandom(seed, x[1], y[1]), Pseudorandom(seed, x[2], y[1]), Pseudorandom(seed, x[3], y[1]),
                            _x);
            double tempC = CubicInterpolation(Pseudorandom(seed, x[0], y[2]), Pseudorandom(seed, x[1], y[2]), Pseudorandom(seed, x[2], y[2]), Pseudorandom(seed, x[3], y[2]),
                            _x);
            double tempD = CubicInterpolation(Pseudorandom(seed, x[0], y[3]), Pseudorandom(seed, x[1], y[3]), Pseudorandom(seed, x[2], y[3]), Pseudorandom(seed, x[3], y[3]),
                            _x);
            gradY = CubicDerivative(tempA, tempB, tempC, tempD, _y) * (1 / (1.5 * 1.5));
            return CubicInterpolation(tempA, tempB, tempC, tempD, _y) * (1 / (1.5 * 1.5));
        }
    }
}