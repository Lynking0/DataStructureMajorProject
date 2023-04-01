using Godot;
using System;
using System.Collections.Generic;

namespace Shared.Charts
{
    public partial class ScatterChart : Chart
    {
        public Vector2[]? Data;
        public Color LineColor = Colors.WebGray;
        public override void _Ready()
        {
            Data = new Vector2[] {
                new Vector2(75, 150),
                new Vector2(125, 90),
                new Vector2(175, 250),
                new Vector2(225, 210),
                new Vector2(275, 220),
                new Vector2(325, 160),
                new Vector2(375, 130),
                new Vector2(425, 130),
                new Vector2(475, 60),
            };
        }
        public override void _Draw()
        {
            base._Draw();
            Font font = ResourceLoader.Load<FontFile>("res://Render/PingFang-SC-Regular.ttf");
            for(int i=0;i<5;i++)
            {
                DrawLine(new Vector2(50,250-i*50), new Vector2(50+Data!.Length*50,250-i*50), Colors.WebGray);
                DrawString(font, new Vector2(10, 255-50*i),(i*50).ToString(),fontSize:14);
            }
            for(int i=0;i<10;i++){
                if(i==0) DrawLine(new Vector2(50+50*i,25), new Vector2(50+50*i,250), Colors.WebGray); 
                else DrawLine(new Vector2(50+50*i,50), new Vector2(50+50*i,250), Colors.WebGray);
            }
            for (int i = 0; i < Data!.Length; i++)
            {
                DrawCircle(Data[i],5, LineColor);
            }
        }
    }
}