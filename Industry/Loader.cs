using Godot;
using System.Collections.Generic;
using IndustryMoudle.Entry;

namespace IndustryMoudle
{
    public partial class Loader
    {
        public static Loader Instance = new Loader();

        private List<Recipe> _recipes = new List<Recipe>();
        public IReadOnlyCollection<Recipe> Recipes => _recipes;

        // Just for DEBUG

        Loader()
        {
#if RECIPE_DEBUG
            _recipes.Add(new Recipe(10, "",
            new Dictionary<ItemType, int>(),
            new Dictionary<ItemType, int> { { "A", 1 } }));
            _recipes.Add(new Recipe(10, "",
            new Dictionary<ItemType, int>(),
            new Dictionary<ItemType, int> { { "B", 1 } }));
            _recipes.Add(new Recipe(10, "",
            new Dictionary<ItemType, int>(),
            new Dictionary<ItemType, int> { { "C", 1 } }));
            _recipes.Add(new Recipe(10, "",
            new Dictionary<ItemType, int>(),
            new Dictionary<ItemType, int> { { "A", 1 } }));
            _recipes.Add(new Recipe(10, "",
            new Dictionary<ItemType, int>(),
            new Dictionary<ItemType, int> { { "B", 1 } }));
            _recipes.Add(new Recipe(10, "",
            new Dictionary<ItemType, int>(),
            new Dictionary<ItemType, int> { { "C", 1 } }));


            _recipes.Add(new Recipe(10, "",
            new Dictionary<ItemType, int>() { { "A", 1 }, { "B", 1 } },
            new Dictionary<ItemType, int>() { { "AB", 1 } }));
            _recipes.Add(new Recipe(10, "",
            new Dictionary<ItemType, int>() { { "A", 1 }, { "C", 1 } },
            new Dictionary<ItemType, int>() { { "AC", 1 } }));
            _recipes.Add(new Recipe(10, "",
            new Dictionary<ItemType, int>() { { "B", 1 }, { "C", 1 } },
            new Dictionary<ItemType, int>() { { "BC", 1 } }));

            _recipes.Add(new Recipe(10, "",
            new Dictionary<ItemType, int>() { { "AB", 1 }, { "C", 1 } },
            new Dictionary<ItemType, int>() { { "ABC", 1 } }));
            _recipes.Add(new Recipe(10, "",
            new Dictionary<ItemType, int>() { { "AC", 1 }, { "B", 1 } },
            new Dictionary<ItemType, int>() { { "ABC", 1 } }));
            _recipes.Add(new Recipe(10, "",
            new Dictionary<ItemType, int>() { { "BC", 1 }, { "A", 1 } },
            new Dictionary<ItemType, int>() { { "ABC", 1 } }));

            _recipes.Add(new Recipe(10, "consumption",
            new Dictionary<ItemType, int>() { { "ABC", 1 } },
            new Dictionary<ItemType, int>()));

#else
            var file = FileAccess.Open("res://Industry/Recipes.json", FileAccess.ModeFlags.Read);
            var recipeData = Json.ParseString(file.GetAsText()).AsGodotArray();
            foreach (var recipe in recipeData)
            {
                var recipeDict = recipe.AsGodotDictionary();
                var time = recipeDict["time"].AsUInt32();
                var group = "原材料";
                if (recipeDict.ContainsKey("group"))
                {
                    group = recipeDict["group"].ToString();
                }

                var input = new List<Item>();
                foreach (var inputItem in recipeDict["input"].AsGodotArray())
                {
                    var number = inputItem.AsGodotDictionary()["number"].AsUInt32();
                    var type = inputItem.AsGodotDictionary()["type"].ToString();
                    input.Add(new Item(number, type));
                }
                var output = new List<Item>();
                foreach (var outputItem in recipeDict["output"].AsGodotArray())
                {
                    var number = outputItem.AsGodotDictionary()["number"].AsUInt32();
                    var type = outputItem.AsGodotDictionary()["type"].ToString();
                    output.Add(new Item(number, type));
                }
                _recipes.Add(new Recipe(time, group, input, output));
            }
#endif
        }

        public Recipe GetRandomRecipe()
        {
            return _recipes[GD.RandRange(0, _recipes.Count - 1)];
        }
    }
}