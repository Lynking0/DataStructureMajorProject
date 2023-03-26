using System.Collections.Generic;
using Shared.Extensions.ICollectionExtensions;

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Compatible(Item item)
        {
            return Type == item.Type;
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

        public static implicit operator string(Recipe r)
        {
            var input = string.Join(',', r.Input.Map(item => item.Type));
            var output = string.Join(',', r.Output.Map(item => item.Type));
            if (input.Length > 0)
                return input + " => " + output;
            return output;
        }

        public bool Available(Item item)
        {
            foreach (Item output in Output)
            {
                if (output.Type == item.Type)
                    return true;
            }
            return false;
        }
    }
}