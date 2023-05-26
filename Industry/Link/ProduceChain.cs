using Godot;
using System.Collections.Generic;
using IndustryMoudle.Entry;
using System.Linq;
using GraphMoudle;

namespace IndustryMoudle.Link
{
    public partial class ProduceChain
    {
        private static int IDCount = 0;
        public readonly int ID = IDCount++;
        private List<Factory> _factories = new List<Factory>();
        public IReadOnlyList<Factory> Factories => _factories;
        private List<ProduceLink> _links = new List<ProduceLink>();
        public IReadOnlyList<ProduceLink> Links => _links;
        // 所属区块
        public readonly Block Block;
        // 亏空
        public readonly ItemBox Deficit = new ItemBox();
        public readonly Dictionary<Factory, ItemBox> DeficitPortion = new Dictionary<Factory, ItemBox>();
        // 输出类型
        public readonly ItemType OutputType;
        public readonly Factory OutputFactory;
        public Item Output
        {
            get
            {
                return new Item(Factories
                            .Where(factory => factory.Recipe.OutputTypes.FirstOrDefault() is not null)
                            .Where(factory => factory.Recipe.OutputTypes.First() == OutputType)
                            .SelectMany(factory => factory.OutputLinks)
                            .Where(link => Factories.Contains(link.To))
                            .Sum(link => link.Item.Number), OutputType);
            }
        }
        public readonly Color Color;

        public ProduceChain(ItemType itemType, Block block, Factory outputFactory)
        {
            OutputType = itemType;
            Block = block;
            OutputFactory = outputFactory;
            _chains.Add(this);

            Color[] colors = {
                Colors.Black,
                Colors.Blue,
                Colors.Brown,
                Colors.Gray,
                Colors.Green,
                Colors.Pink,
                Colors.Purple,
                Colors.Red,
                Colors.Silver,
                Colors.White
            };
            Color = colors[GD.RandRange(0, colors.Length - 1)];
        }
        public void Destory()
        {
            _chains.Remove(this);
            foreach (var link in Links)
            {
                link.Destroy();
            }
        }
        public void AddFactory(Factory factory)
        {
            _factories.Add(factory);
        }

        public void AddFactoryRange(IEnumerable<Factory> factories)
        {
            _factories.AddRange(factories);
        }

        public void AddLink(ProduceLink link)
        {
            _links.Add(link);
        }

        public void AddLinkRange(IEnumerable<ProduceLink> links)
        {
            _links.AddRange(links);
        }
        public void AddDeficit(Factory factory, Item item)
        {
            Deficit.AddItem(item);
            if (!DeficitPortion.ContainsKey(factory))
                DeficitPortion.Add(factory, new ItemBox());
            DeficitPortion[factory].AddItem(item);
        }
    }
}