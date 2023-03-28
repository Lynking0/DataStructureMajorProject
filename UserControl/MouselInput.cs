using Godot;

namespace UserControl
{
    public partial class MouselInput : Control
    {
        [Signal]
        public delegate void MapMoveToEventHandler(Vector2 position);

        private bool MapDragging = false;
        private Vector2? StartDraggingPosition;
        private Vector2? StartPosition;
        private Vector2 MapPosition = new Vector2(0, 0);

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Middle)
            {

                if (!MapDragging && mouseEvent.Pressed)
                {
                    MapDragging = true;
                    StartPosition = MapPosition;
                    StartDraggingPosition = GetGlobalMousePosition();
                }
                if (MapDragging && !mouseEvent.Pressed)
                {
                    MapDragging = false;
                    StartPosition = null;
                    StartDraggingPosition = null;
                }
            }
            else
            {
                if (@event is InputEventMouseMotion motionEvent && MapDragging)
                {
                    MapPosition = (Vector2)StartPosition! + (GetGlobalMousePosition() - (Vector2)StartDraggingPosition!);
                    EmitSignal(SignalName.MapMoveTo, MapPosition);
                }
            }
        }
    }
}