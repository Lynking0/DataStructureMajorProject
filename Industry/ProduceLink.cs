using System.Collections.Generic;

namespace Industry
{
    public struct ProduceLink
    {
        public Factory from;
        public Factory to;
        public Item item;
        public static List<ProduceLink> Links = new List<ProduceLink>();

        public ProduceLink(Factory from, Factory to, Item item)
        {
            this.from = from;
            this.to = to;
            this.item = item;
            Links.Add(this);
        }
    }
}