using Godot;
using System;
using System.Collections.Generic;

namespace Shared.Charts
{
    public partial class BarChart : Chart
    {
        public int[]? Data;
        public Color LineColor = Colors.WebGray;

        public override void _Ready()
        {
            // 最高200
            Data = new int[] {
                25,
                75,
                125,
                175,
                100,
                20,
                185,
                215,
                55
            };
        }

        public override void _Draw()
        {
            base._Draw();
            Font font = ResourceLoader.Load<FontFile>("res://Render/PingFang-SC-Regular.ttf");
            for(int i=0;i<5;i++)
            {
                DrawLine(new Vector2(50,250-i*50), new Vector2(55+Data!.Length*50,250-i*50), Colors.WebGray);
                DrawString(font, new Vector2(10, 255-50*i),(i*50).ToString(),fontSize:14);
            }
            DrawLine(new Vector2(50,250), new Vector2(50,25), Colors.WebGray);
            for (int i = 0; i < Data!.Length; i++)
            {
                DrawRect(new Rect2(60+50*i, 250-Data[i], 25, Data[i]), Colors.Green);
                DrawString(font, new Vector2(60+50*i, 240-Data[i]),(Data[i]).ToString(),fontSize:14);
            }
        }
    }
}