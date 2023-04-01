using Godot;
using System;
using System.Collections.Generic;

// https://echarts.apache.org/examples/zh/index.html
// https://docs.godotengine.org/en/stable/
// https://docs.godotengine.org/en/stable/tutorials/2d/custom_drawing_in_2d.html#introduction
// https://learn.microsoft.com/en-us/dotnet/csharp/


namespace Shared.Charts
{
    public partial class Chart : Node2D
    {
        public int Width = 500;
        public int Height = 300;
        public Color BackgroundColor = new Color(0, 0, 0, 0.2f);
        public override void _Draw()
        {
            DrawRect(new Rect2(0, 0, Width, Height), BackgroundColor);
        }
    }
}