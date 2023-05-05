using Godot;

namespace UserControl
{
    public partial class MouselInput : Control
    {
        [Signal]
        public delegate void MapMoveToEventHandler(Vector2 positionDelta);
        [Signal]
        public delegate void MapZoomInEventHandler(Vector2 mousePosition);
        [Signal]
        public delegate void MapZoomOutEventHandler(Vector2 mousePosition);

        private bool MapDragging = false;
        private Vector2? StartDraggingPosition;

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseEvent)
            {
                switch (mouseEvent.ButtonIndex)
                {
                    case MouseButton.Left:
                    case MouseButton.Middle:
                        if (!MapDragging && mouseEvent.Pressed)
                        {
                            MapDragging = true;
                            StartDraggingPosition = GetGlobalMousePosition();
                        }
                        if (MapDragging && !mouseEvent.Pressed)
                        {
                            MapDragging = false;
                            StartDraggingPosition = null;
                        }
                        break;
                    case MouseButton.WheelUp:
                        EmitSignal(SignalName.MapZoomIn, GetGlobalMousePosition());
                        break;
                    case MouseButton.WheelDown:
                        EmitSignal(SignalName.MapZoomOut, GetGlobalMousePosition());
                        break;
                }
            }
            if (@event is InputEventMouseMotion motionEvent && MapDragging)
            {
                var curMousePosition = GetGlobalMousePosition();
                EmitSignal(SignalName.MapMoveTo, curMousePosition - (Vector2)StartDraggingPosition!);
                StartDraggingPosition = curMousePosition;
            }
        }
    }
}