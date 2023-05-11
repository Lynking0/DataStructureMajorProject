using Godot;


namespace DirectorMoudle
{
    class Constants
    {
        public const float Width = 2000;
        public const float Height = 2000;

        public static Vector2 WorldSize = new Vector2(Width, Height);

        public static Vector2 OriginCoordinates = -(WorldSize / 2);
    }
}