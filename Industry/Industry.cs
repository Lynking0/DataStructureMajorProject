using Godot;
using System.Collections.Generic;
using GraphMoudle;
using System.Linq;

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
        }

        public static void BuildFactoryLinks()
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
            foreach (var curFactory in Factory.Factories)
            {
                var vertex = curFactory.Vertex;
                var queue = new Queue<Vertex>();
                var set = new HashSet<Vertex>();
                var requirements = new List<ItemType>(curFactory.Recipe.InputTypes);
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
                while (queue.Count > 0 && requirements.Count > 0)
                {
                    var otherVertex = queue.Dequeue();
                    var a = VertexToFactory[otherVertex].Recipe.OutputTypes;
                    var intersect = VertexToFactory[otherVertex].Recipe.OutputTypes.Intersect(requirements);
                    if (intersect is not null)
                    {
                        foreach (var itemType in intersect)
                        {
                            requirements.Remove(itemType);
                            // TODO: number
                            var link = new ProduceLink(VertexToFactory[otherVertex], curFactory, new Item(0, itemType));
                        }
                    }
                    Extend(otherVertex);
                }
            }
        }
    }
}


