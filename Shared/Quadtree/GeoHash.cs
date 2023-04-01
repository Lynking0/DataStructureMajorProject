using System;
using System.Collections.Generic;

namespace Shared.QuadTree
{
    public struct GeoHash
    {
        private int Length = 0;
        public uint X;
        public uint Y;
        public GeoHash()
        {
            X = 0;
            Y = 0;
            Length = 0;
        }

#if DEBUG
        public string Codes
        {
            get
            {
                var result = "";
                var x = X;
                var y = Y;
                for (int i = 0; i < Length; i++)
                {
                    result += ((GeoCode)((x & 1) * 2 + (y & 1)));
                    x >>= 1;
                    y >>= 1;
                }
                return result;
            }
        }
#endif

        public GeoHash(GeoHash parent, GeoCode code) : this()
        {
            Length = parent.Length;
            this.X = parent.X;
            this.Y = parent.Y;
            this += code;
        }

        public GeoHash(uint x, uint y, int length) : this()
        {
            X = x;
            Y = y;
            Length = length;
        }

        public static GeoHash operator +(GeoHash self, GeoCode code)
        {
            self.X <<= 1;
            self.Y <<= 1;
            self.X |= (uint)(code.GetHashCode() / 2) & 1;
            self.Y |= (uint)(code.GetHashCode() % 2) & 1;
            self.Length += 1;
            return self;
        }


        public static GeoCode operator -(GeoHash self, GeoHash other)
        {
            if (!self.Include(other) || self == other)
                throw new Exception("There is no inclusion relation that cannot be subtracted.");
            var x = (other.X >> (other.Length - self.Length - 1)) & 1;
            var y = (other.Y >> (other.Length - self.Length - 1)) & 1;
            return (GeoCode)(y + x * 2);
        }

        public static implicit operator string(GeoHash self)
        {
            string result = "";
            for (int i = self.Length - 1; i >= 0; i--)
            {
                result += ((self.X & (1 << i)) > 0) ? "1" : "0";
                result += ((self.Y & (1 << i)) > 0) ? "1" : "0";
                result += "=>";
            }
            return $"{result}  ({Convert.ToString(self.X, 2).PadLeft(self.Length, '0')}, {Convert.ToString(self.Y, 2).PadLeft(self.Length, '0')})";
        }

        public IEnumerable<GeoHash> Around(uint distance)
        {
            int M = 0;
            for (int i = 0; i < Length; i++)
                M |= 1 << i;
            uint X_ = this.X, Y_ = this.Y;
            bool IsAlive(uint x, uint y)
            {
                if (x < 0 || x > M)
                    return false;
                if (y < 0 || y > M)
                    return false;
                if (x == X_ && y == Y_)
                    return false;
                return true;
            }
            // top line and bottom line
            for (uint x = X - distance; x <= X + distance; x++)
            {
                if (IsAlive(x, Y - distance))
                    yield return new GeoHash(x, Y - distance, Length);
                if (IsAlive(x, Y + distance))
                    yield return new GeoHash(x, Y + distance, Length);
            }
            // left side and right side, but not include top line and bottom line
            for (uint y = Y - distance + 1; y <= Y + distance - 1; y++)
            {
                if (IsAlive(X - distance, y))
                    yield return new GeoHash(X - distance, y, Length);
                if (IsAlive(X + distance, y))
                    yield return new GeoHash(X + distance, y, Length);
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is GeoHash hash &&
                    Length == hash.Length &&
                    X == hash.X && Y == hash.Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Include(GeoHash orther)
        {
            if (orther.Length < Length)
                return false;
            return X == orther.X >> orther.Length - Length
                && Y == orther.Y >> orther.Length - Length;
        }
    }
}