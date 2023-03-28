using Godot;
using GraphInformation;

namespace Topography
{
    public partial class Topography
    {
        public static void InitParams()
        {
            FractalNoiseGenerator.Seed = 0x1b0a6fe7;
            FractalNoiseGenerator.Frequency = 1 / 180.0;
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
            RenderingServer.GlobalShaderParameterSet("LevelCnt", 10);
        }

        public static void Generate()
        {
            Graph.Instance.PossionDiskSample();
            Graph.Instance.CreateEdges();
        }
    }
}