using System.Collections.Generic;
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

        public ProduceLink(Factory from, Factory to, Item item, ProduceChain chain)
        {
            From = from;
            To = to;
            Item = item;
            Chain = chain;
            Links.Add(this);
        }

        public static implicit operator string(ProduceLink link)
        {
            return $"{(string)link.Item.Type} x {link.Item.Number}";
        }
    }
}