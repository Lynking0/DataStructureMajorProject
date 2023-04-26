using Godot;
using System.Collections.Generic;
using Shared.QuadTree;
using TransportMoudle;
using IndustryMoudle.Entry;

namespace IndustryMoudle
{
    public partial class Factory : ILocatable
    {
        public Recipe Recipe { get; }
        public GraphMoudle.Vertex Vertex { get; }
        public Vector2 Position { get => (Vector2)Vertex.Position; }
        private Dictionary<ItemType, int> storage = new Dictionary<ItemType, int>();
        // 工厂产能 固定的
        public int BaseProduceSpeed = 100 + GD.RandRange(-20, 20);
        // 工厂可用当前产能 可变的
        public ItemBox Input;
        public ItemBox Output;
        public QuadTree<Factory>.Handle QuadTreeHandle;

        public Factory(Recipe recipe, GraphMoudle.Vertex vertex)
        {
            Recipe = recipe;
            Input = new ItemBox(Recipe.Input) * BaseProduceSpeed;
            Output = new ItemBox(Recipe.Output) * BaseProduceSpeed;
            Vertex = vertex;
            QuadTreeHandle = FactoriesQuadTree.Insert(this);
            Factories.Add(this);
        }
        ~Factory()
        {
            FactoriesQuadTree.Remove(this);
        }

        public void LoadGoods(Goods goods, Train train)
        {
            if (storage.ContainsKey(goods.Item.Type))
                storage[goods.Item.Type] += goods.Item.Number;
            else
                storage.Add(goods.Item.Type, goods.Item.Number);
            goods.EnterFactory(this, train);
        }

        private bool CanProduce()
        {
            foreach (var item in Recipe.Input)
                if (!storage.ContainsKey(item.Key) || storage[item.Key] < item.Value)
                    return false;
            return true;
        }
        private void Produce()
        {
            foreach (var item in Recipe.Input)
                storage[item.Key] -= item.Value;
            foreach (var item in Recipe.Output)
                if (storage.ContainsKey(item.Key))
                    storage[item.Key] += item.Value;
                else
                    storage.Add(item.Key, item.Value);
        }
        public void Update()
        {
            if (CanProduce())
                Produce();
        }
        /// <summary>
        /// 获取该工厂每周期的原料需求
        /// </summary>
        /// <returns></returns>
        public List<Item> GetRequirement()
        {
            var requirement = new List<Item>();
            foreach (var item in Recipe.Input)
                requirement.Add(new Item(item.Value * BaseProduceSpeed / Recipe.Time, item.Key));
            return requirement;
        }

        public void Tick()
        {

        }
    }
}