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
        public int BaseProduceSpeed = 10 + GD.RandRange(-4, 4);
        public QuadTree<Factory>.Handle QuadTreeHandle;

        private List<ProduceLink> _inputLinks = new List<ProduceLink>();
        private List<ProduceLink> _outputLinks = new List<ProduceLink>();
        public IReadOnlyList<ProduceLink> InputLinks => _inputLinks;
        public IReadOnlyList<ProduceLink> OutputLinks => _outputLinks;

        public ItemBox IdealInput;
        public ItemBox IdealOutput;

        public ItemBox CapacityInput { get => new ItemBox(Recipe.Input) * BaseProduceSpeed; }
        public ItemBox CapacityOutput { get => new ItemBox(Recipe.Output) * BaseProduceSpeed; }

        public int ProduceCount { get; private set; } = 0;
        public Dictionary<TrainLine, List<Goods>> Platform = new Dictionary<TrainLine, List<Goods>>();
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

        public IEnumerable<ProduceLink> Links => InputLinks.Concat(OutputLinks);

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
        public List<Goods> OutputGoods(Train train, TrainLine line, int max)
        {
            var result = new List<Goods>();
            var count = 0;
            if (!Platform.ContainsKey(line))
                Platform[line] = new List<Goods>();
            var pla = Platform[line];
            while (count < max && Platform.Count > 0 && pla.Count > 0)
            {
                if (pla.First().Item.Number + count <= max)
                {
                    var goods = Platform[line].First();
                    pla.Remove(goods);
                    Platform[line].Remove(goods);
                    count += goods.Item.Number;
                    result.Add(goods);
                }
                else
                    break;
            }
            return result;
        }
        public void LoadGoods(Goods goods, Train train)
        {
            if (goods.Ticket.Trips.Last().End == Vertex)
            {
                Storage.AddItem(goods.Item);
                goods.EnterFactory(this, train);
            }
            else
            {
                if (Platform.ContainsKey(goods.Ticket.CurTrip.Line))
                    Platform[goods.Ticket.CurTrip.Line].Add(goods);
                else
                    Platform[goods.Ticket.CurTrip.Line] = new List<Goods>(new[] { goods });
            }
        }

        private void Produce()
        {
            var count = 0;
            while (count < BaseProduceSpeed)
            {
                if (OutputLinks.Count == 0)
                {
                    // 消费
                    if (!Storage.HasItem("ABCDEF", BaseProduceSpeed))
                        return;
                    Storage.RequireItem("ABCDEF", BaseProduceSpeed);
                    count += BaseProduceSpeed;
                    ProduceCount += BaseProduceSpeed;
                }
                else
                {
                    foreach (var link in OutputLinks)
                    {
                        var num = link.Item.Number;
                        foreach (var (t, n) in Recipe.Input)
                        {
                            if (!Storage.HasItem(t, n * num))
                                return;
                            Storage.RequireItem(t, n * num);
                        }
                        count += num;
                        ProduceCount += num;
                        var goods = new Goods(link.Item, link);
                        if (Platform.ContainsKey(goods.Ticket.CurTrip.Line))
                            Platform[goods.Ticket.CurTrip.Line].Add(goods);
                        else
                            Platform[goods.Ticket.CurTrip.Line] = new List<Goods>(new[] { goods });
                    }
                }
            }
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
            // 避免整百全部工厂一起生产卡卡
            TickCount++;
            if (TickCount < ID % 100 + 100)
                return;
            TickCount = ID % 100;
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