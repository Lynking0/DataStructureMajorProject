using System.Collections.Generic;
using IndustryMoudle.Entry;
using System.Linq;
using GraphMoudle;

namespace IndustryMoudle.Link
{
    public partial class ProduceChain
    {
        private List<Factory> _factories = new List<Factory>();
        public IReadOnlyCollection<Factory> Factories => _factories;
        private List<ProduceLink> _links = new List<ProduceLink>();
        public IReadOnlyCollection<ProduceLink> Links => _links;
        // 所属区块
        public readonly Block Block;
        // 亏空
        public readonly ItemBox Deficit = new ItemBox();
        // 输出类型
        public readonly ItemType OutputType;
        public readonly Factory OutputFactory;
        public Item Output
        {
            get
            {
                return new Item(_links
                .FindAll(link => link.Item.Type == OutputType)
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

        public void AddDeficit(ItemType type, int number)
        {
            Deficit.AddItem(type, number);
        }
    }
}