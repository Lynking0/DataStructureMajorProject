using Godot;
using System;
using System.Collections.Generic;

namespace Shared.Charts
{
    public partial class ScatterChart : Chart
    {
        public Vector2[]? Data;
        public int[] maxAndMin;
        public Color LineColor = Colors.WebGray;
        public int[] maxValue(int[] num)
        {
            int[] arr = { num[0], num[0] };
            for (int i = 1; i < num.Length; i++)
            {
                arr[0] = arr[0] > num[i] ? arr[0] : num[i];
                arr[1] = arr[1] < num[i] ? arr[1] : num[i];
            }
            return arr;
        }
        public override void _Ready()
        {
            // Data = new Vector2[] {
            //     new Vector2(75, 150),
            //     new Vector2(125, 90),
            //     new Vector2(175, 250),
            //     new Vector2(225, 210),
            //     new Vector2(275, 220),
            //     new Vector2(325, 160),
            //     new Vector2(375, 130),
            //     new Vector2(425, 130),
            //     new Vector2(475, 60),
            // };
            Data = new Vector2[] {
                new Vector2(75, 200),
                new Vector2(125, 290),
                new Vector2(175, 1250),
                new Vector2(225, 1210),
                new Vector2(275, 2270),
                new Vector2(325, 160),
                new Vector2(375, 130),
                new Vector2(425, 350),
                new Vector2(475, 900),
            };
            int[] arr = new int[Data!.Length];
            for (int i = 0; i < Data!.Length; i++) { arr[i] = (int)Data[i].Y; }
            maxAndMin = maxValue(arr);
        }
        public override void _Draw()
        {
            base._Draw();
            Font font = ResourceLoader.Load<FontFile>("res://Render/PingFang-SC-Regular.ttf");

            int maxl = maxAndMin[0].ToString().Length;
            int max = (maxAndMin[0] - maxAndMin[0] % (int)Math.Pow(10, maxl - 2)) + (int)Math.Pow(10, maxl - 2);
            int step = (int)max / 4;

            for (int i = 0; i < 5; i++)
            {
                DrawLine(new Vector2(50, 250 - i * 50), new Vector2(50 + Data!.Length * 50, 250 - i * 50), Colors.WebGray);
                DrawString(font, new Vector2(40 - 8 * (i * step).ToString().Length, 255 - 50 * i), (i * step).ToString(), fontSize: 14);
            }
            for (int i = 0; i < 10; i++)
            {
                if (i == 0) DrawLine(new Vector2(50 + 50 * i, 25), new Vector2(50 + 50 * i, 250), Colors.WebGray);
                else DrawLine(new Vector2(50 + 50 * i, 50), new Vector2(50 + 50 * i, 250), Colors.WebGray);
            }
            for (int i = 0; i < Data!.Length; i++)
            {
                Vector2 v = new Vector2(Data[i].X, 250 - 200 * Data[i].Y / max);
                // DrawCircle(Data[i],5, LineColor);
                DrawCircle(v, 5, LineColor);
            }
        }
    }
}