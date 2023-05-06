using System.Collections.Generic;
using System.Linq;
using GraphMoudle;
using IndustryMoudle.Entry;

namespace IndustryMoudle.Link
{
    public struct LoadInfo
    {
        public int ForwardLoad { get; }
        public int ReverseLoad { get; }
        public int TotalLoad => ForwardLoad + ReverseLoad;

        public LoadInfo(int forwardLoad, int reverseLoad)
        {
            ForwardLoad = forwardLoad;
            ReverseLoad = reverseLoad;
        }
    }

    public struct ProduceLink
    {
        public Factory From;
        public Factory To;
        public Item Item;
        public readonly ProduceChain Chain;
        public static List<ProduceLink> Links = new List<ProduceLink>();
        private static readonly Dictionary<Edge, List<(ProduceLink link, bool reverse)>> _edgeToLinks = new Dictionary<Edge, List<(ProduceLink link, bool reverse)>>();
        public static IReadOnlyDictionary<Edge, List<(ProduceLink link, bool reverse)>> EdgeToLinks => _edgeToLinks;

        private List<Vertex> _vertexes;
        public IReadOnlyList<Vertex> Vertexes => _vertexes;
        private List<EdgeInfo> _edgeInfos;
        public IReadOnlyList<EdgeInfo> EdgeInfos => _edgeInfos;

        public static LoadInfo GetEdgeLoad(Edge edge)
        {
            if (!EdgeToLinks.TryGetValue(edge, out var linkInfos))
            {
                return new LoadInfo(0, 0);
            }
            var reverseLoad = linkInfos.Where(info => info.reverse).Sum(info => info.link.Item.Number);
            var forwardLoad = linkInfos.Where(info => !info.reverse).Sum(info => info.link.Item.Number);
            return new LoadInfo(forwardLoad, reverseLoad);
        }

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
                    _edgeToLinks[edgeIfno.Edge].Add((this, edgeIfno.Reverse));
                }
                else
                {
                    _edgeToLinks.Add(edgeIfno.Edge, new List<(ProduceLink link, bool reverse)>() { (this, edgeIfno.Reverse) });
                }
            }
            Links.Add(this);
        }

        // 销毁此产线并解除两端工厂绑定
        public void Destroy()
        {
            foreach (var (edge, reverse) in _edgeInfos)
            {
                if (edge is null)
                    return;
                _edgeToLinks[edge].Remove((this, reverse));
            }
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