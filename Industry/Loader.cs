using Godot;
using System.Collections.Generic;

namespace Industry
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
            _recipes.Add(new Recipe(10, "", new List<Item> { }, new List<Item> { new Item(1, "A") }));
            _recipes.Add(new Recipe(10, "", new List<Item> { }, new List<Item> { new Item(1, "B") }));
            _recipes.Add(new Recipe(10, "", new List<Item> { }, new List<Item> { new Item(1, "C") }));
            _recipes.Add(new Recipe(10, "", new List<Item> { }, new List<Item> { new Item(1, "D") }));


            _recipes.Add(new Recipe(10, "", new List<Item> { new Item(1, "A"), new Item(1, "B") }, new List<Item> { new Item(1, "AB") }));
            _recipes.Add(new Recipe(10, "", new List<Item> { new Item(1, "AB"), new Item(1, "C") }, new List<Item> { new Item(1, "ABC") }));

            _recipes.Add(new Recipe(10, "", new List<Item> { new Item(1, "B"), new Item(1, "C") }, new List<Item> { new Item(1, "BC") }));
            _recipes.Add(new Recipe(10, "", new List<Item> { new Item(1, "BC"), new Item(1, "D") }, new List<Item> { new Item(1, "BCD") }));

            _recipes.Add(new Recipe(10, "", new List<Item> { new Item(1, "A"), new Item(1, "C") }, new List<Item> { new Item(1, "AC") }));
            _recipes.Add(new Recipe(10, "", new List<Item> { new Item(1, "B"), new Item(1, "D") }, new List<Item> { new Item(1, "BD") }));

            _recipes.Add(new Recipe(10, "", new List<Item> { new Item(1, "AC"), new Item(1, "BD") }, new List<Item> { new Item(1, "ABCD") }));
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