using System;
using Godot;
using IndustryMoudle.Entry;
using IndustryMoudle.Extensions;
using System.Collections.Generic;
using System.Linq;
using GraphMoudle;

namespace TransportMoudle
{
    public partial class Train : Node2D
    {
        private static int IDCount = 0;
        public readonly int ID = IDCount++;
        public static List<Train> Trains = new List<Train>();
        public const int CarriageCount = 2;
        public TrainLine TrainLine { get; private set; }
        public Path2D Path => TrainLine.Path;
        public List<Carriage> Carriages = new List<Carriage>();
        public Vector2 TrainPosition => Carriages.First().Position;
        private int StopTickCount = 0;
        private const int StopTick = 50;
        private int Speed = 2;
        private List<Goods> Goodses = new List<Goods>();
        public int GoodsCount => Goodses.Sum(g => g.Item.Number);
        private readonly int GoodsCapacity = 1000;
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
            switch (trainLine.Level)
            {
                case TrainLineLevel.MainLine:
                    GoodsCapacity = 5000; Size = 30; break;
                case TrainLineLevel.SideLine:
                    GoodsCapacity = 2500; Size = 20; break;
                case TrainLineLevel.FootPath:
                    GoodsCapacity = 800; Size = 15; break;
            }
            Trains.Add(this);
        }
        private Vertex? LastStop;
        public void Tick()
        {
            if (StopTickCount > 0)
            {
                StopTickCount--;
                return;
            }
            foreach (var carriage in Carriages)
            {
                carriage.Progress += Speed;
            }
            var p = Carriages.First().Progress;
            var nearStations = TrainLine.Station
                .Where(item => Math.Abs(p - item.Value) < Math.Abs(Speed))
                .ToList();
            if (nearStations.Count() > 0)
            {
                var (vertex, length) = nearStations.First();
                if (LastStop == vertex)
                    return;
                LastStop = vertex;
                StopTickCount = StopTick;
                if (vertex == TrainLine.Vertexes.First())
                    Speed = 8;
                if (vertex == TrainLine.Vertexes.Last())
                    Speed = -8;
                var factory = vertex.GetFactory()!;
                // unload goods
                for (int i = 0; i < Goodses.Count; i++)
                {
                    if (Goodses[i].Ticket.Arrive(vertex))
                    {
                        // if (TrainLine.ID == "F0")
                        //     GD.Print("Unload ", (string)Goodses[i].Item.Type, " ", Goodses[i].Item.Number, " to ", factory.ID);
                        factory.LoadGoods(Goodses[i], this);
                        Goodses.RemoveAt(i);
                        i--;
                    }
                }
                // load goods
                var goodses = factory.OutputGoods(this, TrainLine, GoodsCapacity - GoodsCount);
                goodses.AddRange(goodses);
                return;
            }
        }
    }
}