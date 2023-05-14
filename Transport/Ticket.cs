using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using GraphMoudle;
using IndustryMoudle.Link;
using IndustryMoudle.Extensions;

namespace TransportMoudle
{
    [DebuggerDisplayAttribute("CommandLine: {DebuggerDisplay}")]
    public class Ticket
    {
        public string DebuggerDisplay
        {
            get
            {
                var r = new List<string>();
                foreach (var trip in Trips)
                {
                    r.Add($"{trip.Start.GetFactory()!.ID} -> {trip.End.GetFactory()!.ID} by {trip.Line.ID}");
                }
                return string.Join(" ", r);
            }
        }
        private Vertex From;
        private Vertex To;
        public List<Trip> Trips = new List<Trip>();
        private int TripIndex = 0;
        public Trip CurTrip => Trips[TripIndex];
        public Ticket(ProduceLink link)
        {
            From = link.From.Vertex;
            To = link.To.Vertex;
            var vertices = link.Vertexes;
            Trips = TrainLine.Navigate(link.Vertexes.ToArray());
        }
        /// <summary>
        /// 到达一个站点
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns>是否结束一段Trip</returns>
        public bool Arrive(Vertex vertex)
        {
            if (vertex == CurTrip.End)
            {
                if (TripIndex < Trips.Count - 1)
                {
                    TripIndex++;
                }
                return true;
            }
            return false;
        }
    }
}