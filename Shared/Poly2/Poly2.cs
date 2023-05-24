using Godot;
using System.Collections.Generic;

namespace Shared.Poly2
{
    public class Poly2
    {
        private List<Vector2> _points = new List<Vector2>();
        public Vector2[] Points => _points.ToArray();

        public Poly2(IEnumerable<Vector2> points)
        {
            _points.AddRange(points);
        }
        public Poly2(params Vector2[] points)
        {
            _points.AddRange(points);
        }
        public Poly2(Poly2 poly)
        {
            _points.AddRange(poly._points);
        }

        public void Expand(Vector2 to)
        {
            if (_points.Count <= 2)
            {
                _points.Add(to);
            }
            else
            {
                for (int i = 0; i < _points.Count; i++)
                {
                    var cur = _points[i];
                    var next = _points[(i + 1) % _points.Count];
                    if ((next - cur).Cross(to - cur) < 0)
                    {
                        _points.Insert(i + 1, to);
                        break;
                    }
                }
            }
        }
        public void Expand(float x, float y)
        {
            Expand(new Vector2(x, y));
        }

        public bool HasPoint(Vector2 point)
        {
            for (int i = 0; i < _points.Count; i++)
            {
                var cur = _points[i];
                var next = _points[(i + 1) % _points.Count];
                if ((next - cur).Cross(point - cur) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool Encloses(Poly2 b)
        {
            foreach (var point in b._points)
            {
                if (!HasPoint(point))
                {
                    return false;
                }
            }
            return true;
        }

        public bool Intersects(Rect2 b)
        {
            foreach (var point in _points)
            {
                if (b.HasPoint(point))
                {
                    return true;
                }
            }
            return false;
        }

        public Poly2 Merge(Poly2 b)
        {
            var result = new Poly2(this);
            foreach (var point in b._points)
            {
                result.Expand(point);
            }
            return result;
        }
    }
}