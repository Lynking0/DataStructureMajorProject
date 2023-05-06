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
        private List<Queue<TElement>> Content;
        public int Capacity { get; private set; } = 0;
        public MiniPriorityQueue()
        {
            Content = new List<Queue<TElement>>();
        }
        public MiniPriorityQueue(int initialCapacity)
        {
            Content = new List<Queue<TElement>>(initialCapacity);
            Content.AddRange(Enumerable.Repeat(new Queue<TElement>(), initialCapacity));
            Capacity = initialCapacity;
        }
        public int Count => Content.Sum(queue => queue.Count);
        public void Clear()
        {
            for (int i = 0; i < Capacity; i++)
            {
                Content[i].Clear();
            }
        }
        public int EnsureCapacity(int capacity)
        {
            if (capacity > Capacity)
            {
                for (int i = 0; i < capacity - Capacity; i++)
                {
                    Content.Add(new Queue<TElement>());
                }
                Capacity = capacity;
            }
            return Capacity;
        }
        public TElement Dequeue()
        {
            for (int i = 0; i < Capacity; i++)
            {
                if (Content[i].Count > 0)
                {
                    return Content[i].Dequeue();
                }
            }
            throw new System.InvalidOperationException("Queue is empty");
        }
        public (TElement element, int priority) DequeueWithPriority()
        {
            for (int i = 0; i < Capacity; i++)
            {
                if (Content[i].Count > 0)
                {
                    return (Content[i].Dequeue(), i);
                }
            }
            throw new System.InvalidOperationException("Queue is empty");
        }
        public void Enqueue(TElement element, int priority)
        {
            EnsureCapacity(priority + 1);
            Content[priority].Enqueue(element);
        }
        public void EnqueueRange(IEnumerable<TElement> elements, int priority)
        {
            EnsureCapacity(priority + 1);
            foreach (var element in elements)
            {
                Content[priority].Enqueue(element);
            }
        }
    }
}