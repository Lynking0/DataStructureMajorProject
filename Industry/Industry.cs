using Godot;
using System.Collections.Generic;
using System.Linq;
using GraphInformation;
using Shared.QuadTree;

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
            var quadtree = new QuadTree<Factory>(new Rect2(0, 0, 16, 16));
            var r = new Recipe(1, "", new List<Item>(), new List<Item>());
            quadtree.Insert(new Factory(r, new Vector2(0, 0)));
            quadtree.Insert(new Factory(r, new Vector2(1, 1)));
            quadtree.Insert(new Factory(r, new Vector2(2, 2)));
            quadtree.Insert(new Factory(r, new Vector2(3, 3)));
            quadtree.Insert(new Factory(r, new Vector2(4, 4)));
            GD.Print(quadtree);
            quadtree.Remove(new Factory(r, new Vector2(4, 4)));
            GD.Print(quadtree);
            foreach (Vertex vertex in Graph.Instance.Vertices)
            {
                if (vertex.Position.IsInRect(0, 0, 1152, 648))
                {
                    var factory = new Factory(Loader.Instance.GetRandomRecipe(), (Vector2)vertex.Position);
                    DrawString(Font, (Vector2)vertex.Position, factory.Recipe, fontSize: 12);
                }
            }
            foreach (var curFactory in Factory.Factories)
            {
                foreach (var requirement in curFactory.Recipe.Input)
                {
                    foreach (var target in
                    (from targetFactory in Factory.Factories
                     where targetFactory != curFactory
                     where targetFactory.Recipe.Available(requirement)
                     orderby targetFactory.Position.DistanceSquaredTo(curFactory.Position)
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


