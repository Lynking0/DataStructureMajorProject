using System.Collections.Generic;
using System.Linq;
using TransportMoudle;
using GraphMoudle;
using TransportMoudle.Extensions;

namespace IndustryMoudle.Link
{
    public partial class ProduceLink
    {
        private static Dictionary<ProduceLink, List<Trip>> NavigateCache = new Dictionary<ProduceLink, List<Trip>>();
        public List<Trip> NavigateTrips
        {
            get
            {
                lock (NavigateCache)
                {
                    if (NavigateCache.ContainsKey(this))
                    {
                        return NavigateCache[this];
                    }
                    var result = TrainLine.Navigate(Vertexes.ToList());
                    NavigateCache[this] = result;
                    return result;
                }
            }
        }
    }
}