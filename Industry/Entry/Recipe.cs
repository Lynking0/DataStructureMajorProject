using System.Linq;
using System.Collections.Generic;
using Shared.Extensions.ICollectionExtensions;

namespace IndustryMoudle.Entry
{
    public enum RecipeGroup
    {
        Raw,
        Factory,
        City,
    }
    public class Recipe
    {
        public readonly int Time;
        public RecipeGroup Group
        {
            get
            {
                if (InputTypes.Count() == 0)
                    return RecipeGroup.Raw;
                if (OutputTypes.Count() == 0)
                    return RecipeGroup.City;
                return RecipeGroup.Factory;
            }
        }
        private readonly Dictionary<ItemType, int> _input;
        public readonly Dictionary<ItemType, int> _output;
        public IReadOnlyDictionary<ItemType, int> Input => _input;
        public IEnumerable<ItemType> InputTypes { get => Input.Map(i => i.Key); }
        public IReadOnlyDictionary<ItemType, int> Output => _output;
        public IEnumerable<ItemType> OutputTypes { get => Output.Map(i => i.Key); }
        public Recipe(in int time, in string group, in Dictionary<ItemType, int> intput, in Dictionary<ItemType, int> output)
        {
            Time = time;
            _output = output;
            _input = intput;
        }

        public string DEBUG_OUTPUT()
        {
            var result = "";
            result += Input.Count;
            result += "[";
            foreach (var item in Input)
            {
                result += item.Key + ":" + item.Value + ",";
            }
            result += "]";
            result += " => ";
            result += Output.Count;
            result += "[";
            foreach (var item in Output)
            {
                result += item.Key + ":" + item.Value + ",";
            }
            result += "]";
            return result;
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