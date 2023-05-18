using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace TransportMoudle
{
    public partial class Train
    {
        private const int TickChunkCount = 4;
        private static List<Train>[] TrainTickGroup = new List<Train>[TickChunkCount];
        private static void TickSomeTrain(object? ts)
        {
            lock (TrainTickGroup)
            {
                if (ts is List<Train> trains)
                {
                    foreach (var factory in trains)
                    {
                        factory.Tick();
                    }
                }
            }
        }
        public static void TickAll()
        {
            lock (TrainTickGroup)
            {
                foreach (var item in TrainTickGroup)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(TickSomeTrain), item);
                }
            }
        }
    }
}