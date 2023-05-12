using Godot;

namespace TransportMoudle
{
    public partial class Carriage : PathFollow2D
    {
        public readonly bool IsHead;
        public int Size = 20;
        public override void _Draw()
        {
            DrawRect(new Rect2(new Vector2(-Size / 2, -Size / 2), new Vector2(Size, Size)), Colors.Red);
        }
        public override void _Process(double delta)
        {
            Progress += 1;
        }
    }
}