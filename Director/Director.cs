using Godot;
using UserControl;
using TopographyMoudle;

namespace Director
{
    public partial class Director : Control
    {
        private MapController? MapController;
        private MouselInput? MouselInput;

        public override void _Ready()
        {
            MapController = GetNode<MapController>("../MouseInput/GameViewportContainer");
            MouselInput = GetNode<MouselInput>("../MouseInput");

            MouselInput.MapMoveTo += MapController!.SetMapPosition;
            MouselInput.MapZoomIn += MapController.MapZoomIn;
            MouselInput.MapZoomOut += MapController.MapZoomOut;

            Topography.InitParams();
            Topography.Generate();
        }
    }
}