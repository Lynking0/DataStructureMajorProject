using Godot;
using System;
using System.Collections.Generic;
using GraphMoudle;
using System.Linq;
using IndustryMoudle.Entry;
using IndustryMoudle.Link;

namespace IndustryMoudle
{
    public partial class Industry
    {
        private static Dictionary<Vertex, Factory> VertexToFactory = new Dictionary<Vertex, Factory>();
        public static void BuildFactories()
        {
            // Formula.Program.MyFun(Graph.Instance.Vertices.Count);
            // var recipeEnumerator = Formula.Program.factories.GetEnumerator();
            // foreach (Vertex vertex in Graph.Instance.Vertices)
            // {
            //     recipeEnumerator.MoveNext();
            //     var factory = new Factory(recipeEnumerator.Current.ToRecipe(), vertex);
            //     VertexToFactory[vertex] = factory;
            // }
            foreach (Vertex vertex in Graph.Instance.Vertices)
            {
                var factory = new Factory(Loader.Instance.GetRandomRecipe(), vertex);
                VertexToFactory[vertex] = factory;
            }
            Logger.trace($"生成工厂 {Factory.Factories.Count} 座");
            Logger.trace($"工厂理想平衡情况 {(string)Factory.Blanace}");
            Logger.trace($"工厂理想消费品产能 {Factory.ConsumeCount}");
        }

        public static void BuildFactoryChains()
        {
            // 使用四叉树查找
            // foreach (var curFactory in Factory.Factories)
            // {
            //     var nearFactories = curFactory.QuadTreeHandle.Nearby(curFactory);
            //     foreach (var requirement in curFactory.Recipe.Input)
            //     {
            //         foreach (var targetFactory in nearFactories)
            //         {
            //             if (targetFactory.Recipe.Output.Any(outputItem => requirement.Type == outputItem.Type))
            //             {
            //                 var link = new ProduceLink(targetFactory, curFactory, requirement);
            //                 break;
            //             }
            //         }
            //     }
            // }
            // 使用浩哥查找

            (List<(Factory factory, int outputNumber)> downstream, List<ProduceLink> links) linkFactory(Factory curFactory, int requirementNumber, ProduceChain chain)
            {
                var queue = new List<(Vertex cur, int parIndex)>();
                var set = new HashSet<Vertex>();
                var requirement = (new ItemBox(curFactory.Recipe.Input) * requirementNumber).ToDictionary();
                var requirementTypes = requirement.Keys.ToList();
                var downstream = new List<(Factory factory, int outputNumber)>();
                var links = new List<ProduceLink>();
                // for BFS
                void Extend(Vertex vertex, int index)
                {
                    set.Add(vertex);
                    foreach (var edge in vertex.Adjacencies)
                    {
                        if (edge.GetOtherEnd(vertex) is Vertex otherVertex)
                        {
                            if (set.Contains(otherVertex))
                            {
                                continue;
                            }
                            queue!.Add((otherVertex, index));
                            set.Add(otherVertex);
                        }
                    }
                }
                List<Vertex> GetPath(int i)
                {
                    var index = i;
                    var result = new List<Vertex>();
                    while (index != 0)
                    {
                        result.Add(queue[index].cur);
                        index = queue[index].parIndex;
                    }
                    return result;
                }

                // 尝试复用已有的链接
                foreach (var (type, number) in requirement)
                {
                    foreach (var link in curFactory.InputLinks)
                    {
                        if (link.Item.Type != type)
                        {
                            continue;
                        }
                        var (deficit, actual) = link.From.IdealOutput.RequireItem(type, number);
                        if (actual == 0)
                        {
                            continue;
                        }
                        if (deficit == 0)
                        {
                            requirement.Remove(type);

                        }
                        else
                        {
                            requirement[type] = deficit;
                        }
                        link.Item.Number += actual;
                    }
                }

                Extend(curFactory.Vertex, 0);
                int i = 0;
                while (i < queue.Count && requirementTypes.Count > 0)
                {
                    var (otherVertex, parIndex) = queue[i];
                    var targetFactory = VertexToFactory[otherVertex];
                    var intersect = targetFactory.Recipe.OutputTypes.Intersect(requirementTypes);
                    if (intersect is not null)
                    {
                        foreach (var itemType in intersect)
                        {
                            if (!requirement.ContainsKey(itemType))
                            {
                                continue;
                            }
                            var outputNumber = targetFactory.IdealOutput.GetItem(itemType);

                            var (deficit, actual) = targetFactory.IdealOutput.RequireItem(itemType, requirement[itemType]);
                            if (actual == 0)
                            {
                                continue;
                            }
                            if (deficit == 0)
                            {
                                requirement.Remove(itemType);
                            }
                            else
                            {
                                requirement[itemType] = deficit;
                            }
                            var link = new ProduceLink(targetFactory, curFactory, GetPath(i), new Item(actual, itemType), chain);
                            targetFactory.AddOutputLink(link);
                            curFactory.AddInputLink(link);
                            links.Add(link);
                            downstream.Add((factory: targetFactory, outputNumber: actual));
                        }
                    }
                    Extend(otherVertex, i);
                    i++;
                }
                foreach (var (type, number) in requirement)
                {
                    chain.AddDeficit(type, number);
                }
                return (downstream, links);
            }

            foreach (var consumptionFactory in Factory.Factories.Where(f => f.Recipe.Group == "consumption"))
            {
                // Build industrial chain
                var chain = new ProduceChain("ABC", consumptionFactory.Vertex.ParentBlock, consumptionFactory);
                chain.AddFactory(consumptionFactory);
                var factoryQueue = new Queue<(Factory factory, int outputNumber)>();
                var (downstream, links) = linkFactory(consumptionFactory, consumptionFactory.BaseProduceSpeed, chain);
                chain.AddFactoryRange(downstream.Select(t => t.factory));
                chain.AddLinkRange(links);
                downstream.ForEach(factoryQueue.Enqueue);

                while (factoryQueue.Count > 0)
                {
                    var (factory, outputNumber) = factoryQueue.Dequeue();
                    (downstream, links) = linkFactory(factory, outputNumber, chain);
                    chain.AddFactoryRange(downstream.Select(t => t.factory));
                    chain.AddLinkRange(links);
                    downstream.ForEach(factoryQueue.Enqueue);
                }
                // 定性链路构建完成，开始定量收缩
                void ShirkChain(ProduceChain chain)
                {
                    // 目前只考虑最终工厂为消费工厂
                    if (chain.OutputFactory.Recipe.Group != "consumption")
                    {
                        Logger.error("目前只考虑最终工厂为消费工厂");
                        throw new Exception("目前只考虑最终工厂为消费工厂");
                    }
                    var consumerFactory = chain.OutputFactory;
                    var consumption = GetActualOutput(consumerFactory);
                    AdjustLink(consumerFactory, consumption);
                }

                // 根据上游输入，获取该工厂实际产出
                int GetActualOutput(Factory factory)
                {
                    // 原材料工厂
                    if (factory.Recipe.Input.Count == 0)
                        return factory.CapacityOutput.ToList()[0].number;
                    var outputNumber = 0;
                    // 消费工厂
                    if (factory.CapacityOutput.ToList().Count == 0)
                        outputNumber = int.MaxValue;
                    else
                        outputNumber = factory.CapacityOutput.ToList()[0].number;
                    foreach (var type in factory.Recipe.Input.Keys)
                    {
                        // 该原料的消耗比例
                        var rate = factory.Recipe.Input[type];
                        var supplyCount = 0;
                        foreach (var link in factory.InputLinks.Where(l => l.Item.Type == type))
                        {
                            supplyCount += GetActualOutput(link.From);
                        }
                        outputNumber = Math.Min(outputNumber, supplyCount / rate);
                    }
                    return outputNumber;
                }

                // 根据实际产出，调整上游链路
                void AdjustLink(Factory factory, int outputNumber)
                {
                    // 缩减输入，释放过量的输入
                    foreach (var type in factory.Recipe.Input.Keys)
                    {
                        var rate = factory.Recipe.Input[type];
                        var requirementNumber = rate * outputNumber;
                        var supplyCount = 0;
                        var enough = false;

                        foreach (var link in factory.InputLinks.ToArray())
                        {
                            if (link.Item.Type != type)
                            {
                                continue;
                            }
                            if (enough || outputNumber == 0)
                            {
                                // 释放该link
                                link.From.IdealOutput.AddItem(new Item(link.Item.Number, type));
                                link.Destroy();
                                // 释放下游需求
                                AdjustLink(link.From, 0);
                                continue;
                            }
                            supplyCount += link.Item.Number;
                            // 上游供应溢出
                            if (supplyCount > requirementNumber)
                            {
                                enough = true;
                                var overflow = supplyCount - requirementNumber;
                                link.Item.Number -= overflow;
                                link.From.IdealOutput.AddItem(new Item(overflow, type));
                                // 释放下游需求
                                AdjustLink(link.From, link.Item.Number);
                            }
                            // 上游供应恰好满足
                            else if (supplyCount == requirementNumber)
                            {
                                enough = true;
                                // 释放下游需求
                                AdjustLink(link.From, link.Item.Number);
                            }
                        }
                    }
                }

                // ShirkChain(chain);
            }

            Logger.trace($"生成产业链 {ProduceChain.Chains.Count} 条");
            Logger.trace($"完整产业链 {ProduceChain.Chains.Where(c => c.Deficit.Empty).Count()} 条");
            Logger.trace($"产业链平衡情况 {(string)ProduceChain.AllDeficit}");
            Logger.trace($"产业链计划消费品产能 {ProduceChain.ConsumeCount}");
        }
    }
}


