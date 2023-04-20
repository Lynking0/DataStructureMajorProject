using Godot;


namespace Director
{
    class Constants
    {
        public const float Width = 10000;
        public const float Height = 6000;

        public static Vector2 WorldSize = new Vector2(Width, Height);

        public static Vector2 OriginCoordinates = -(WorldSize / 2);
    }
}