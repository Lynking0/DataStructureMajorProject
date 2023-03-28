using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using Shared.Extensions.DoubleVector2Extensions;

namespace NetworkGraph.DataStructureAndAlgorithm.OptimalCombinationAlgorithm.ComputeShader
{
    public static class EdgeEvaluatorInvoker
    {
        private static RenderingDevice? rd;
        private static Rid shader; 
        private static Rid[]? Buffers;
        public static List<(Vector2D a, Vector2D b, Vector2D c, Vector2D d)>? Data;
        public static void Init()
        {
            // Create a local rendering device.
            rd = RenderingServer.CreateLocalRenderingDevice();
            // Load GLSL shader
            RDShaderFile shaderFile = GD.Load<RDShaderFile>("res://NetworkGraph/DataStructureAndAlgorithm/OptimalCombinationAlgorithm/ComputeShader/EdgeEvaluator.glsl");
            RDShaderSpirV shaderBytecode = shaderFile.GetSpirV();
            Rid shader = rd.ShaderCreateFromSpirV(shaderBytecode);
        }
        public static void Invoke()
        {
            float[] input0 = new float[]
            {
                (float)Topography.FractalNoiseGenerator.Seed,
                (float)Topography.FractalNoiseGenerator.Frequency,
                (float)Topography.FractalNoiseGenerator.Amplitude,
                (float)Topography.FractalNoiseGenerator.Octaves,
                (float)Topography.FractalNoiseGenerator.PingPongStrength,
                (float)Topography.FractalNoiseGenerator.Lacunarity,
                (float)Topography.FractalNoiseGenerator.Gain,
                (float)Topography.FractalNoiseGenerator.BottomNumber
            };
            float[] input10 = new float[]
            {
                (float)Math.PI,
                (float)NetworkGraph.Graph.MaxVertexAltitude,
                (float)NetworkGraph.Graph.CtrlPointDistance,
                (float)Data!.Count
            };
            float[] input1 = new float[Data.Count];
            float[] input2 = new float[Data.Count];
            float[] input3 = new float[Data.Count];
            float[] input4 = new float[Data.Count];
            float[] input5 = new float[Data.Count];
            float[] input6 = new float[Data.Count];
            float[] input7 = new float[Data.Count];
            float[] input8 = new float[Data.Count];
            uint[] input9 = new uint[Data.Count];
            bool[] input11 = new bool[Data.Count];
            for (int i = 0; i < Data.Count; ++i)
            {
                input1[i] = (float)Data[i].a.X;
                input2[i] = (float)Data[i].a.Y;
                input3[i] = (float)Data[i].b.X;
                input4[i] = (float)Data[i].b.Y;
                input5[i] = (float)Data[i].c.X;
                input6[i] = (float)Data[i].c.Y;
                input7[i] = (float)Data[i].d.X;
                input8[i] = (float)Data[i].d.Y;
                input9[i] = GD.Randi();
            }

            byte[][] inputBytes = new byte[12][]
            {
                new byte[input0.Length * sizeof(float)],
                new byte[input1.Length * sizeof(float)],
                new byte[input2.Length * sizeof(float)],
                new byte[input3.Length * sizeof(float)],
                new byte[input4.Length * sizeof(float)],
                new byte[input5.Length * sizeof(float)],
                new byte[input6.Length * sizeof(float)],
                new byte[input7.Length * sizeof(float)],
                new byte[input8.Length * sizeof(float)],
                new byte[input9.Length * sizeof(uint)],
                new byte[input10.Length * sizeof(float)],
                new byte[input11.Length * sizeof(bool)]
            };
            Buffer.BlockCopy(input0, 0, inputBytes[0], 0, inputBytes[0].Length);
            Buffer.BlockCopy(input1, 0, inputBytes[1], 0, inputBytes[1].Length);
            Buffer.BlockCopy(input2, 0, inputBytes[2], 0, inputBytes[2].Length);
            Buffer.BlockCopy(input3, 0, inputBytes[3], 0, inputBytes[3].Length);
            Buffer.BlockCopy(input4, 0, inputBytes[4], 0, inputBytes[4].Length);
            Buffer.BlockCopy(input5, 0, inputBytes[5], 0, inputBytes[5].Length);
            Buffer.BlockCopy(input6, 0, inputBytes[6], 0, inputBytes[6].Length);
            Buffer.BlockCopy(input7, 0, inputBytes[7], 0, inputBytes[7].Length);
            Buffer.BlockCopy(input8, 0, inputBytes[8], 0, inputBytes[8].Length);
            Buffer.BlockCopy(input9, 0, inputBytes[9], 0, inputBytes[9].Length);
            Buffer.BlockCopy(input10, 0, inputBytes[10], 0, inputBytes[10].Length);
            Buffer.BlockCopy(input11, 0, inputBytes[11], 0, inputBytes[11].Length);

            Buffers = new Rid[12];
            Array<RDUniform> uniforms = new Array<RDUniform>();
            for (int i = 0; i < inputBytes.Length; ++i)
            {
                Buffers[i] = rd!.StorageBufferCreate((uint)inputBytes[i].Length, inputBytes[i]);
                RDUniform uniform = new RDUniform
                {
                    UniformType = RenderingDevice.UniformType.StorageBuffer,
                    Binding = i
                };
                uniform.AddId(Buffers[i]);
                uniforms.Add(uniform);
            }
            Rid uniformSet = rd!.UniformSetCreate(uniforms, shader, 0);

            // Create a compute pipeline
            Rid pipeline = rd.ComputePipelineCreate(shader);
            long computeList = rd.ComputeListBegin();
            rd.ComputeListBindComputePipeline(computeList, pipeline);
            rd.ComputeListBindUniformSet(computeList, uniformSet, 0);
            rd.ComputeListDispatch(computeList, xGroups: 5, yGroups: 1, zGroups: 1);
            rd.ComputeListEnd();
            // Submit to GPU and wait for sync
            rd.Submit();
        }
        public static bool[] Receive()
        {
            rd!.Sync();
            // Read back the data from the buffers
            byte[] outputBytes = rd.BufferGetData(Buffers![11]);
            bool[] output = new bool[Data!.Count];
            Buffer.BlockCopy(outputBytes, 0, output, 0, outputBytes.Length);
            return output;
        }
    }
}