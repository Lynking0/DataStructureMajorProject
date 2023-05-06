using Godot;
using System.Collections.Generic;
using Shared.QuadTree;
using TransportMoudle;
using IndustryMoudle.Entry;
using IndustryMoudle.Link;

namespace IndustryMoudle
{
    public partial class Factory : ILocatable
    {
        private static int IDCount = 0;
        public readonly int ID = IDCount++;
        public Recipe Recipe { get; }
        public GraphMoudle.Vertex Vertex { get; }
        public Vector2 Position { get => (Vector2)Vertex.Position; }
        private Dictionary<ItemType, int> storage = new Dictionary<ItemType, int>();
        // 工厂产能 固定的
        public int BaseProduceSpeed = 100 + GD.RandRange(-4, 4);
        public QuadTree<Factory>.Handle QuadTreeHandle;

        private List<ProduceLink> _inputLinks = new List<ProduceLink>();
        private List<ProduceLink> _outputLinks = new List<ProduceLink>();
        public IReadOnlyList<ProduceLink> InputLinks => _inputLinks;
        public IReadOnlyList<ProduceLink> OutputLinks => _outputLinks;

        public ItemBox IdealInput;
        public ItemBox IdealOutput;

        public ItemBox CapacityInput { get => new ItemBox(Recipe.Input) * BaseProduceSpeed; }
        public ItemBox CapacityOutput { get => new ItemBox(Recipe.Output) * BaseProduceSpeed; }

        public void AddInputLink(ProduceLink link)
        {
            _inputLinks.Add(link);
        }
        public void RemoveInputLink(ProduceLink link)
        {
            _inputLinks.Remove(link);
        }
        public void AddOutputLink(ProduceLink link)
        {
            _outputLinks.Add(link);
        }
        public void RemoveOutputLink(ProduceLink link)
        {
            _outputLinks.Remove(link);
        }

        public IEnumerable<ProduceLink> Links
        {
            get
            {
                foreach (var link in InputLinks)
                {
                    yield return link;
                }
                foreach (var link in OutputLinks)
                {
                    yield return link;
                }
            }
        }

        public Factory(Recipe recipe, GraphMoudle.Vertex vertex)
        {
            Recipe = recipe;
            Vertex = vertex;
            QuadTreeHandle = FactoriesQuadTree.Insert(this);
            Factories.Add(this);
            _vertexToFactory[vertex] = this;
            IdealInput = new ItemBox(Recipe.Input) * BaseProduceSpeed;
            IdealOutput = new ItemBox(Recipe.Output) * BaseProduceSpeed;
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