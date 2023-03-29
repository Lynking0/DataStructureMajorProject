using Godot;
using GraphMoudle;
using System.Linq;

namespace IndustryMoudle
{
    public partial class Industry
    {
        public static void BuildFactories()
        {
            foreach (Vertex vertex in Graph.Instance.Vertices)
            {
                new Factory(Loader.Instance.GetRandomRecipe(), (Vector2)vertex.Position);
            }
        }

        public static void BuildFactoryLinks()
        {
            foreach (var curFactory in Factory.Factories)
            {
                var nearFactories = curFactory.QuadTreeHandle.Nearby(curFactory);
                foreach (var requirement in curFactory.Recipe.Input)
                {
                    foreach (var targetFactory in nearFactories)
                    {
                        if (targetFactory.Recipe.Output.Any(outputItem => requirement.Type == outputItem.Type))
                        {
                            var link = new ProduceLink(targetFactory, curFactory, requirement);
                            break;
                        }
                    }
                }
            }
        }
    }
}


