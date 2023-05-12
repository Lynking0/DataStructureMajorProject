using System.Linq;
using System.Collections.Generic;
using GraphMoudle;
using IndustryMoudle.Link;

namespace TransportMoudle
{
    public class Ticket
    {
        private Vertex From;
        private Vertex To;
        public List<Trip> Trips = new List<Trip>();
        public Ticket(ProduceLink link)
        {
            From = link.From.Vertex;
            To = link.To.Vertex;
            var vertices = link.Vertexes;
            Trips = TrainLine.Navigate(link.Vertexes.ToArray());
        }
    }
}