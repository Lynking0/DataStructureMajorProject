using System.Collections.Generic;
using IndustryMoudle.Entry;

namespace IndustryMoudle.Link
{
    public partial class ProduceChain
    {
        private static List<ProduceChain> _chains = new List<ProduceChain>();
        public static IReadOnlyCollection<ProduceChain> Chains => _chains;

        public static int ConsumeCount
        {
            get
            {
                int result = 0;
                foreach (ProduceChain chain in _chains)
                {
                    result += chain.Output.Number;
                }
                return result;
            }
        }
    }
}