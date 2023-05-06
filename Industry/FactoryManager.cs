using Godot;
using System.Collections.Generic;
using Shared.QuadTree;
using GraphMoudle;
using IndustryMoudle.Entry;

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
    }
}