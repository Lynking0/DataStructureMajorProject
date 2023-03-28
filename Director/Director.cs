using Godot;

namespace Director
{
    partial class Director : Control
    {
        private MapController? MapController;
        private UserControl.MouselInput? MouselInput;

        public override void _Ready()
        {
            MapController = GetNode<MapController>("../MouseInput/GameViewportContainer");
            MouselInput = GetNode<UserControl.MouselInput>("../MouseInput");
            // GD.Print("MapController: ", MapController);
            // GD.Print("MouselInput: ", MouselInput);

            TopographyMoudle.Topography.InitParams();
            TopographyMoudle.Topography.Generate();
        }
    }
}