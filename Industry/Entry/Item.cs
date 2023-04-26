namespace IndustryMoudle.Entry
{
    /// <summary>
    /// 抽象概念，仅表示类型和数量
    /// 实体应使用 <see cref="IndustryMoudle.Goods"/>
    /// </summary>
    public partial class Item
    {
        public int Number;
        public ItemType Type;
        public Item(int number, ItemType type)
        {
            Number = number;
            Type = type;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Compatible(Item item)
        {
            return Type == item.Type;
        }
    }
}