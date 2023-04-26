

namespace IndustryMoudle.Entry
{
    /// <summary>
    /// 货物，实体概念
    /// 工厂生产时构造，到达下一个工厂即销毁
    /// 若仅表示抽象概念，应使用<see cref=IndustryMoudle.Item />
    /// </summary>
    public class Goods
    {
        public delegate void GoodsLeaveFactoryHandler(Factory factory, TransportMoudle.Train to);
        /// <summary>
        /// 当货物离开工厂时触发
        /// </summary>
        public event GoodsLeaveFactoryHandler? GoodsLeaveFactory;
        /// <summary>
        /// 货物离开工厂
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="to"></param>
        public void LeaveFactory(Factory factory, TransportMoudle.Train to)
        {
            GoodsLeaveFactory?.Invoke(factory, to);
        }

        public delegate void GoodsEnterFactoryHandler(Factory factory, TransportMoudle.Train from);
        /// <summary>
        /// 当货物进入工厂时触发
        /// </summary>
        public event GoodsEnterFactoryHandler? GoodsEnterFactory;
        /// <summary>
        /// 货物进入工厂
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="from"></param>
        public void EnterFactory(Factory factory, TransportMoudle.Train from)
        {
            GoodsEnterFactory?.Invoke(factory, from);
        }

        public delegate void GoodsLoadHandler(Goods goods);
        /// <summary>
        /// TODO: 未实现
        /// 当货物装载到火车时触发
        /// </summary>
        public event GoodsLoadHandler? GoodsLoad;

        public delegate void GoodsUnloadHandler(Goods goods);
        /// <summary>
        /// TODO: 未实现
        /// 当货物从火车卸载时触发
        /// </summary>
        public event GoodsUnloadHandler? GoodsUnload;

        public delegate void GoodsTranformHandler(ItemType from, ItemType to, Factory factory);
        /// <summary>
        /// TODO: 未实现
        /// 当货物生产为其他货物时触发
        /// </summary>
        public event GoodsTranformHandler? GoodsTranform;

        public readonly Item Item;

        public Goods(Item item)
        {
            Item = item;
        }
    }
}