using Godot;
using System;

namespace GraphMoudle.DataStructureAndAlgorithm.OptimalCombinationAlgorithm.Abstract
{
    public abstract class GeneticAlgorithm<TData, TIndividual>
    {
        protected readonly int MaxIterTimes;
        protected readonly double CrossProb;
        protected readonly double MutateProb;
        protected (TIndividual individual, double fitness)? BestIndividual;
        protected (TIndividual individual, double fitness)[] Population;
        protected abstract TData ConvertToData(in TIndividual individual);
        /// <summary>
        ///   生成初始种群，并更新BestIndividual
        /// </summary>
        protected abstract void InitPopulation();
        /// <summary>
        ///   计算个体的适应度，并尝试更新BestIndividual
        /// </summary>
        protected abstract double CalcFitness(in TIndividual individual);
        /// <summary>
        ///   在当前种群选择两个个体（注意不能直接引用原种群个体，需进行复制）
        /// </summary>
        protected abstract void Select(out TIndividual individual1, out TIndividual individual2);
        /// <summary>
        ///   对两个体进行交叉操作
        /// </summary>
        protected abstract void Cross(ref TIndividual individual1, ref TIndividual individual2);
        /// <summary>
        ///   对个体进行变异操作
        /// </summary>
        protected abstract void Mutate(ref TIndividual individual);
        protected abstract bool CanEndEarly();
        public GeneticAlgorithm(int populationSize, int maxIterTimes, double crossProb, double mutateProb)
        {
#if SECURITY
            if ((populationSize & 1) == 1)
                throw new Exception($"{GetType()}.GeneticAlgorithm(): The parameter populationSize must be an even number.");
#endif
            Population = new (TIndividual individual, double fitness)[populationSize];
            MaxIterTimes = maxIterTimes;
            CrossProb = crossProb;
            MutateProb = mutateProb;
        }
        public virtual TData Run()
        {
            BestIndividual = null;
            InitPopulation();
            (TIndividual individual, double fitness)[] Subpopulation = new (TIndividual individual, double fitness)[Population.Length];
            for (int i = 0; i < MaxIterTimes; ++i)
            {
                if (CanEndEarly())
                    break;
                for (int idx = 0; idx < Population.Length;)
                {
                    TIndividual individual1, individual2;
                    Select(out individual1, out individual2);
                    if (GD.Randf() < CrossProb)
                        Cross(ref individual1, ref individual2);
                    if (GD.Randf() < MutateProb)
                        Mutate(ref individual1);
                    if (GD.Randf() < MutateProb)
                        Mutate(ref individual2);
                    Subpopulation[idx++] = (individual1, CalcFitness(individual1));
                    Subpopulation[idx++] = (individual2, CalcFitness(individual2));
                }
                Subpopulation.CopyTo(Population, 0);
            }
            if (BestIndividual is (TIndividual individual, double _))
                return ConvertToData(individual);
            throw new Exception($"{GetType()}.Run(): Can't get result.");
        }
    }
}