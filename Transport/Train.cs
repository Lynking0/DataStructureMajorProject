using Godot;
using System.Collections.Generic;
using System.Linq;
namespace TransportMoudle
{
    public partial class Train : Node2D
    {
        private static int IDCount = 0;
        public readonly string ID = IDCount++.ToString();
        public static List<Train> Trains = new List<Train>();
        public const int CarriageCount = 2;
        public TrainLine TrainLine { get; private set; }
        public Path2D Path => TrainLine.Path;
        public List<Carriage> Carriages = new List<Carriage>();
        public Vector2 TrainPosition => Carriages.First().Position;

        public int Size
        {
            get => Carriages.First().Size;
            set
            {
                foreach (var carriage in Carriages)
                {
                    carriage.Size = value;
                    carriage.QueueRedraw();
                }
            }
        }

        public Train(TrainLine trainLine)
        {
            TrainLine = trainLine;
            for (int i = 0; i < CarriageCount; i++)
            {
                var carriage = new Carriage();
                carriage.ProgressRatio = 0.4f * i;
                carriage.Loop = true;
                Path.AddChild(carriage);
                Carriages.Add(carriage);
            }
            Trains.Add(this);
        }

        public void Tick()
        {
            // foreach (var carriage in Carriages)
            // {
            //     carriage.Progress += 50;
            // }
        }
    }
}