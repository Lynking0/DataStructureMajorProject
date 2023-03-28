using Godot;
using System.Collections.Generic;
using Shared.QuadTree;

namespace Industry
{
    public partial class Factory
    {
        public static QuadTree<Factory> FactoriesQuadTree = new QuadTree<Factory>(new Rect2(-1600, -1200, 3200, 2400));
        public static IEnumerable<Factory> Factories => FactoriesQuadTree.GetItems();
    }
}