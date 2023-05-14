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
        public const int CarriageCount = 1;
        public TrainLine TrainLine { get; private set; }
        public Path2D Path => TrainLine.Path;
        public List<Carriage> Carriages = new List<Carriage>();
        public Vector2 TrainPosition => Carriages.First().Position;
        private int StopTickCount = 0;
        private const int StopTick = 50;
        private readonly int TrainSpeed = 1;
        private int CurSpeed;
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
                    GoodsCapacity = 5000; Size = 30; TrainSpeed = 8; break;
                case TrainLineLevel.SideLine:
                    GoodsCapacity = 2500; Size = 20; TrainSpeed = 4; break;
                case TrainLineLevel.FootPath:
                    GoodsCapacity = 800; Size = 15; TrainSpeed = 1; break;
            }
            CurSpeed = TrainSpeed;
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
                carriage.Progress += CurSpeed;
            }
            var p = Carriages.First().Progress;
            var nearStations = TrainLine.Station
                .Where(item => Math.Abs(p - item.Value) < Math.Abs(CurSpeed))
                .ToList();
            if (nearStations.Count() > 0)
            {
                var (vertex, length) = nearStations.First();
                if (LastStop == vertex)
                    return;
                LastStop = vertex;
                StopTickCount = StopTick;
                if (vertex == TrainLine.Vertexes.First())
                    CurSpeed = TrainSpeed;
                if (vertex == TrainLine.Vertexes.Last())
                    CurSpeed = -TrainSpeed;
                var factory = vertex.GetFactory()!;
                // unload goods
                for (int i = 0; i < Goodses.Count; i++)
                {
                    if (Goodses[i].Ticket.Arrive(vertex))
                    {
                        factory.LoadGoods(Goodses[i], this);
                        Goodses.RemoveAt(i);
                        i--;
                    }
                }
                // load goods
                var goodses = factory.OutputGoods(this, TrainLine, GoodsCapacity - GoodsCount);
                Goodses.AddRange(goodses);
                return;
            }
        }
    }
}