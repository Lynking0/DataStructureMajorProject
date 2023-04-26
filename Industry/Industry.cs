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

            (List<Factory> downstream, List<ProduceLink> links) linkFactory(Factory curFactory, ProduceChain chain)
            {
                var vertex = curFactory.Vertex;
                var queue = new Queue<Vertex>();
                var set = new HashSet<Vertex>();
                // var requirement = new Dictionary<ItemType, int>(curFactory.Recipe.Input);
                var requirement = curFactory.Input.ToDictionary();
                var requirementTypes = curFactory.Recipe.InputTypes.ToList();
                var downstream = new List<Factory>();
                var links = new List<ProduceLink>();
                void Extend(Vertex vertex)
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
                            queue!.Enqueue(otherVertex);
                            set.Add(otherVertex);
                        }
                    }
                }

                Extend(vertex);
                while (queue.Count > 0 && requirementTypes.Count > 0)
                {
                    var otherVertex = queue.Dequeue();
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
                            var requirementNumber = requirement[itemType];
                            var outputNumber = targetFactory.Output.GetItem(itemType);

                            var (deficit, actual) = targetFactory.Output.RequireItem(itemType, requirementNumber);
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
                            var link = new ProduceLink(targetFactory, curFactory, new Item(actual, itemType), chain);
                            targetFactory.OutputLinks.Add(link);
                            curFactory.InputLinks.Add(link);
                            links.Add(link);
                            downstream.Add(targetFactory);
                        }
                    }
                    Extend(otherVertex);
                }
                foreach (var (type, number) in requirement)
                {
                    chain.AddDeficit(type, number);
                }
                return (downstream, links);
            }

            var consumptionFactories = Factory.Factories.Where(f => f.Recipe.Group == "consumption");

            foreach (var consumptionFactory in consumptionFactories)
            {
                // Build industrial chain
                var chain = new ProduceChain("ABC");
                chain.AddFactory(consumptionFactory);
                var factoryQueue = new Queue<Factory>();
                var (downstream, links) = linkFactory(consumptionFactory, chain);
                chain.AddFactoryRange(downstream);
                chain.AddLinkRange(links);
                downstream.ForEach(factoryQueue.Enqueue);

                while (factoryQueue.Count > 0)
                {
                    (downstream, links) = linkFactory(factoryQueue.Dequeue(), chain);
                    chain.AddFactoryRange(downstream);
                    chain.AddLinkRange(links);
                    downstream.ForEach(factoryQueue.Enqueue);
                }
            }
            Logger.trace($"生成产业链 {ProduceChain.Chains.Count} 条");
            Logger.trace($"完整产业链 {ProduceChain.Chains.Where(c => c.Deficit.Empty).Count()} 条");
            Logger.trace($"产业链平衡情况 {(string)ProduceChain.AllDeficit}");
            Logger.trace($"产业链实际消费品产能 {ProduceChain.ConsumeCount}");

        }
    }
}


