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
            var factoryInitStopWatch = new System.Diagnostics.Stopwatch();
            factoryInitStopWatch.Start();
            foreach (Vertex vertex in Graph.Instance.Vertices)
            {
                var factory = new Factory(Loader.Instance.GetRandomRecipe(), (Vector2)vertex.Position);
                if (vertex.Position.IsInRect(0, 0, 1152, 648))
                {
                    DrawString(Font, (Vector2)vertex.Position, factory.Recipe, fontSize: 12);
                }
            }
            GD.Print(Factory.FactoriesQuadTree.Count);
            foreach (var curFactory in Factory.Factories)
            {
                var nearFactorySets = curFactory.QuadTreeHandle.Nearby();
                foreach (var requirement in curFactory.Recipe.Input)
                {
                    bool requirementFulfill = false;
                    foreach (var nearFactorires in nearFactorySets)
                    {
                        var factorires = nearFactorires.ToList();
                        factorires.Sort((a, b) => a.Position.DistanceSquaredTo(curFactory.Position).CompareTo(b.Position.DistanceSquaredTo(curFactory.Position)));
                        foreach (var targetFactory in factorires)
                        {
                            if (targetFactory.Recipe.Output.Any(outputItem => requirement.Type == outputItem.Type))
                            {
                                var link = new ProduceLink(targetFactory, curFactory, requirement);
                                DrawLine(curFactory.Position, targetFactory.Position, Colors.Red, width: 2);
                                requirementFulfill = true;
                                break;
                            }
                        }
                        if (requirementFulfill)
                            break;
                    }
                }
            }
            factoryInitStopWatch.Stop();
            GD.Print("Factory build in ", factoryInitStopWatch.ElapsedMilliseconds, " ms");
            Factory.FactoriesQuadTree.Detail();
        }
#endif
    }
}


