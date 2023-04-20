using Godot;
using System.Collections.Generic;
using Shared.QuadTree;

using TransportMoudle;

namespace IndustryMoudle
{
    public partial class Factory : ILocatable
    {
        public Recipe Recipe { get; }
        public GraphMoudle.Vertex Vertex { get; }
        public Vector2 Position { get => (Vector2)Vertex.Position; }
        private Dictionary<ItemType, uint> storage = new Dictionary<ItemType, uint>();
        private const uint BaseProduceSpeed = 100;
        public QuadTree<Factory>.Handle QuadTreeHandle;

        public Factory(Recipe recipe, GraphMoudle.Vertex vertex)
        {
            Recipe = recipe;
            Vertex = vertex;
            QuadTreeHandle = FactoriesQuadTree.Insert(this);
            Factories.Add(this);
        }
        ~Factory()
        {
            FactoriesQuadTree.Remove(this);
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

        public void Tick()
        {

        }
    }
}