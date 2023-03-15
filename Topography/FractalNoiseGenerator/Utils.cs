using System;
using Godot;

public static partial class FractalNoiseGenerator
{
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
    ///   将最后的结果映射到[0, 1]区间上
    /// </summary>
    private static double TransformResult(double result)
    {
        return 1 - (Mathf.Log(Mathf.Clamp(result, -1, 1) + 2)) / Mathf.Log(2);
    }
}