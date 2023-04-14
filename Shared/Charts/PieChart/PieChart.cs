using Godot;
using System;
using System.Collections.Generic;

namespace Shared.Charts
{
    public partial class PieChart : Chart
    {
        public Color[]? cols;
        public int[]? Data;
        public string[]? label;
        public override void _Ready()
        {
            cols = new Color[] {
                Colors.WebGray, Colors.AliceBlue, Colors.DarkOrange, Colors.Chocolate,Colors.BlanchedAlmond,Colors.Crimson
            };
            Data = new int[] { 5, 10, 20, 15, 35, 15 };
            label = new string[] { "标签1", "标签2", "标签3", "标签4", "标签5", "标签6" };
        }
        public void DrawCircleArcPoly(Vector2 center, float radius, float angleFrom, float angleTo, Color color)
        {
            int nbPoints = 32;
            var pointsArc = new Vector2[nbPoints + 2];
            pointsArc[0] = center;
            var colors = new Color[] { color };

            for (int i = 0; i <= nbPoints; i++)
            {
                float anglePoint = Mathf.DegToRad(angleFrom + i * (angleTo - angleFrom) / nbPoints - 90);
                pointsArc[i + 1] = center + new Vector2(Mathf.Cos(anglePoint), Mathf.Sin(anglePoint)) * radius;
            }
            DrawPolygon(pointsArc, colors);
            // for (int i = 2; i < pointsArc.Length; i++)
            // {
            //     DrawLine(pointsArc[i - 1], pointsArc[i], color, antialiased: true, width: 2);
            // }
        }
        public Vector2 transformFromArcToVec(Vector2 center, float radius, float angleFrom, float angleTo)
        {
            float middle = (angleFrom + angleTo) * (float)Math.PI / 360;
            float y = radius * Mathf.Cos(middle);
            float x = radius * Mathf.Sin(middle);
            return new Vector2(center.X + x, center.Y - y);
        }
        public override void _Draw()
        {
            base._Draw();
            var center = new Vector2(300, 150);
            float radius = 100;
            float nowAngle = 0;
            float size = 0;
            Font font = ResourceLoader.Load<FontFile>("res://Render/PingFang-SC-Regular.ttf");
            for (int i = 0; i < Data!.Length; i++)
            {
                size = Data[i] * 3.6f;
                DrawCircleArcPoly(center, radius, nowAngle, nowAngle + size, cols[i]);

                Vector2 middleLine = transformFromArcToVec(center, radius + 20, nowAngle, nowAngle + size);
                DrawLine(center, middleLine, cols[i], 2);
                DrawLine(middleLine, new Vector2(middleLine.X >= center.X ? middleLine.X + 50 : middleLine.X - 50, middleLine.Y), cols[i], 2);
                DrawString(font, new Vector2(middleLine.X >= center.X ? middleLine.X + 10 : middleLine.X - 40, middleLine.Y - 10), Data[i].ToString() + "%", fontSize: 14);

                nowAngle += size;
                DrawRect(new Rect2(25, 25 + 25 * i, 25, 15), cols[i]);
                DrawString(font, new Vector2(55, 38 + 25 * i), label[i], fontSize: 14);
            }
        }
    }
}