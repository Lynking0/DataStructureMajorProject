using System.Diagnostics;
using IndustryMoudle.Extensions;
using GraphMoudle;

namespace TransportMoudle
{
    [DebuggerDisplayAttribute("CommandLine: {DebuggerDisplay}")]
    public struct Trip
    {
        public TrainLine Line;
        public Vertex Start;
        public Vertex End;
        public string DebuggerDisplay
        {
            get
            {
                return $"{Start.GetFactory()!.ID} -> {End.GetFactory()!.ID} by {Line.ID}";
            }
        }
    }
}