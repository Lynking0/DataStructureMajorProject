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

        private void BuildFactories()
        {
            foreach (Vertex vertex in Graph.Instance.Vertices)
            {
                new Factory(Loader.Instance.GetRandomRecipe(), (Vector2)vertex.Position);
            }
        }

        private void BuildFactoryLinks()
        {
            foreach (var curFactory in Factory.Factories)
            {
                var nearFactories = curFactory.QuadTreeHandle.Nearby(curFactory);
                foreach (var requirement in curFactory.Recipe.Input)
                {
                    foreach (var targetFactory in nearFactories)
                    {
                        if (targetFactory.Recipe.Output.Any(outputItem => requirement.Type == outputItem.Type))
                        {
                            var link = new ProduceLink(targetFactory, curFactory, requirement);
                            break;
                        }
                    }
                }
            }
        }

        public override void _Ready()
        {
            var factoryInitStopWatch = new System.Diagnostics.Stopwatch();
            factoryInitStopWatch.Start();
            BuildFactories();
            BuildFactoryLinks();
            factoryInitStopWatch.Stop();
            GD.Print("Factory build in ", factoryInitStopWatch.ElapsedMilliseconds, " ms");
            Factory.FactoriesQuadTree.Detail();
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }

        private void DrawArrow(Vector2 from, Vector2 to, Color color, float width = -1, bool antialiased = false)
        {
            DrawLine(from, to, color, width, antialiased);
            var arrow = to - from;
            var arrowLength = arrow.Length();
            var arrowHead = arrow.Normalized() * 10;
            var arrowLeft = arrowHead.Rotated(Mathf.Pi / 6);
            var arrowRight = arrowHead.Rotated(-Mathf.Pi / 6);
            DrawLine(to, to - arrowLeft, color, width, antialiased);
            DrawLine(to, to - arrowRight, color, width, antialiased);
        }
        private void DrawFactor(Factory factory)
        {
            DrawString(Font, factory.Position, factory.Recipe, fontSize: 12);
        }
        private void DrawLink(ProduceLink link)
        {
            DrawArrow(link.from.Position, link.to.Position, Colors.WebGray, width: 2);
            DrawString(Font, (link.from.Position + link.to.Position) / 2, link, fontSize: 10, modulate: Colors.Red);
        }

#if DEBUG
        public override void _Draw()
        {
            Factory.Factories.ToList().ForEach(DrawFactor);
            ProduceLink.Links.ForEach(DrawLink);
        }
#endif
    }
}


