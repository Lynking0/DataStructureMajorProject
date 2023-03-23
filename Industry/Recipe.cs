using System.Collections.Generic;

namespace Industry
{
    // public enum ItemType
    // {
    //     A,
    //     B,
    //     C,
    //     AB,
    //     BC,
    //     AC,
    //     ABC
    // }
    using ItemType = System.String;
    public partial class Item
    {
        public uint Number;
        public ItemType Type;
        public Item(uint number, ItemType type)
        {
            Number = number;
            Type = type;
        }
    }

    public class Recipe
    {
        public readonly uint Time;
        public readonly string Group;
        private readonly List<Item> _input;
        public readonly List<Item> _output;
        public IReadOnlyCollection<Item> Input => _input;
        public IReadOnlyCollection<Item> Output => _output;
        public Recipe(in uint time, in string group, in List<Item> intput, in List<Item> output)
        {
            Time = time;
            Group = group;
            _output = output;
            _input = intput;
        }
    }
}