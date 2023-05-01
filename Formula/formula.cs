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
        TopLevel,
        Consumption
    }
    class Factory
    {
        public int Id { get; }
        public FactoryType Type { get; }
        public string Material { get; set; }
        public string Result { get; set; }
        public string DependenciesNos { get; set; }
        public int height { get; set; } // 最大高度为5
        public Factory(int id, string material) // 用于原料厂
        {
            Id = id;
            Type = FactoryType.RawMaterial;
            Material = "";
            Result = material;
            DependenciesNos = "";
            height = 1;
        }
        public Factory(int id, FactoryType type, Factory[] dependencies) // 用于加工厂和顶级材料厂
        {
            Id = id;
            Type = type;
            GenerateResult(dependencies);
        }
        public Factory(int id, FactoryType type, string material, int depid) // 用于消费工厂
        {
            Id = id;
            Type = type;
            Material = material;
            Result = material;
            DependenciesNos = depid.ToString();
        }
        public IndustryMoudle.Entry.Recipe ToRecipe()
        {
            // TODO: Formula.Factory 转 IndustryMoudle.Recipe
            Dictionary<IndustryMoudle.Entry.ItemType, int> input = new Dictionary<IndustryMoudle.Entry.ItemType, int>();
            Dictionary<IndustryMoudle.Entry.ItemType, int> output = new Dictionary<IndustryMoudle.Entry.ItemType, int>();
            // string[] d = DependenciesNos.Split(",");
            if (Type != FactoryType.RawMaterial)
            {
                string[] m = new string[Material.Split(",").Length];
                m = Material.Split(",");
                for (int i = 0; i < m.Length; i++)
                {
                    input.Add(m[i], 1);
                }
            }
            if (Type != FactoryType.Consumption) output.Add(Result, 1);
            IndustryMoudle.Entry.Recipe res;
            if (Type == FactoryType.Consumption)
            {
                res = new IndustryMoudle.Entry.Recipe(1, "consumption", input, output);
            }
            else if (Type == FactoryType.RawMaterial)
            {
                res = new IndustryMoudle.Entry.Recipe(1, "raw_material", input, output);
            }
            else res = new IndustryMoudle.Entry.Recipe(1, "", input, output);
            return res;
        }
        private void GenerateResult(Factory[] dependencies)
        {
            int realLen = dependencies.Length;
            for (int i = 0; i < dependencies.Length; i++)
            {
                if (dependencies[i] == null) realLen--;
            }
            string[] inputMaterials = new string[realLen];
            int[] dependenciesNos = new int[realLen];
            int h = 1;
            int index = 0;
            for (int i = 0; i < dependencies.Length; i++)
            {
                if (dependencies[i] == null) continue;
                inputMaterials[index] = dependencies[i].Result;
                dependenciesNos[index] = dependencies[i].Id;
                if (dependencies[i].height + 1 > h) h = dependencies[i].height + 1;
                index++;
            }
            Material = string.Join(",", inputMaterials);

            string strInput = string.Join("", inputMaterials);
            char[] arrInput = new char[strInput.Length];
            arrInput = strInput.ToCharArray();
            Array.Sort(arrInput);
            Result = string.Join("", arrInput);

            DependenciesNos = string.Join(",", dependenciesNos);
            height = h; // 高度
        }
    }
    static class Program
    {
        static int N = 0;
        static int nowId = 0;
        static public List<Factory> factories = new List<Factory>(); // 所有工厂的列表
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
            int r = GD.RandRange(0, 5); // 决定2或3输入
            if ((r >= 3 || flag == 2) && res.Length >= 3) // 当flag减小到2且要合成的长度大于3时，必须进行三输入合成，否则高度可能大于5
            { // 3输入
                int r1 = GD.RandRange(0, res.Length - 3); // 第一个index
                int r2 = GD.RandRange(r1 + 1, res.Length - 2); // 第二个index
                // string t1 = res.Substring(0, r1 + 1);
                // string t2 = res.Substring(r1 + 1, r2 - r1);
                // string t3 = res.Substring(r2 + 1);
                string t1 = "", t2 = "", t3 = "";
                char[] arrt1 = new char[r1 + 1];
                char[] arrt2 = new char[r2 - r1];
                string temp = res;
                int index = 0;
                for (int i = 0; i < r1 + 1; i++)
                {
                    index = GD.RandRange(0, temp.Length - 1);
                    arrt1[i] = temp[index];
                    temp = temp.Replace(arrt1[i].ToString(), "");
                }
                for (int i = 0; i < r2 - r1; i++)
                {
                    index = GD.RandRange(0, temp.Length - 1);
                    arrt2[i] = temp[index];
                    temp = temp.Replace(arrt2[i].ToString(), "");
                }
                Array.Sort(arrt1);
                Array.Sort(arrt2);
                t1 = string.Join("", arrt1);
                t2 = string.Join("", arrt2);
                t3 = temp;
                GD.Print(res + "," + t1 + "," + t2 + "," + t3);
                Factory[] deps = new Factory[3];
                deps[0] = generatorRawMaterial(t1, flag - 1);
                deps[1] = generatorRawMaterial(t2, flag - 1);
                deps[2] = generatorRawMaterial(t3, flag - 1);
                if (nowId >= N) return null;
                Factory newf;
                if (res == "ABCDEF")
                {
                    newf = new Factory(nowId++, FactoryType.TopLevel, deps);
                }
                else
                {
                    newf = new Factory(nowId++, FactoryType.Processing, deps);
                }
                factories.Add(newf);
                if (nowId >= N) return newf;
                if (res == "ABCDEF") factories.Add(new Factory(nowId++, FactoryType.Consumption, res, nowId - 2)); // 消费工厂
                return newf;
            }
            else
            { // 2输入
                int r1 = GD.RandRange(0, res.Length - 2); // 决定子串长度
                // string t1 = res.Substring(0, r1 + 1);
                // string t2 = res.Substring(r1 + 1);
                string t1 = "", t2 = "";
                char[] arrt1 = new char[r1 + 1];
                string temp = res;
                int index = 0;
                for (int i = 0; i < r1 + 1; i++)
                {
                    index = GD.RandRange(0, temp.Length - 1);
                    arrt1[i] = temp[index];
                    temp = temp.Replace(arrt1[i].ToString(), "");
                }
                Array.Sort(arrt1);
                t1 = string.Join("", arrt1);
                t2 = temp;
                GD.Print(res + "," + t1 + "," + t2);

                Factory[] deps = new Factory[2];
                deps[0] = generatorRawMaterial(t1, flag - 1);
                deps[1] = generatorRawMaterial(t2, flag - 1);
                if (nowId >= N) return null;
                Factory newf;
                if (res == "ABCDEF")
                {
                    newf = new Factory(nowId++, FactoryType.TopLevel, deps);
                }
                else
                {
                    newf = new Factory(nowId++, FactoryType.Processing, deps);
                }
                factories.Add(newf);
                if (nowId >= N) return newf;
                if (res == "ABCDEF") factories.Add(new Factory(nowId++, FactoryType.Consumption, res, nowId - 2)); // 消费工厂
                return newf;
            }
        }
        public static IEnumerable<IndustryMoudle.Entry.Recipe> GenerateFactories(int n)
        {
            for (int i = 0; i < n; i++)
            {
                yield return generatorRawMaterial("ABCDEF", 5).ToRecipe();
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