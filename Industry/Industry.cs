using Godot;
using System;
using System.Collections.Generic;
using GraphMoudle;
using System.Linq;
using IndustryMoudle.Entry;
using IndustryMoudle.Link;

namespace IndustryMoudle
{
    public class EdgeInfo
    {
        public EdgeInfo(Edge? edge, bool reverse)
        {
            Edge = edge;
            Reverse = reverse;
        }

        public Edge? Edge;
        public bool Reverse;

        internal void Deconstruct(out Edge? edge, out bool reverse)
        {
            edge = Edge;
            reverse = Reverse;
        }
    }

    public class VertexInfo
    {
        public VertexInfo(Vertex vertex, EdgeInfo edgeInfo, VertexInfo? parent)
        {
            Vertex = vertex;
            EdgeInfo = edgeInfo;
            Parent = parent;
        }

        public Vertex Vertex;
        public EdgeInfo EdgeInfo;
        public VertexInfo? Parent;

        internal void Deconstruct(out Vertex vertex, out EdgeInfo edgeInfo, out VertexInfo? parent)
        {
            vertex = Vertex;
            edgeInfo = EdgeInfo;
            parent = Parent;
        }
    }

    public partial class Industry
    {
        private static Dictionary<Vertex, Factory> VertexToFactory = new Dictionary<Vertex, Factory>();
        private const int MaxBlockSpan = 2;
        public static void BuildFactories()
        {
            Formula.Program.MyFun(Graph.Instance.Vertices.Count);
            var recipeEnumerator = Formula.Program.factories.GetEnumerator();
            foreach (Vertex vertex in Graph.Instance.Vertices)
            {
                recipeEnumerator.MoveNext();
                Recipe? recipe = recipeEnumerator.Current.ToRecipe();
                var factory = new Factory(recipe, vertex);
                VertexToFactory[vertex] = factory;
            }
            // foreach (Vertex vertex in Graph.Instance.Vertices)
            // {
            //     var factory = new Factory(Loader.Instance.GetRandomRecipe(), vertex);
            //     VertexToFactory[vertex] = factory;
            // }
            Logger.trace($"生成工厂 {Factory.Factories.Count} 座");
            Logger.trace($"工厂理想平衡情况 {(string)Factory.Blanace}");
            Logger.trace($"工厂理想消费品产能 {Factory.ConsumeCount}");
        }

        // 按照所需输出，建立该工厂所需原料供应
        private static List<(Factory factory, int outputNumber, ProduceLink link)>
            linkFactory(Factory curFactory, int requirementNumber, ProduceChain chain, ProduceLink? for_ = null)
        {
            var root = new VertexInfo(curFactory.Vertex, new EdgeInfo(null, default), null);
            var factoryQueue = new MiniPriorityQueue<VertexInfo>();
            var set = new HashSet<Vertex>();
            var requirement = (new ItemBox(curFactory.Recipe.Input) * requirementNumber).ToDictionary();
            var requirementTypes = requirement.Keys.ToList();
            var downstream = new List<(Factory factory, int outputNumber, ProduceLink link)>();
            var links = new List<ProduceLink>();
            // for BFS
            void Extend(VertexInfo vertexInfo, int priority)
            {
                if (priority >= MaxBlockSpan)
                    return;
                set.Add(vertexInfo.Vertex);
                foreach (var edge in vertexInfo.Vertex.Adjacencies)
                {
                    if (edge.GetOtherEnd(vertexInfo.Vertex) is Vertex otherVertex)
                    {
                        if (set.Contains(otherVertex))
                        {
                            continue;
                        }
                        factoryQueue.Enqueue(new VertexInfo(otherVertex,
                            new EdgeInfo(edge, vertexInfo.Vertex == edge.A), vertexInfo),
                            priority + (vertexInfo.Vertex.ParentBlock != otherVertex.ParentBlock ? 1 : 0));
                        set.Add(otherVertex);
                    }
                }
            }
            List<Vertex> GetVertexes(VertexInfo vertexInfo)
            {
                var result = new List<Vertex>();
                var info = vertexInfo;
                while (info.Parent is not null)
                {
                    result.Add(info.Vertex);
                    info = info.Parent;
                }
                result.Add(root.Vertex);
                return result;
            }
            List<EdgeInfo> GetEdges(VertexInfo vertexInfo)
            {
                var result = new List<EdgeInfo>();
                var info = vertexInfo;
                while (info.Parent is not null)
                {
                    result.Add(info.EdgeInfo);
                    info = info.Parent;
                }
                return result;
            }

            // 尝试复用已有的链接
            // 不许用了 链路全部专用！
            // foreach (var (type, number) in requirement.ToArray())
            // {
            //     foreach (var link in curFactory.InputLinks)
            //     {
            //         if (link.Item.Type != type)
            //         {
            //             continue;
            //         }
            //         var (deficit, actual) = link.From.IdealOutput.RequireItem(type, number);
            //         if (actual == 0)
            //         {
            //             continue;
            //         }
            //         if (deficit == 0)
            //         {
            //             requirement.Remove(type);

            //         }
            //         else
            //         {
            //             requirement[type] = deficit;
            //         }
            //         link.Item.Number += actual;
            //     }
            // }

            Extend(new VertexInfo(curFactory.Vertex, new EdgeInfo(null, default), null), 0);
            while (factoryQueue.Count > 0 && requirementTypes.Count > 0)
            {
                var (vertexInfo, priority) = factoryQueue.DequeueWithPriority();
                var (otherVertex, edge, parIndex) = vertexInfo;
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
                        var link = new ProduceLink(targetFactory, curFactory, GetVertexes(vertexInfo),
                            GetEdges(vertexInfo), new Item(actual, itemType), chain, for_);
                        targetFactory.AddOutputLink(link);
                        curFactory.AddInputLink(link);
                        links.Add(link);
                        downstream.Add((targetFactory, actual, link));
                    }
                }
                Extend(vertexInfo, priority);
            }
            foreach (var (type, number) in requirement)
            {
                chain.AddDeficit(curFactory, new Item(number, type));
            }
            return downstream;
        }

        // 木桶收缩产业链
        private static void ShirkChain(ProduceChain chain)
        {
            var consumerFactory = chain.OutputFactory;
            var consumption = GetActualOutput(consumerFactory);
            if (consumption == 0)
            {
                chain.Destory();
            }
            foreach (var link in consumerFactory.InputLinks.ToArray())
            {
                // 顶级供给产业链（输出消费品给消费工厂）调整自身
                var output = GetActualOutput(link.From, link);
                var overflow = link.Item.Number - output;
                link.Item.Number -= overflow;
                link.From.IdealOutput.AddItem(new Item(overflow, link.Item.Type));
                AdjustUpstreamLink(link.From, output, link);
                if (output == 0)
                    link.Destroy();
            }
        }

        // 根据上游输入，获取该工厂实际产出
        private static int GetActualOutput(Factory factory, ProduceLink? parentLink = null)
        {
            // 原材料工厂
            if (factory.Recipe.Input.Count == 0 && parentLink is not null)
            {
                return parentLink.Item.Number;
            }
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
                var links = factory.InputLinks.Where(l => l.Item.Type == type);
                if (parentLink is not null)
                    links = links.Where(l => l.For == parentLink);
                foreach (var link in links)
                {
                    var a = GetActualOutput(link.From, link);
                    supplyCount += GetActualOutput(link.From, link);
                }
                outputNumber = Math.Min(outputNumber, supplyCount / rate);
            }
            return outputNumber;
        }

        // 根据实际产出，调整上游链路
        private static void AdjustUpstreamLink(Factory factory, int outputNumber, ProduceLink for_)
        {
            // 缩减输入，释放过量的输入
            foreach (var type in factory.Recipe.Input.Keys)
            {
                var rate = factory.Recipe.Input[type];
                var requirementNumber = rate * outputNumber;
                var supplyCount = 0;
                var enough = false;

                foreach (var link in factory.InputLinks.Where(l => l.For == for_).ToArray())
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
                        // 释放上游需求
                        AdjustUpstreamLink(link.From, 0, link);
                        continue;
                    }
                    supplyCount += link.Item.Number;
                    // 上游供应满足
                    if (supplyCount >= requirementNumber)
                    {
                        enough = true;
                        // 释放上游需求
                        if (supplyCount > requirementNumber)
                        {
                            var overflow = supplyCount - requirementNumber;
                            link.Item.Number -= overflow;
                            link.From.IdealOutput.AddItem(new Item(overflow, type));
                        }
                        AdjustUpstreamLink(link.From, link.Item.Number, link);
                    }
                }
                // if (supplyCount < requirementNumber)
                // {
                //     Logger.error("过量需求 无法满足");
                //     throw new Exception("过量需求 无法满足");
                // }
            }
        }

        const string consumption = "ABCDEF";
        public static void BuildFactoryChains()
        {
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
            }
            // 使用浩哥查找
            foreach (var consumptionFactory in Factory.Factories.Where(f => f.Recipe.Group == RecipeGroup.City))
            {
                // Build industrial chain
                var factoryQueue = new Queue<(Factory factory, int outputNumber, ProduceLink link)>();
                var chain = new ProduceChain(consumption, consumptionFactory.Vertex.ParentBlock, consumptionFactory);
                chain.AddFactory(consumptionFactory);
                void BFS(Factory factory, int outputNumber, ProduceLink? link = null)
                {
                    var downstream =
                        linkFactory(factory, outputNumber, chain, link);
                    chain.AddFactoryRange(downstream.Select(t => t.factory));
                    chain.AddLinkRange(downstream.Select(d => d.link));
                    downstream.ForEach(factoryQueue.Enqueue);
                }
                BFS(consumptionFactory, consumptionFactory.BaseProduceSpeed);

                while (factoryQueue.Count > 0)
                {
                    var (factory, outputNumber, link) = factoryQueue.Dequeue();
                    BFS(factory, outputNumber, link);
                }

                // 定性链路构建完成，开始定量收缩
                ShirkChain(chain);
            }

            var lengths = ProduceLink.Links.Select(l => l.EdgeInfos.Sum(e => e.Edge?.Length ?? 0));

            Logger.trace($"生成产业链 {ProduceChain.Chains.Count} 条");
            Logger.trace($"产业链接长度 MAX/AVG/MIN {lengths.Max()}/{lengths.Average()}/{lengths.Min()}");
            Logger.trace($"产业链计划消费品产能 {ProduceChain.ConsumeCount}");
        }
    }
}


