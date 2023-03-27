using Godot;
using System.Collections.Generic;
using Shared.QuadTree;

namespace Industry
{
    public partial class Factory
    {
        private static List<Factory> _Factories = new List<Factory>();
        public static IReadOnlyCollection<Factory> Factories => _Factories;
        public static QuadTree<Factory> QuadTree = new QuadTree<Factory>(new Rect2(0, 0, 1152, 648));
    }
}