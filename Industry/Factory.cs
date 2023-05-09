using Godot;
using System.Collections.Generic;
using System.Linq;
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
        public ItemBox Storage = new ItemBox();
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

        public ItemBox ActualOutput
        {
            get
            {
                if (Recipe.OutputTypes.First() is null)
                    return new ItemBox();
                return new ItemBox(new[] { new Item(OutputLinks.Sum(link => link.Item.Number), Recipe.OutputTypes.First()) });
            }
        }

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
            Storage.AddItem(goods.Item);
            goods.EnterFactory(this, train);
        }

        private void Produce()
        {
            foreach (var item in CapacityInput)
                var (_, actual) = Storage.RequireItem(item.Key, item.Value);
            foreach (var item in CapacityOutput)
                Storage.AddItem(item.Key, item.Value);
        }
        /// <summary>
        /// 获取该工厂每周期的原料需求
        /// </summary>
        /// <returns></returns>
        public List<Item> GetRequirement()
        {
            var requirement = new List<Item>();
            foreach (var item in CapacityInput)
                requirement.Add(new Item(item.Value / Recipe.Time, item.Key));
            return requirement;
        }

        private int TickCount = 0;
        public void Tick()
        {
            TickCount++;
            if (TickCount < 100)
                return;
            TickCount = 0;
            foreach (var (type, number) in CapacityInput)
            {
                if (!Storage.HasItem(type, number))
                {
                    return;
                }
            }
            Produce();
        }
    }
}