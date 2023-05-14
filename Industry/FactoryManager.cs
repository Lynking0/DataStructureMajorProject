using Godot;
using System.Collections.Generic;
using Shared.QuadTree;
using GraphMoudle;
using IndustryMoudle.Entry;
using System.Linq;

namespace IndustryMoudle
{
    public partial class Factory
    {
        public static QuadTree<Factory> FactoriesQuadTree = new QuadTree<Factory>(new Rect2(DirectorMoudle.Constants.OriginCoordinates, DirectorMoudle.Constants.WorldSize));
        public static List<Factory> Factories = new List<Factory>();
        private static Dictionary<Vertex, Factory> _vertexToFactory = new Dictionary<Vertex, Factory>();
        public static IReadOnlyDictionary<Vertex, Factory> VertexToFactory => _vertexToFactory;

        public static ItemBox Blanace
        {
            get
            {
                var result = new ItemBox();
                foreach (var factory in Factories)
                {
                    result += factory.IdealInput;
                    result += factory.IdealOutput * -1;
                }
                return result;
            }
        }

        public static int ConsumeCount
        {
            get
            {
                var result = 0;
                foreach (var factory in Factories)
                {
                    result += factory.IdealOutput.GetItem("ABCDEF");
                }
                return result;
            }
        }

        public static IEnumerable<(Factory factory, Dictionary<ItemType, int> deficit)> TotalDeficit
        {
            get
            {
                return Factory.Factories.Select(f =>
               {
                   var input = new ItemBox(f.InputLinks.Select(l => l.Item));
                   var outputNumber = f.OutputLinks.Select(l => l.Item.Number).Sum();
                   var deficit = new Dictionary<ItemType, int>();
                   foreach (var (type, num) in f.Recipe.Input)
                   {
                       var (deficitNumber, _) = input.RequireItem(type, num * outputNumber);
                       if (deficitNumber != 0)
                           deficit[type] = deficitNumber;
                       if (input.GetItem(type) != 0)
                       {
                           deficit[type] = -input.GetItem(type);
                       }
                   }
                   return (f, deficit);
               })
               .Where(i => i.deficit.Values.Sum() > 0);
            }
        }
    }
}