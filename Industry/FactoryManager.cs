using Godot;
using System.Collections.Generic;
using Shared.QuadTree;

namespace IndustryMoudle
{
    public partial class Factory
    {
        public static QuadTree<Factory> FactoriesQuadTree = new QuadTree<Factory>(new Rect2(DirectorMoudle.Constants.OriginCoordinates, DirectorMoudle.Constants.WorldSize));
        public static List<Factory> Factories = new List<Factory>();
    }
}