using System.Collections.Generic;
using System.Linq;

namespace IndustryMoudle
{
    /// <summary>
    /// IndustryMoudle专供！！！
    /// 保证FIFO，仅适用于优先级连续且较少的情况
    /// </summary>
    public class MiniPriorityQueue<TElement>
    {
        private Queue<TElement>[] Content;
        public MiniPriorityQueue(int initialCapacity)
        {
            Content = new Queue<TElement>[initialCapacity];
            for (int i = 0; i < Content.Length; i++)
            {
                Content[i] = new Queue<TElement>();
            }
        }
        public int Count => Content.Sum(queue => queue.Count);
        public void Clear()
        {
            for (int i = 0; i < Content.Length; i++)
            {
                Content[i].Clear();
            }
        }
        public TElement Dequeue()
        {
            for (int i = 0; i < Content.Length; i++)
            {
                if (Content[i].Count > 0)
                {
                    return Content[i].Dequeue();
                }
            }
            throw new System.InvalidOperationException("Queue is empty");
        }
        public void Enqueue(TElement element, int priority)
        {
            Content[priority].Enqueue(element);
        }
        public void EnqueueRange(IEnumerable<TElement> elements, int priority)
        {
            foreach (var element in elements)
            {
                Content[priority].Enqueue(element);
            }
        }
    }
}