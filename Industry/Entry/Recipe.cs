using System.Collections.Generic;
using Shared.Extensions.ICollectionExtensions;

namespace IndustryMoudle.Entry
{
    public class Recipe
    {
        public readonly int Time;
        public readonly string Group;
        private readonly Dictionary<ItemType, int> _input;
        public readonly Dictionary<ItemType, int> _output;
        public IReadOnlyDictionary<ItemType, int> Input => _input;
        public IEnumerable<ItemType> InputTypes { get => Input.Map(i => i.Key); }
        public IReadOnlyDictionary<ItemType, int> Output => _output;
        public IEnumerable<ItemType> OutputTypes { get => Output.Map(i => i.Key); }
        public Recipe(in int time, in string group, in Dictionary<ItemType, int> intput, in Dictionary<ItemType, int> output)
        {
            Time = time;
            Group = group;
            _output = output;
            _input = intput;
        }

        public static implicit operator string(Recipe r)
        {
            var input = string.Join(',', r.Input.Map(item => (string)item.Key));
            var output = string.Join(',', r.Output.Map(item => (string)item.Key));
            if (input.Length > 0)
                return input + " => " + output;
            return output;
        }

        public bool Available(Item item)
        {
            foreach (var output in Output)
            {
                if (output.Key == item.Type)
                    return true;
            }
            return false;
        }
    }
}