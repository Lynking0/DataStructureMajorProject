using Godot;
using System;
using System.Collections.Generic;

namespace Shared
{
    public static class RandomMethods
    {
        /// <summary>
        ///   返回两个标准正态分布的随机数
        /// </summary>
        public static (double, double) RandomNormalDistribution()
        {
            double u = 0, v = 0, w = 0, c = 0;
            do
            {
                u = GD.Randf() * 2 - 1;
                v = GD.Randf() * 2 - 1;
                w = u * u + v * v;
            } while (w == 0 || w >= 1);
            c = Math.Sqrt((-2 * Math.Log(w)) / w);
            return (u * c, v * c);
        }
        /// <summary>
        ///   生成正态分布的随机数
        /// </summary>
        public static double NormalRandom(double mean = 0, double deviation = 1)
        {
            (double a, double _) = RandomNormalDistribution();
            return a * deviation + mean;
        }
        /// <summary>
        ///   生成min与max之间的正态分布的随机数
        /// </summary>
        public static double NormalRandom(double mean = 0, double deviation = 1, double min = System.Double.MinValue, double max = System.Double.MaxValue)
        {
            while (true)
            {
                (double a, double b) = RandomNormalDistribution();
                double temp = a * deviation + mean;
                if (temp <= max && temp >= min)
                    return temp;
                temp = b * deviation + mean;
                if (temp <= max && temp >= min)
                    return temp;
            };
        }
        /// <summary>
        ///   生成一对正态分布的随机数
        /// </summary>
        public static (double, double) NormalRandomPair(double mean1 = 0, double deviation1 = 1, double mean2 = 0, double deviation2 = 1)
        {
            (double a, double b) = RandomNormalDistribution();
            return (a * deviation1 + mean1, b * deviation2 + mean2);
        }
        /// <summary>
        ///   随机打乱一个列表
        /// </summary>
        public static void RandomDislocate<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; --i)
            {
                int rand = GD.RandRange(0, i);
                T temp = list[i];
                list[i] = list[rand];
                list[rand] = temp;
            }
        }
    }
}