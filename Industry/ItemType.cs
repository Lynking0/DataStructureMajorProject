namespace IndustryMoudle
{
    public class ItemType
    {
        private string type;
        private ItemType(string type) => this.type = type;
        public static implicit operator ItemType(string type) => new ItemType(type);
        public static implicit operator string(ItemType type) => type.type;
        public static bool operator ==(ItemType self, ItemType obj)
        {
            return self.type == obj.type;
        }
        public static bool operator !=(ItemType self, ItemType obj)
        {
            return self.type != obj.type;
        }
        public override bool Equals(object? obj)
        {
            if (obj is ItemType o)
            {
                return type == o.type;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return type.GetHashCode();
        }
    }
}