using Godot;
using System;
using System.Collections.Generic;

namespace Shared.Charts
{
    public partial class BarChart : Chart
    {
        public int[]? Data;
        public int[] maxAndMin;
        public Color LineColor = Colors.WebGray;
        public int[] maxValue(int[] num)
        {
            int[] arr = {num[0],num[0]};
            for (int i = 1; i < num.Length; i++)
            {
                arr[0] = arr[0] > num[i] ? arr[0] : num[i];
                arr[1] = arr[1] < num[i] ? arr[1] : num[i];
            }
            return arr;
        }
        public override void _Ready()
        {
            // Data = new int[] {
            //     25,
            //     75,
            //     125,
            //     175,
            //     100,
            //     20,
            //     185,
            //     215,
            //     55
            // };
            Data = new int[] {
                129,
                275,
                1225,
                175,
                20,
                600,
                185,
                215,
                900
            };
            maxAndMin = maxValue(Data);
        }
        public override void _Draw()
        {
            base._Draw();
            Font font = ResourceLoader.Load<FontFile>("res://Render/PingFang-SC-Regular.ttf");

            int maxl = maxAndMin[0].ToString().Length;
            int max = (maxAndMin[0] - maxAndMin[0] % (int)Math.Pow(10,maxl-2)) + (int)Math.Pow(10,maxl-2);
            int step = (int)max/4;

            for (int i = 0; i < 5; i++)
            {
                DrawLine(new Vector2(50, 250 - i * 50), new Vector2(55 + Data!.Length * 50, 250 - i * 50), Colors.WebGray);
                DrawString(font, new Vector2(40 - 8*(i * step).ToString().Length, 255 - 50 * i), (i * step).ToString(), fontSize: 14);
            }
            DrawLine(new Vector2(50, 250), new Vector2(50, 25), Colors.WebGray);
            for (int i = 0; i < Data!.Length; i++)
            {
                int h = 200*Data[i]/max;
                DrawRect(new Rect2(60 + 50 * i, 250 - h, 25, h), Colors.Green);
                double x = (60 + 50*i + 12.5) - 8*(Data[i]).ToString().Length/2;
                DrawString(font, new Vector2((int)x, 240 - h), (Data[i]).ToString(), fontSize: 14);
            }
        }
    }
}