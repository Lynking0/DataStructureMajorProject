using System;
using System.Collections.Generic;
using IndustryMoudle.Entry;
using IndustryMoudle.Link;


namespace IndustryMoudle
{
    public class FactorySchedule
    {
        int BaseProduceSpeed;
        Recipe Recipe;

        public ItemBox IdealInput;
        public ItemBox IdealOutput;
        public FactorySchedule(Factory factory)
        {
            BaseProduceSpeed = factory.BaseProduceSpeed;
            Recipe = factory.Recipe;
            IdealInput = new ItemBox(Recipe.Input) * BaseProduceSpeed;
            IdealOutput = new ItemBox(Recipe.Output) * BaseProduceSpeed;
        }

        // public Item ActualOutputNumber
        // {
        //     get
        //     {
        //         if (Recipe.Output.Count > 1)
        //         {
        //             Logger.error("暂不支持产物数量大于一的配方计算");
        //             throw new System.NotImplementedException("暂不支持产物数量大于一的配方计算");
        //         }
        //         var outputType = new List<ItemType>(Recipe.Output.Keys)[0];
        //         var number = new List<int>(Recipe.Output.Values)[0];
        //         foreach (var type in IdealInput.GetItemTypes())
        //         {
        //             // 假定配方输出数量均为一
        //             var rate = Recipe.Input[type];
        //             number = Math.Min(number, IdealInput.GetItem(type) / rate);
        //         }
        //         return new Item(number, outputType);
        //     }
        // }
    }
}