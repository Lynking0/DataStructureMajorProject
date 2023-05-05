using Godot;
using GraphMoudle;

namespace TopographyMoudle
{
    public partial class Topography
    {
        public static void InitParams()
        {
            FractalNoiseGenerator.Seed = 1054;
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
            RenderingServer.GlobalShaderParameterSet("LevelCnt", 10);
            var c = (Color)ProjectSettings.GetSetting("rendering/environment/defaults/default_clear_color");
            RenderingServer.GlobalShaderParameterSet("DefaultClearColor", new Vector4(c.R, c.G, c.B, c.A));
        }

        public static void Generate()
        {
            Graph.Instance.CreateVertices();
            Graph.Instance.CreateEdges();
        }
    }
}