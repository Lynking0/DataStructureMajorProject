using Godot;

namespace Director
{
    partial class Director : Node
    {
        public override void _Ready()
        {
            Topography.Topography.InitParams();
            Topography.Topography.Generate();
        }
    }
}