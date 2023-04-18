using System;

namespace GraphMoudle.DataStructureAndAlgorithm
{
    public static class Utils
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }
        /// <summary>
        ///   盛金公式求解三次方程的实数解
        /// </summary>
        public static (double? t1, double? t2, double? t3) SolveCubicEquation(double a, double b, double c, double d)
        {
            double A = b * b - 3 * a * c;
            double B = b * c - 9 * a * d;
            double C = c * c - 3 * b * d;
            double delta = B * B - 4 * A * C;
            if (A == 0 && B == 0)
            {
                double t = -b / (3 * a);
                return (t, null, null);
            }
            else if (delta > 0)
            {
                double Y1 = A * b + 1.5 * a * (-B + Math.Sqrt(delta));
                double Y2 = A * b + 1.5 * a * (-B - Math.Sqrt(delta));
                double t = (-b - (Math.Cbrt(Y1) + Math.Cbrt(Y2))) / (3 * a);
                return (t, null, null);
            }
            else if (delta == 0)
            {
                double K = B / A;
                double t1 = -b / a + K;
                double t2 = -0.5 * K;
                return (t1, t2, null);
            }
            else // delta < 0
            {
                double T = (2 * A * b - 3 * a * B) / (2 * Math.Sqrt(A * A * A));
                double theta = Math.Acos(T);
                double t1 = (-b - 2 * Math.Sqrt(A) * Math.Cos(theta / 3)) / (3 * a);
                double t2 = (-b + 2 * Math.Sqrt(A) * Math.Sin(Math.PI / 6 + theta / 3)) / (3 * a);
                double t3 = (-b + 2 * Math.Sqrt(A) * Math.Sin(Math.PI / 6 - theta / 3)) / (3 * a);
                return (t1, t2, t3);
            }
        }
    }
}