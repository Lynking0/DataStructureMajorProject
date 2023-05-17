using System.Collections.Generic;
using System.Threading;
using System.Linq;
namespace IndustryMoudle
{
    public partial class Factory
    {
        private static int TickChunkSize = -1;
        private static void TickSomeFactory(object? fs)
        {
            if (fs is IEnumerable<Factory> factories)
                foreach (var factory in factories)
                {
                    factory.Tick();
                }
        }
        public static void TickAll()
        {
            if (TickChunkSize == -1)
            {
                TickChunkSize = Factories.Count / 4 + 1;
                ThreadPool.SetMinThreads(4, 4);
            }
            foreach (var item in Factories.Chunk(TickChunkSize))
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(TickSomeFactory), item);
            }
        }
    }
}