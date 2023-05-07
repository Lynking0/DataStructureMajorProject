using Godot;


namespace DirectorMoudle
{
    class Constants
    {
        public const float Width = 2500;
        public const float Height = 1500;

        public static Vector2 WorldSize = new Vector2(Width, Height);

        public static Vector2 OriginCoordinates = -(WorldSize / 2);
    }
}