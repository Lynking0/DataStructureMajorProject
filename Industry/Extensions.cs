using GraphMoudle;
using IndustryMoudle.Link;

namespace IndustryMoudle.Extensions
{
    public static class _Vertex
    {
        public static Factory? GetFactory(this Vertex vertex)
        {
            if (!Factory.VertexToFactory.ContainsKey(vertex))
                return null;
            return Factory.VertexToFactory[vertex];
        }
    }

    public static class _Edge
    {
        public static LoadInfo GetLoadInfo(this Edge edge)
        {
            return ProduceLink.GetEdgeLoad(edge);
        }
    }
}