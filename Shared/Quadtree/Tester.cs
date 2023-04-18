using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Shared.QuadTree;

#if QUADTREE_DEBUG
namespace Shared.QuadTree
{
    public partial class QuadTree<T> where T : class, ILocatable
    {
        public partial class QuadTreeNode
        {
            public void DrawIn(Node2D node)
            {
                node.DrawRect(Bounds, Colors.Gray, false, 2);

                if (Type == QuadTreeNodeType.Internal)
                {
                    foreach (var subNode in Nodes!)
                    {
                        if (subNode is not null)
                            subNode.DrawIn(node);
                    }
                }
            }
            public void Highlight(Node2D node)
            {
                node.DrawRect(Bounds, Colors.Red, false, 2);
            }
        }

        public void DrawIn(Node2D node)
        {
            Root.DrawIn(node);
            GetItems().ToList().ForEach(item => node.DrawCircle(item.Position, 3, Colors.Red));
        }

        public QuadTreeNode Query(GeoHash hash)
        {
            return Root.Query(hash);
        }
    }

    public class Point : ILocatable
    {
        public Vector2 Position { get; set; }
        public Point(Vector2 position) => Position = position;
        public QuadTree<Point>.Handle? Handle { get; set; }
    }

    public partial class Tester : Node2D
    {
        private QuadTree<Point> Tree;
        private List<Point> Points = new List<Point>();

        Tester()
        {
            Tree = new QuadTree<Point>(new Rect2(0, 0, 1000, 600));
        }

        public void NewPoint(int num)
        {
            for (int i = 0; i < num; i++)
            {
                var point = new Point(new Vector2((float)GD.RandRange(0, 1000), (float)GD.RandRange(0, 600)));
                point.Handle = Tree.Insert(point);
                Points.Add(point);
            }
            QueueRedraw();
        }

        public void NewPoint(int[,] data)
        {
            var result = new QuadTree<Point>.Handle[data.GetLength(0)];
            for (int i = 0; i < data.GetLength(0); i++)
            {
                var point = new Point(new Vector2(data[i, 0], data[i, 1]));
                point.Handle = Tree.Insert(point);
                Points.Add(point);
            }
            QueueRedraw();
        }

        public void Sample1()
        {
            var ps = new int[,] {
                {10,10},  // 0//
                {30,30},//
                {300,170},//
                {300,180},//
                {300,250},//
                {400,200},//

                {300,280},//
                {390,280},//
                {410,280}, // 8//
                {420,280},//
                {450,280}, // 10//
                
                {450,290},//

                {870,570}, // where are you?
                {900,570},//
                {920,570},//
                {950,570},//
                {980,570}, // 16//
        };
            NewPoint(ps);
        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            Sample1();
            foreach (var item in Points[8]!.Handle!.Nearby(Points[8]))
            {
                GD.Print(item.Position);
            }
            GD.Print(Points[8].Handle.Node.Bounds);
            GD.Print(Points[8]!.Handle!.Nearby(Points[8]).Count());
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }

        public override void _Draw()
        {
            Tree.DrawIn(this);
        }
    }
}
#endif