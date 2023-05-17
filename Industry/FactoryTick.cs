using System.Collections.Generic;
using System.Threading;
using System.Linq;
namespace IndustryMoudle
{
    public partial class Factory
    {
        private const int TickChunkCount = 4;
        private static List<Factory>[] FactoryTickGroup = new List<Factory>[TickChunkCount];
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
            foreach (var item in FactoryTickGroup)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(TickSomeFactory), item);
            }
        }
    }
}