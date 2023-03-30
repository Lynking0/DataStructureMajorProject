using Godot;
using System;
using System.Collections.Generic;

namespace Shared.Charts
{
    public partial class LineChart : Chart
    {
        public Vector2[]? Data;
        public Color LineColor = Colors.WebGray;

        public override void _Ready()
        {
            Data = new Vector2[] {
                new Vector2(25, 50),
                new Vector2(75, 150),
                new Vector2(125, 90),
                new Vector2(175, 250),
                new Vector2(225, 210),
                new Vector2(275, 270),
                new Vector2(325, 160),
                new Vector2(375, 130),
                new Vector2(425, 130),
                new Vector2(475, 20),
            };
        }

        public override void _Draw()
        {
            base._Draw();
            for (int i = 1; i < Data!.Length; i++)
            {
                Vector2 from = new Vector2(Data[i - 1].X, Height - Data[i - 1].Y);
                Vector2 to = new Vector2(Data[i].X, Height - Data[i].Y);
                DrawLine(from, to, LineColor);
            }
        }
    }
}