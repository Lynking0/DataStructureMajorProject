using System.Collections.Generic;

namespace Industry
{
    using ItemType = System.String;
    public class Factory
    {
        private readonly Recipe _recipe;
        private Dictionary<ItemType, uint> storage = new Dictionary<ItemType, uint>();

        private const uint _baseReference = 100;
        Factory(Recipe recipe)
        {
            _recipe = recipe;
        }
        private bool CanProduce()
        {
            foreach (var item in _recipe.Input)
                if (!storage.ContainsKey(item.Type) || storage[item.Type] < item.Number)
                    return false;
            return true;
        }
        private void Produce()
        {
            foreach (var item in _recipe.Input)
                storage[item.Type] -= item.Number;
            foreach (var item in _recipe.Output)
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
            foreach (var item in _recipe.Input)
                requirement.Add(new Item(item.Number * _baseReference / _recipe.Time, item.Type));
            return requirement;
        }
    }
}