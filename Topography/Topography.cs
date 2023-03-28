using Godot;
using System;
using NetworkGraph;
using Shared.Extensions.DoubleVector2Extensions;

namespace Topography
{
    public partial class Topography : Node2D
    {
        public override void _Ready()
        {
            FractalNoiseGenerator.Seed = 0;
            FractalNoiseGenerator.Frequency = 1 / 200.0;
            FractalNoiseGenerator.Amplitude = 1 / 2.5;
            FractalNoiseGenerator.Octaves = 5;
            FractalNoiseGenerator.PingPongStrength = 2;
            FractalNoiseGenerator.Lacunarity = 2.2;
            FractalNoiseGenerator.Gain = 0.56;
            FractalNoiseGenerator.BottomNumber = 30;
            FractalNoiseGenerator.PseudorandomGenerator.Precalculate(Graph.MinX, Graph.MinY, Graph.MaxX, Graph.MaxY);
            RenderingServer.GlobalShaderParameterSet("Seed", FractalNoiseGenerator.Seed);
            RenderingServer.GlobalShaderParameterSet("Frequency", FractalNoiseGenerator.Frequency);
            RenderingServer.GlobalShaderParameterSet("Amplitude", FractalNoiseGenerator.Amplitude);
            RenderingServer.GlobalShaderParameterSet("Octaves", FractalNoiseGenerator.Octaves);
            RenderingServer.GlobalShaderParameterSet("PingPongStrength", FractalNoiseGenerator.PingPongStrength);
            RenderingServer.GlobalShaderParameterSet("Lacunarity", FractalNoiseGenerator.Lacunarity);
            RenderingServer.GlobalShaderParameterSet("Gain", FractalNoiseGenerator.Gain);
            RenderingServer.GlobalShaderParameterSet("BottomNumber", FractalNoiseGenerator.BottomNumber);
            RenderingServer.GlobalShaderParameterSet("LevelCnt", 0);

            Graph.Instance.PossionDiskSample();
            Graph.Instance.CreateEdges();
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
        public override void _Draw()
        {
            // for (int x = 0; x <= 1152; ++x)
            // {
            //     for (int y = 0; y <= 648; ++y)
            //     {
            //         float v = (float)FractalNoiseGenerator.GetFractalNoise(x, y);
            //         DrawRect(new Rect2(new Vector2(x, y), new Vector2(1, 1)),new Color(
            //             Mathf.Lerp(205.0f, 255.0f, v) / 256f,
            //             Mathf.Lerp(218.0f, 123.0f, v) / 256f,
            //             Mathf.Lerp(134.0f, 0.0f, v) / 256f,
            //         1));
            //     }
            // }
        }
    }
}