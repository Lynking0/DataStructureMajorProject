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
        private static readonly Dictionary<Edge, List<ProduceLink>> _edgeToLinks = new Dictionary<Edge, List<ProduceLink>>();
        public static IReadOnlyDictionary<Edge, List<ProduceLink>> EdgeToLinks => _edgeToLinks;

        private List<Vertex> _vertexes;
        public IReadOnlyCollection<Vertex> Vertexes => _vertexes;
        private List<EdgeInfo> _edgeInfos;
        public IReadOnlyCollection<EdgeInfo> EdgeInfos => _edgeInfos;

        public ProduceLink(Factory from, Factory to, IEnumerable<Vertex> vertexs, IEnumerable<EdgeInfo> edges, Item item, ProduceChain chain)
        {
            From = from;
            To = to;
            Item = item;
            Chain = chain;
            _vertexes = new List<Vertex>(vertexs);
            _edgeInfos = new List<EdgeInfo>(edges);
            foreach (var edgeIfno in edges)
            {
                if (edgeIfno.Edge == null)
                    continue;
                if (_edgeToLinks.ContainsKey(edgeIfno.Edge))
                {
                    _edgeToLinks[edgeIfno.Edge].Add(this);
                }
                else
                {
                    _edgeToLinks.Add(edgeIfno.Edge, new List<ProduceLink>() { this });
                }
            }
            Links.Add(this);
        }

        // 销毁此产线并解除两端工厂绑定
        public void Destroy()
        {
            Links.Remove(this);
            From.RemoveOutputLink(this);
            To.RemoveInputLink(this);
        }

        public static implicit operator string(ProduceLink link)
        {
            return $"{(string)link.Item.Type} x {link.Item.Number}";
        }
    }
}