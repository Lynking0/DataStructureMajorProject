using System.Collections.Generic;


namespace Industry
{
    public partial class Factory
    {
        private static List<Factory> _Factories = new List<Factory>();
        public static IReadOnlyCollection<Factory> Factories => _Factories;
    }
}