using System.Collections.Generic;

namespace IndustryMoudle.Entry
{
    public class ItemBox
    {
        private Dictionary<ItemType, int> Content = new Dictionary<ItemType, int>();

        public ItemBox() { }
        public ItemBox(IReadOnlyCollection<Item> items)
        {
            foreach (var item in items)
            {
                AddItem(item);
            }
        }
        public ItemBox(IReadOnlyDictionary<ItemType, int> items)
        {
            foreach (var (type, number) in items)
            {
                AddItem(type, number);
            }
        }

        public bool Empty => Content.Count == 0;
        public IEnumerable<ItemType> Types => Content.Keys;

        public static implicit operator string(ItemBox self)
        {
            var result = string.Empty;
            foreach (var (type, number) in self.Content)
            {
                result += $"{(string)type}({number}) ";
            }
            return result;
        }

        public Dictionary<ItemType, int> ToDictionary()
        {
            return new Dictionary<ItemType, int>(Content);
        }

        public IEnumerable<ItemType> GetItemTypes()
        {
            return Content.Keys;
        }

        public void AddItem(ItemType type, int number)
        {
            if (Content.ContainsKey(type))
            {
                Content[type] += number;
            }
            else
            {
                Content[type] = number;
            }
        }

        public (int deficit, int actual) RequireItem(ItemType type, int number)
        {
            if (Content.ContainsKey(type))
            {
                if (number < Content[type])
                {
                    Content[type] -= number;
                    return (0, number);
                }
                else if (number == Content[type])
                {
                    Content.Remove(type);
                    return (0, number);
                }
                else
                {
                    var result = number - Content[type];
                    Content.Remove(type);
                    return (result, number - result);
                }
            }
            return (number, 0);
        }

        public void AddItem(Item item)
        {
            AddItem(item.Type, item.Number);
        }

        public int GetItem(ItemType type)
        {
            if (Content.ContainsKey(type))
            {
                return Content[type];
            }
            return 0;
        }

        public static ItemBox operator +(ItemBox self, ItemBox other)
        {
            var result = new ItemBox();
            foreach (var (type, number) in self.Content)
            {
                result.AddItem(type, number);
            }
            foreach (var (type, number) in other.Content)
            {
                result.AddItem(type, number);
            }
            return result;
        }

        public static ItemBox operator *(ItemBox self, int scale)
        {
            var result = new ItemBox();
            foreach (var (type, number) in self.Content)
            {
                result.AddItem(type, number * scale);
            }
            return result;
        }
    }
}