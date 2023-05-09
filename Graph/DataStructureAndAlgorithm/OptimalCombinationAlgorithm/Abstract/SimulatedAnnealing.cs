// #define PrintAnnealCnt

using Godot;
using System;

namespace GraphMoudle.DataStructureAndAlgorithm.OptimalCombinationAlgorithm.Abstract
{
    public abstract class SimulatedAnnealing<TStatus>
    {
        /// <summary>
        ///   得到某一状态对应的能量
        /// </summary>
        protected abstract double GetEnergy(TStatus status);
        /// <summary>
        ///   得到初始状态
        /// </summary>
        protected abstract TStatus GetInitStatus();
        /// <summary>
        ///   根据当前状态及温度得到临近状态
        /// </summary>
        protected abstract TStatus GetNearStatus(TStatus curStatus, double temperature);
        /// <summary>
        ///   根据能量判断是否可以提前结束退火，默认不提前结束
        /// </summary>
        protected virtual bool CanEndAnnealing(double energy) { return false; }
        /// <summary>
        ///   接受准则
        /// </summary>
        protected abstract double GetAcceptPR(double curEnergy, double nextEnergy, double temperature);
        /// <summary>
        ///   结果更新方式。
        /// </summary>
        protected virtual void UpdateResult(ref (TStatus status, double energy) result, TStatus status, double energy) { result = (status, energy); }

        protected readonly double AttenuationRate;
        protected readonly double InitTemperature;
        protected readonly double LowestTemperature;
        protected readonly int RepeatTimes;
        protected readonly int MaxRejectTimes;

        /// <param name="attenuationRate">温度的衰减系数</param>
        /// <param name="initTemperature">初始温度</param>
        /// <param name="lowestTemperature">最低温度</param>
        /// <param name="repeatTimes">同一温度下重复 扰动-接受 的次数</param>
        /// <param name="maxRejectTimes">最大连续拒绝次数。当温度低于阈值时，若连续多次扰动结果没有被接受，则结束退火过程。</param>
        public SimulatedAnnealing(double attenuationRate, double initTemperature
            , double lowestTemperature, int repeatTimes, int maxRejectTimes)
        {
            this.AttenuationRate = attenuationRate;
            this.InitTemperature = initTemperature;
            this.LowestTemperature = lowestTemperature;
            this.RepeatTimes = repeatTimes;
            this.MaxRejectTimes = maxRejectTimes;
        }
        public virtual (TStatus status, double energy) Annealing()
        {
            TStatus status = this.GetInitStatus();
            double energy = this.GetEnergy(status);
            (TStatus status, double energy) result = (status, energy);
            int rejectCnt = 0;
#if PrintAnnealCnt
            int @cnt = 0;
#endif
            for (double t = this.InitTemperature; ; t *= this.AttenuationRate)
            {
                if (t < this.LowestTemperature)
                    t = this.LowestTemperature;
                for (double k = 0; k < RepeatTimes; ++k)
                {
                    if (this.CanEndAnnealing(energy))
                    {
#if PrintAnnealCnt
                        GD.Print(@cnt);
#endif
                        return (status, energy);
                    }
#if PrintAnnealCnt
                    ++@cnt;
#endif
                    TStatus nextStatus = this.GetNearStatus(status, t);
                    double nextEnergy = this.GetEnergy(nextStatus);
                    if (GD.Randf() < this.GetAcceptPR(energy, nextEnergy, t))
                    {
                        rejectCnt = 0;
                        status = nextStatus;
                        energy = nextEnergy;
                        UpdateResult(ref result, status, energy);
                    }
                    else
                    {
                        if (t == this.LowestTemperature)
                        {
                            if (rejectCnt < this.MaxRejectTimes)
                                ++rejectCnt;
                            else
                            {
#if PrintAnnealCnt
                                GD.Print(@cnt);
#endif
                                return result;
                            }
                        }
                    }
                }
            }
        }
    }
}