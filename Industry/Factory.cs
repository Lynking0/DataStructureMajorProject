using Godot;
using System.Collections.Generic;
using GraphInformation.DoubleVector2Extensions;

namespace Industry
{
    using ItemType = System.String;
    public partial class Factory
    {
        public readonly Recipe Recipe;
        public readonly Vector2D Position;
        private Dictionary<ItemType, uint> storage = new Dictionary<ItemType, uint>();
        private const uint BaseProduceSpeed = 100;

        public Factory(Recipe recipe, Vector2D position)
        {
            Recipe = recipe;
            Position = position;
            _Factories.Add(this);
        }
        ~Factory()
        {
            _Factories.Remove(this);
        }

        private bool CanProduce()
        {
            foreach (Item item in Recipe.Input)
                if (!storage.ContainsKey(item.Type) || storage[item.Type] < item.Number)
                    return false;
            return true;
        }
        private void Produce()
        {
            foreach (Item item in Recipe.Input)
                storage[item.Type] -= item.Number;
            foreach (Item item in Recipe.Output)
                if (storage.ContainsKey(item.Type))
                    storage[item.Type] += item.Number;
                else
                    storage.Add(item.Type, item.Number);
        }
        public void Update()
        {
            if (CanProduce())
                Produce();
        }
        public void AddItem(ItemType type, uint number)
        {
            if (storage.ContainsKey(type))
                storage[type] += number;
            else
                storage.Add(type, number);
        }
        /// <summary>
        /// 获取该工厂每周期的原料需求
        /// </summary>
        /// <returns></returns>
        public List<Item> GetRequirement()
        {
            var requirement = new List<Item>();
            foreach (var item in Recipe.Input)
                requirement.Add(new Item(item.Number * BaseProduceSpeed / Recipe.Time, item.Type));
            return requirement;
        }
    }
}