using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using Shared.Extensions.DoubleVector2Extensions;

namespace GraphMoudle.DataStructureAndAlgorithm.OptimalCombinationAlgorithm.ComputeShader
{
    public static class BridgePlannerInvoker
    {
        private static RenderingDevice? RD;
        private static Rid Shader;
        private static RDShaderFile? ShaderFile;
        private static RDShaderSpirV? ShaderBytecode;
        private static Rid[]? Buffers;
        public static List<(Vertex a, Vertex b, Vector2D aCtrl, Vector2D bCtrl)>? Data;
        public static void Init()
        {
            // Create a local rendering device.
            RD = RenderingServer.CreateLocalRenderingDevice();
            // Load GLSL shader
            ShaderFile = GD.Load<RDShaderFile>("res://Graph/DataStructureAndAlgorithm/OptimalCombinationAlgorithm/ComputeShader/BridgePlanner.glsl");
            ShaderBytecode = ShaderFile.GetSpirV();
            Shader = RD.ShaderCreateFromSpirV(ShaderBytecode);
        }
        public static void Invoke()
        {
            float[] input0 = new float[]
            {
                (float)TopographyMoudle.FractalNoiseGenerator.Seed,
                (float)TopographyMoudle.FractalNoiseGenerator.Frequency,
                (float)TopographyMoudle.FractalNoiseGenerator.Amplitude,
                (float)TopographyMoudle.FractalNoiseGenerator.Octaves,
                (float)TopographyMoudle.FractalNoiseGenerator.PingPongStrength,
                (float)TopographyMoudle.FractalNoiseGenerator.Lacunarity,
                (float)TopographyMoudle.FractalNoiseGenerator.Gain,
                (float)TopographyMoudle.FractalNoiseGenerator.BottomNumber
            };
            float[] input10 = new float[]
            {
                (float)Graph.MaxVertexAltitude,
                (float)Graph.CtrlPointDistance,
                (float)Data!.Count,
                (float)Graph.MinX,
                (float)Graph.MinY,
                (float)Graph.MaxX,
                (float)Graph.MaxY,
                (float)BridgePlanner.ToleranceLength
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
            float[] input11 = new float[Data.Count];
            float[] input12 = new float[Data.Count];
            float[] input13 = new float[Data.Count];
            float[] input14 = new float[Data.Count];
            for (int i = 0; i < Data.Count; ++i)
            {
                input1[i] = (float)Data[i].a.Position.X;
                input2[i] = (float)Data[i].a.Position.Y;
                input3[i] = (float)Data[i].b.Position.X;
                input4[i] = (float)Data[i].b.Position.Y;
                input5[i] = (float)Data[i].aCtrl.X;
                input6[i] = (float)Data[i].aCtrl.Y;
                input7[i] = (float)Data[i].bCtrl.X;
                input8[i] = (float)Data[i].bCtrl.Y;
                input9[i] = GD.Randi();
            }

            byte[][] inputBytes = new byte[15][]
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
                new byte[input11.Length * sizeof(float)],
                new byte[input12.Length * sizeof(float)],
                new byte[input13.Length * sizeof(float)],
                new byte[input14.Length * sizeof(float)]
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
            Buffer.BlockCopy(input12, 0, inputBytes[12], 0, inputBytes[12].Length);
            Buffer.BlockCopy(input13, 0, inputBytes[13], 0, inputBytes[13].Length);
            Buffer.BlockCopy(input14, 0, inputBytes[14], 0, inputBytes[14].Length);

            Buffers = new Rid[15];
            Array<RDUniform> uniforms = new Array<RDUniform>();
            for (int i = 0; i < inputBytes.Length; ++i)
            {
                Buffers[i] = RD!.StorageBufferCreate((uint)inputBytes[i].Length, inputBytes[i]);
                RDUniform uniform = new RDUniform
                {
                    UniformType = RenderingDevice.UniformType.StorageBuffer,
                    Binding = i
                };
                uniform.AddId(Buffers[i]);
                uniforms.Add(uniform);
            }
            Rid uniformSet = RD!.UniformSetCreate(uniforms, Shader, 0);

            // Create a compute pipeline
            Rid pipeline = RD.ComputePipelineCreate(Shader);
            long computeList = RD.ComputeListBegin();
            RD.ComputeListBindComputePipeline(computeList, pipeline);
            RD.ComputeListBindUniformSet(computeList, uniformSet, 0);
            RD.ComputeListDispatch(computeList, xGroups: (uint)Mathf.CeilToInt(Data.Count / 5), yGroups: 1, zGroups: 1);
            RD.ComputeListEnd();
            // Submit to GPU and wait for sync
            RD.Submit();
        }
        public static (float[], float[], float[], float[]) Receive()
        {
            RD!.Sync();

            byte[] outputBytes;

            outputBytes = RD.BufferGetData(Buffers![11]);
            float[] output1 = new float[Data!.Count];
            Buffer.BlockCopy(outputBytes, 0, output1, 0, outputBytes.Length);

            outputBytes = RD.BufferGetData(Buffers![12]);
            float[] output2 = new float[Data!.Count];
            Buffer.BlockCopy(outputBytes, 0, output2, 0, outputBytes.Length);

            outputBytes = RD.BufferGetData(Buffers![13]);
            float[] output3 = new float[Data!.Count];
            Buffer.BlockCopy(outputBytes, 0, output3, 0, outputBytes.Length);

            outputBytes = RD.BufferGetData(Buffers![14]);
            float[] output4 = new float[Data!.Count];
            Buffer.BlockCopy(outputBytes, 0, output4, 0, outputBytes.Length);

            return (output1, output2, output3, output4);
        }
    }
}