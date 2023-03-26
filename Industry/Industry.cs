using Godot;
using System.Collections.Generic;
using System.Linq;
using GraphInformation;

namespace Industry
{
    public partial class Industry : Node2D
    {
        private FontVariation Font;

        Industry()
        {
            Font = new FontVariation();
            Font.BaseFont = ResourceLoader.Load<Font>("res://Render/PingFang-SC-Regular.ttf");

        }
        public override void _Ready()
        {
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }

#if DEBUG
        public override void _Draw()
        {
            foreach (Vertex vertex in Graph.Instance.Vertices)
            {
                if (vertex.Position.IsInRect(0, 0, 1152, 648))
                {
                    var factory = new Factory(Loader.Instance.GetRandomRecipe(), vertex.Position);
                    DrawString(Font, (Vector2)vertex.Position, factory.Recipe, fontSize: 12);
                }
            }
            // foreach (var factory in
            // from factory in Factory.Factories
            // where factory.Recipe.Group == "consumption"
            // select factory)
            // {

            // }
            var a = new Item(10, "A");
            var b = new Item(10, "A");
            GD.Print(a == b);
            GD.Print(a.Equals(b));
            foreach (var curFactory in Factory.Factories)
            {
                foreach (var requirement in curFactory.Recipe.Input)
                {
                    foreach (var target in
                    (from targetFactory in Factory.Factories
                     where targetFactory != curFactory
                     where targetFactory.Recipe.Available(requirement)
                     orderby targetFactory.Position.DistanceSquaredToD(curFactory.Position)
                     select targetFactory))
                    {
                        var link = new ProduceLink(target, curFactory, requirement);
                        DrawLine((Vector2)target.Position, (Vector2)curFactory.Position, new Color(1, 1, 1, 1), 2);
                        break;
                    }
                }
            }
        }
#endif
    }
}


