using System.Collections.Generic;
using Shared.Extensions.DoubleVector2Extensions;

namespace Shared.Extensions.BrokenLineExtensions
{
    public interface IBrokenLineLike
    {
        public List<Vector2D> Points { get; }
    }
}