using Godot;

namespace Shared.QuadTree
{
    public enum QuadTreeNodeType
    {
        Internal,
        Leaf,
    }
    public enum GeoCode
    {
        LU,
        LD,
        RU,
        RD,
    }
    class Constants
    {
        public static int MAX_ITEM = 4;
        public static int MAX_LEVEL = 8;
    }
}