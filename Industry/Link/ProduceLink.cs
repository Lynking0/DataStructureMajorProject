using System.Collections.Generic;
using GraphMoudle;
using IndustryMoudle.Entry;

namespace IndustryMoudle.Link
{
    public struct ProduceLink
    {
        public Factory From;
        public Factory To;
        public Item Item;
        public readonly ProduceChain Chain;
        public static List<ProduceLink> Links = new List<ProduceLink>();
        private List<Vertex> _vertexes;
        public IReadOnlyCollection<Vertex> Vertexes => _vertexes;

        public ProduceLink(Factory from, Factory to, IEnumerable<Vertex> vertexs, Item item, ProduceChain chain)
        {
            From = from;
            To = to;
            Item = item;
            Chain = chain;
            _vertexes = new List<Vertex>(vertexs);
            Links.Add(this);
        }

        public static implicit operator string(ProduceLink link)
        {
            return $"{(string)link.Item.Type} x {link.Item.Number}";
        }
    }
}