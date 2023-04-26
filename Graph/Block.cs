using System;
using System.Collections.Generic;

namespace GraphMoudle
{
    public class Block
    {
        public int Count => Vertices.Count;
        public List<Vertex> Vertices;
        public Block()
        {
            Vertices = new List<Vertex>();
        }
        public Block(params Vertex[] vertices)
        {
            Vertices = new List<Vertex>(vertices);
        }
        public Block(IEnumerable<Vertex> vertices)
        {
            Vertices = new List<Vertex>(vertices);
        }
    }
}