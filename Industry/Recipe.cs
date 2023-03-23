


namespace Industry
{

    public struct Item
    {
        public readonly int Number;
        public readonly string Type;
        public Item(int number, string type)
        {
            Number = number;
            Type = type;
        }
    }

    public class Recipe
    {
        public readonly int Cost;
        public readonly Item[] Input;
        public readonly Item[] Output;
    }
}