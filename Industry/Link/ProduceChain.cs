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
#if DEBUG
        public readonly Godot.Color Color;
#endif

        public ProduceChain(ItemType itemType, Block block, Factory outputFactory)
        {
            OutputType = itemType;
            Block = block;
            OutputFactory = outputFactory;
            _chains.Add(this);
#if DEBUG
            Godot.Color[] colors = {
                Godot.Colors.Black,
                Godot.Colors.Blue,
                Godot.Colors.Brown,
                Godot.Colors.Gray,
                Godot.Colors.Green,
                Godot.Colors.Pink,
                Godot.Colors.Purple,
                Godot.Colors.Red,
                Godot.Colors.Silver,
                Godot.Colors.White
            };
            Color = colors[Godot.GD.RandRange(0, colors.Length - 1)];
#endif
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