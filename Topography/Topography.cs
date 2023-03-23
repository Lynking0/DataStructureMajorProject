using Godot;
using System;
using GraphInformation;
using GraphInformation.DoubleVector2Extensions;

namespace Topography
{
    public partial class Topography : Node2D
    {
        public override void _Ready()
        {
            FractalNoiseGenerator.Seed = -0x1a41c0e9;
            FractalNoiseGenerator.Frequency = 1 / 120.0;
            FractalNoiseGenerator.Amplitude = 1 / 2.5;
            FractalNoiseGenerator.Octaves = 8;
            FractalNoiseGenerator.PingPongStrength = 2;
            FractalNoiseGenerator.WeightedStrength = 0;
            FractalNoiseGenerator.Lacunarity = 2.2;
            FractalNoiseGenerator.Gain = 0.56;
            FractalNoiseGenerator.BottomNumber = 25;
            RenderingServer.GlobalShaderParameterSet("Seed", FractalNoiseGenerator.Seed);
            RenderingServer.GlobalShaderParameterSet("Frequency", FractalNoiseGenerator.Frequency);
            RenderingServer.GlobalShaderParameterSet("Amplitude", FractalNoiseGenerator.Amplitude);
            RenderingServer.GlobalShaderParameterSet("Octaves", FractalNoiseGenerator.Octaves);
            RenderingServer.GlobalShaderParameterSet("PingPongStrength", FractalNoiseGenerator.PingPongStrength);
            RenderingServer.GlobalShaderParameterSet("WeightedStrength", FractalNoiseGenerator.WeightedStrength);
            RenderingServer.GlobalShaderParameterSet("Lacunarity", FractalNoiseGenerator.Lacunarity);
            RenderingServer.GlobalShaderParameterSet("Gain", FractalNoiseGenerator.Gain);
            RenderingServer.GlobalShaderParameterSet("BottomNumber", FractalNoiseGenerator.BottomNumber);
            RenderingServer.GlobalShaderParameterSet("LevelCnt", 0);

            Graph.Instance.PossionDiskSample();
            GD.Print(Graph.Instance.vertices.Count);
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton eventMouseButton)
            {
                if ((int)eventMouseButton.ButtonIndex == 1 && (int)eventMouseButton.ButtonMask == 1)
                {
                    Vector2 clickPos = eventMouseButton.Position;
                    double gradX, gradY;
                    FractalNoiseGenerator.GetFractalNoise(clickPos.X, clickPos.Y, out gradX, out gradY);
                    GD.Print((gradX, gradY));
                }
            }
        }

        // private FractalNoiseGenerator noise;
        // public int HorCellNum = 1000;
        // public int VerCellNum = 580;
        // public float LevelCnt = 8.0f;
        // public override void _Ready()
        // {
        //     GD.Randomize();
        //     noise = new FractalNoiseGenerator(637);
        //     noise.Frequency = 1 / 150.0;
        //     noise.Octaves = 8;
        //     noise.WeightedStrength = -0.08;
        //     noise.Lacunarity = 2.2;
        //     noise.Gain = 0.56;
        //     noise.Amplitude = 1 / 2.5;
        // }
        // public override void _Draw()
        // {
        // 	const int size = 1;
        //     for (int y = 0; y < VerCellNum; y += size)
        //     {
        //         for (int x = 0; x < HorCellNum; x += size)
        //         {
        //             float curNoiseVal = (float)noise.GetFractalNoise(x, y);
        //             curNoiseVal = (1 - curNoiseVal) * 0.7f;
        //             curNoiseVal = (int)(curNoiseVal * LevelCnt) / LevelCnt;
        //             base.DrawRect(new Rect2(new Vector2(x, y), new Vector2(size, size)), new Color((float)curNoiseVal, curNoiseVal, curNoiseVal));
        //         }
        //     }
        // }
    }
}