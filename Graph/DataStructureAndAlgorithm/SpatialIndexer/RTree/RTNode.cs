using System;
using System.Collections.Generic;
using Shared.Extensions.ICollectionExtensions;
using Shared.Extensions.DoubleVector2Extensions;

namespace GraphMoudle.DataStructureAndAlgorithm.SpatialIndexer.RTreeStructure
{
    public partial class RTree
    {
        private abstract class RTNode : IShape
        {
            public RTree RTree;
            public RTNode? Parent;
            /// <summary>
            ///   当前节点的MBR
            /// </summary>
            public RTRect2 Rectangle { get; private set; }
            /// <summary>
            ///   当前节点的层数
            /// </summary>
            public int Level;
            /// <summary>
            ///   RTNode和TData均实现IShape，为RTIntlNode和RTLeafNode获取子MBR的统一方法
            ///   选择IEnumerable<T>仅因为IEnumerable<T>支持协变
            /// </summary>
            public abstract IEnumerable<IShape> SubShapes { get; }
            public abstract int SubShapesCount { get; }
            protected RTNode(RTree rTree, RTNode? parent, int level)
            {
                RTree = rTree;
                Parent = parent;
                Level = level;
            }
            /// <summary>
            ///   添加shape，不改变MBR
            /// </summary>
            protected abstract void AddShape(IShape shape);
            /// <summary>
            ///   清空shape并分裂，不改变MBR
            /// </summary>
            protected abstract RTNode ClearAndSplit();
            protected abstract IShape[] GetSubShapesCopy();
            /// <summary>
            ///   检查Parent是否为空，若为空，则创建一个Parent并调整相关数据（不更改MBR）
            /// </summary>
            protected void CheckParent()
            {
                if (Parent is null)
                {
                    RTIntlNode parent = new RTIntlNode(RTree, null, Level + 1);
                    parent.Children.Add(this);
                    RTree.Root = Parent = parent;
                }
            }
            /// <summary>
            ///   更新MBR
            /// </summary>
            protected void UpdateMBR()
            {
                double minX = double.PositiveInfinity;
                double minY = double.PositiveInfinity;
                double maxX = double.NegativeInfinity;
                double maxY = double.NegativeInfinity;
                foreach (IShape shape in SubShapes)
                {
                    if (shape.Rectangle.TL.X < minX)
                        minX = shape.Rectangle.TL.X;
                    if (shape.Rectangle.TL.Y < minY)
                        minY = shape.Rectangle.TL.Y;
                    if (shape.Rectangle.BR.X > maxX)
                        maxX = shape.Rectangle.BR.X;
                    if (shape.Rectangle.BR.Y > maxY)
                        maxY = shape.Rectangle.BR.Y;
                }
                Rectangle = new RTRect2(
                    new Vector2D(minX, minY),
                    new Vector2D(maxX, maxY)
                );
            }
            /// <summary>
            ///   搜索子树中所有MBR与搜索框有重叠的条目
            /// </summary>
            public abstract void SearchLeaves(RTRect2 searchArea, List<IRTreeData> result);
            /// <summary>
            ///   找出存储某数据的叶子节点
            /// </summary>
            public abstract RTLeafNode? FindLeaf(IRTreeData data);
            private int _comparison((double value, IShape shape) x, (double value, IShape shape) y)
            {
                return Math.Sign(x.value - y.value);
            }
            /// <summary>
            ///   更新节点的MBR，
            ///   若SubShapes数量超出上限，则先进行分裂再更新
            /// </summary>
            public void Adjust()
            {
                if (SubShapesCount <= RTree.Capacity)
                {
                    UpdateMBR();
                    return;
                }
                // 选出两个seed
                double maxIncrement = double.NegativeInfinity;
                IShape? seed1 = null, seed2 = null;
                foreach ((IShape a, IShape b) in SubShapes.ToPairs())
                {
                    double temp = RTRect2.CalcExpandCost(a.Rectangle, b.Rectangle);
                    if (temp > maxIncrement)
                    {
                        seed1 = a;
                        seed2 = b;
                        maxIncrement = temp;
                    }
                }
                // 通过扩展大小确定分组
                List<(double value, IShape shape)> subShapeInfos = new List<(double, IShape)>();
                foreach (IShape shape in SubShapes)
                {
                    if (shape == seed1)
                        subShapeInfos.Add((Double.NegativeInfinity, shape));
                    else if (shape == seed2)
                        subShapeInfos.Add((Double.PositiveInfinity, shape));
                    else
                    {
                        subShapeInfos.Add((
                            seed1!.Rectangle.CalcExpandCost(shape.Rectangle)
                             - seed2!.Rectangle.CalcExpandCost(shape.Rectangle),
                            shape
                        ));
                    }
                }
                subShapeInfos.Sort(_comparison);
                RTNode splitNode = ClearAndSplit();
                int i = 0;
                for (; i < RTree.MinimumCapacity; ++i)
                    this.AddShape(subShapeInfos[i].shape);
                for (; i < subShapeInfos.Count - RTree.MinimumCapacity; ++i)
                {
                    if (subShapeInfos[i].value < 0)
                        this.AddShape(subShapeInfos[i].shape);
                    else
                        splitNode.AddShape(subShapeInfos[i].shape);
                }
                for (; i < subShapeInfos.Count; ++i)
                    splitNode.AddShape(subShapeInfos[i].shape);
                // 调整MBR
                this.UpdateMBR();
                splitNode.UpdateMBR();
            }
            /// <summary>
            ///   更新节点的MBR，
            ///   若SubShapes数量低于下限，则先删除该节点（Parent参数暂时不改变）并记录被删除的条目
            /// </summary>
            public void Condense(List<IShape> RemovedShapes)
            {
                if (SubShapesCount >= RTree.MinimumCapacity || RTree.Root == this)
                {
                    UpdateMBR();
                    return;
                }
                (Parent as RTIntlNode)!.Children.Remove(this);
                RemovedShapes.AddRange(SubShapes);
            }
        }
    }
}