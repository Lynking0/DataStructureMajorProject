using Godot;


namespace DirectorMoudle
{
    class Constants
    {
        public const float Magnification = 1;
        public const float Width = 10000 * Magnification;
        public const float Height = 6000 * Magnification;

        public static Vector2 WorldSize = new Vector2(Width, Height);
        public static Vector2 OriginCoordinates = -(WorldSize / 2);
    }
}