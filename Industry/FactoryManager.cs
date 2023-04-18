using Godot;
using System.Collections.Generic;
using Shared.QuadTree;

namespace IndustryMoudle
{
    public partial class Factory
    {
        public static QuadTree<Factory> FactoriesQuadTree = new QuadTree<Factory>(new Rect2(-5000, -3000, 10000, 6000));
        public static IEnumerable<Factory> Factories => FactoriesQuadTree.GetItems();
    }
}