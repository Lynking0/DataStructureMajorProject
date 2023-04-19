using Godot;
using System.Collections.Generic;
using Shared.QuadTree;

namespace IndustryMoudle
{
    public partial class Factory
    {
        public static QuadTree<Factory> FactoriesQuadTree = new QuadTree<Factory>(new Rect2(Director.Constant.OriginCoordinates, Director.Constant.WorldSize));
        public static IEnumerable<Factory> Factories => FactoriesQuadTree.GetItems();
    }
}