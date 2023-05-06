using GraphMoudle;

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
}