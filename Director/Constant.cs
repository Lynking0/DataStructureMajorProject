using Godot;


namespace Director
{
    class Constant
    {
        public static Vector2 OriginCoordinates
        {
            get => new Vector2((float)GraphMoudle.Graph.MinX, (float)GraphMoudle.Graph.MinY);
        }

        public static Vector2 WorldSize
        {
            get => new Vector2((float)(GraphMoudle.Graph.MaxX - GraphMoudle.Graph.MinX), (float)(GraphMoudle.Graph.MaxY - GraphMoudle.Graph.MinY));
        }
    }
}