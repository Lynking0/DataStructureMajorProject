using System;
using Godot;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
namespace Formula
{
    enum FactoryType
    {
        RawMaterial,
        Processing,
        TopLevel
    }
    class Factory
    {
        public int Id { get; }
        public FactoryType Type { get; }
        public string Material { get; }
        public string Result { get; set; }
        public string DependenciesNos { get; set; }
        public int height { get; set; } // 最大高度为5
        public Factory(int id, string material) // 用于原料厂
        {
            Id = id;
            Type = FactoryType.RawMaterial;
            Material = material;
            Result = material;
            DependenciesNos = "";
            height = 1;
        }
        public Factory(int id, FactoryType type, Factory[] dependencies) // 用于加工厂和顶级材料厂
        {
            Id = id;
            Type = type;
            Material = "";
            GenerateResult(dependencies);
        }
        IndustryMoudle.Recipe ToRecipe()
        {
            // TODO: Formula.Factory 转 IndustryMoudle.Recipe
            
        }
        private void GenerateResult(Factory[] dependencies)
        {
            string[] inputMaterials = new string[dependencies.Length];
            string[] dependenciesNos = new string[dependencies.Length];
            int h = 1;
            for (int i = 0; i < dependencies.Length; i++)
            {
                if (dependencies[i] == null) continue;
                inputMaterials[i] = dependencies[i].Result;
                dependenciesNos[i] = dependencies[i].Id + "";
                if (dependencies[i].height + 1 > h) h = dependencies[i].height + 1;
            }
            Result = string.Join("", inputMaterials);
            DependenciesNos = string.Join(",", dependenciesNos);
            height = h; // 高度
        }
    }
    static class Program
    {
        static int N = 0;
        static int nowId = 0;
        static List<Factory> factories = new List<Factory>(); // 所有工厂的列表
        // 创建一个字典用于存储没有被依赖的工厂
        // static Dictionary<string, List<Factory>> factoryDict = new Dictionary<string, List<Factory>>();
        static Random random = new Random();
        private static Factory generatorRawMaterial(string res, int flag)
        {
            if (nowId >= N) return null;
            if (res == "") return null;
            if (res.Length == 1)
            {
                Factory newf = new Factory(nowId++, res);
                factories.Add(newf);
                return newf;
            }
            int r = random.Next(6); // 决定2或3输入
            if ((r >= 3 || flag == 2) && res.Length >= 3) // 当flag减小到2且要合成的长度大于3时，必须进行三输入合成，否则高度可能大于5
            { // 3输入
                int r1 = random.Next(0, res.Length - 2); // 第一个index
                int r2 = random.Next(r1 + 1, res.Length - 1); // 第二个index
                string t1 = res.Substring(0, r1 + 1);
                string t2 = res.Substring(r1 + 1, r2 - r1);
                string t3 = res.Substring(r2 + 1);
                Factory[] deps = new Factory[3];
                deps[0] = generatorRawMaterial(t1, flag - 1);
                deps[1] = generatorRawMaterial(t2, flag - 1);
                deps[2] = generatorRawMaterial(t3, flag - 1);
                if (nowId >= N) return null;
                Factory newf = new Factory(nowId++, res == "ABCDEF" ? FactoryType.TopLevel : FactoryType.Processing, deps);
                factories.Add(newf);
                return newf;
            }
            else
            { // 2输入
                int r1 = random.Next(res.Length - 1); // 决定子串长度
                string t1 = res.Substring(0, r1 + 1);
                string t2 = res.Substring(r1 + 1);
                Factory[] deps = new Factory[2];
                deps[0] = generatorRawMaterial(t1, flag - 1);
                deps[1] = generatorRawMaterial(t2, flag - 1);
                if (nowId >= N) return null;
                Factory newf = new Factory(nowId++, res == "ABCDEF" ? FactoryType.TopLevel : FactoryType.Processing, deps);
                factories.Add(newf);
                return newf;
            }
        }
        public static void MyFun(int n)
        {
            Stopwatch stopwatch = new Stopwatch();
            // 开始计时
            stopwatch.Start();

            N = n; // 工厂数量
            while (nowId < N)
            {
                generatorRawMaterial("ABCDEF", 5);
            }
            // 输出工厂信息
            string result1 = @"result1.txt";
            FileStream fs = new FileStream(result1, FileMode.Create);
            StreamWriter wr = null;
            wr = new StreamWriter(fs);
            foreach (Factory factory in factories)
            {
                wr.WriteLine($"序号: {factory.Id}, 类型: {factory.Type}, 来源:{factory.DependenciesNos},原料: {factory.Material}, 高度 : {factory.height}, 合成结果: {factory.Result}");
            }
            wr.Close();

            // 打印时间
            stopwatch.Stop();
            TimeSpan timespan = stopwatch.Elapsed;
            GD.Print("程序运行时间：" + timespan.TotalMilliseconds + "毫秒");
        }
    }
}