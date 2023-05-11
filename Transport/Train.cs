using Godot;
using System.Collections.Generic;
namespace TransportMoudle
{
    public class Train
    {
        private static int IDCount = 0;
        public readonly string ID = IDCount++.ToString();
        public static List<Train> Trains = new List<Train>();
        public const int CarriageCount = 3;
        public TrainLine TrainLine { get; private set; }
        public Path2D Path => TrainLine.Path;
        public List<PathFollow2D> Carriages = new List<PathFollow2D>();

        public Train(TrainLine trainLine)
        {
            TrainLine = trainLine;
            for (int i = 0; i < CarriageCount; i++)
            {
                var carriage = new PathFollow2D();
                carriage.Progress = 0.1f * i;
                carriage.Loop = true;
                Path.AddChild(carriage);
            }
            Trains.Add(this);
        }

        public void Tick()
        {
            foreach (var carriage in Carriages)
            {
                carriage.Progress += 0.01f;
            }
        }

    }
}