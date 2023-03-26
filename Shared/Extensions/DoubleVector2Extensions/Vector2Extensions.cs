using System;
using Godot;

namespace Shared.Extensions.DoubleVector2Extensions
{
    public static class _DoubleVector2Extensions
    {
        private const double Epsilon = 1e-14;
        /// <summary>
        /// Returns vec vector's angle with respect to the X axis, or (1, 0) vector, in radians.
        ///
        /// Equivalent to the result of <see cref="Math.Atan2(double, double)"/> when
        /// called with the vector's <see cref="Y"/> and <see cref="X"/> as parameters: <c>Math.Atan2(v.Y, v.X)</c>.
        /// </summary>
        /// <returns>The angle of vec vector, in radians.</returns>
        public static double AngleD(this Vector2 vec)
        {
            return Math.Atan2(vec.Y, vec.X);
        }

        /// <summary>
        /// Returns the angle to the given vector, in radians.
        /// </summary>
        /// <param name="to">The other vector to compare vec vector to.</param>
        /// <returns>The angle between the two vectors, in radians.</returns>
        public static double AngleToD(this Vector2 vec, Vector2D to)
        {
            return Math.Atan2(vec.CrossD(to), vec.DotD(to));
        }

        /// <summary>
        /// Returns the angle between the line connecting the two points and the X axis, in radians.
        /// </summary>
        /// <param name="to">The other vector to compare vec vector to.</param>
        /// <returns>The angle between the two vectors, in radians.</returns>
        public static double AngleToPointD(this Vector2 vec, Vector2D to)
        {
            return Math.Atan2(to.Y - (double)vec.Y, to.X - (double)vec.X);
        }

        /// <summary>
        /// Returns the aspect ratio of vec vector, the ratio of <see cref="X"/> to <see cref="Y"/>.
        /// </summary>
        /// <returns>The <see cref="X"/> component divided by the <see cref="Y"/> component.</returns>
        public static double AspectD(this Vector2 vec)
        {
            return (double)vec.X / (double)vec.Y;
        }

        /// <summary>
        /// Returns the vector "bounced off" from a plane defined by the given normal.
        /// </summary>
        /// <param name="normal">The normal vector defining the plane to bounce off. Must be normalized.</param>
        /// <returns>The bounced vector.</returns>
        public static Vector2D BounceD(this Vector2 vec, Vector2D normal)
        {
            return -vec.ReflectD(normal);
        }

        /// <summary>
        /// Returns a new vector with all components rounded up (towards positive infinity).
        /// </summary>
        /// <returns>A vector with <see cref="Math.Ceiling"/> called on each component.</returns>
        public static Vector2D CeilD(this Vector2 vec)
        {
            return new Vector2D(Math.Ceiling(vec.X), Math.Ceiling(vec.Y));
        }

        /// <summary>
        /// Returns a new vector with all components clamped between the
        /// components of <paramref name="min"/> and <paramref name="max"/> using
        /// <see cref="Mathf.Clamp(double, double, double)"/>.
        /// </summary>
        /// <param name="min">The vector with minimum allowed values.</param>
        /// <param name="max">The vector with maximum allowed values.</param>
        /// <returns>The vector with all components clamped.</returns>
        public static Vector2D ClampD(this Vector2 vec, Vector2D min, Vector2D max)
        {
            return new Vector2D
            (
                Mathf.Clamp(vec.X, min.X, max.X),
                Mathf.Clamp(vec.Y, min.Y, max.Y)
            );
        }

        /// <summary>
        /// Returns the cross product of vec vector and <paramref name="with"/>.
        /// </summary>
        /// <param name="with">The other vector.</param>
        /// <returns>The cross product value.</returns>
        public static double CrossD(this Vector2 vec, Vector2D with)
        {
            return ((double)vec.X * with.Y) - ((double)vec.Y * with.X);
        }

        /// <summary>
        /// Returns the normalized vector pointing from vec vector to <paramref name="to"/>.
        /// </summary>
        /// <param name="to">The other vector to point towards.</param>
        /// <returns>The direction from vec vector to <paramref name="to"/>.</returns>
        public static Vector2D DirectionToD(this Vector2 vec, Vector2D to)
        {
            double lengthsq = vec.DistanceSquaredToD(to);
            if (lengthsq == 0)
                return new Vector2D(0, 0);
            return (to - vec) / Math.Sqrt(lengthsq);
        }

        /// <summary>
        /// Returns the squared distance between vec vector and <paramref name="to"/>.
        /// This method runs faster than <see cref="DistanceTo"/>, so prefer it if
        /// you need to compare vectors or need the squared distance for some formula.
        /// </summary>
        /// <param name="to">The other vector to use.</param>
        /// <returns>The squared distance between the two vectors.</returns>
        public static double DistanceSquaredToD(this Vector2 vec, Vector2D to)
        {
            return ((double)vec.X - to.X) * ((double)vec.X - to.X)
                    + ((double)vec.Y - to.Y) * ((double)vec.Y - to.Y);
        }

        /// <summary>
        /// Returns the distance between vec vector and <paramref name="to"/>.
        /// </summary>
        /// <param name="to">The other vector to use.</param>
        /// <returns>The distance between the two vectors.</returns>
        public static double DistanceToD(this Vector2 vec, Vector2D to)
        {
            return Math.Sqrt(
                ((double)vec.X - to.X) * ((double)vec.X - to.X) +
                ((double)vec.Y - to.Y) * ((double)vec.Y - to.Y));
        }

        /// <summary>
        /// Returns the dot product of vec vector and <paramref name="with"/>.
        /// </summary>
        /// <param name="with">The other vector to use.</param>
        /// <returns>The dot product of the two vectors.</returns>
        public static double DotD(this Vector2 vec, Vector2D with)
        {
            return ((double)vec.X * with.X) + ((double)vec.Y * with.Y);
        }

        /// <summary>
        /// Returns a new vector with all components rounded down (towards negative infinity).
        /// </summary>
        /// <returns>A vector with <see cref="Math.Floor"/> called on each component.</returns>
        public static Vector2D FloorD(this Vector2 vec)
        {
            return new Vector2D(Math.Floor(vec.X), Math.Floor(vec.Y));
        }

        /// <summary>
        /// Returns the inverse of vec vector. This is the same as <c>new Vector2(1 / v.X, 1 / v.Y)</c>.
        /// </summary>
        /// <returns>The inverse of vec vector.</returns>
        public static Vector2D InverseD(this Vector2 vec)
        {
            return new Vector2D(1 / (double)vec.X, 1 / (double)vec.Y);
        }

        /// <summary>
        /// Returns <see langword="true"/> if the vector is normalized, and <see langword="false"/> otherwise.
        /// </summary>
        /// <returns>A <see langword="bool"/> indicating whether or not the vector is normalized.</returns>
        public static bool IsNormalizedD(this Vector2 vec)
        {
            return Math.Abs(vec.LengthSquaredD() - 1.0) < Epsilon;
        }

        /// <summary>
        /// Returns the length (magnitude) of vec vector.
        /// </summary>
        /// <seealso cref="LengthSquared"/>
        /// <returns>The length of vec vector.</returns>
        public static double LengthD(this Vector2 vec)
        {
            return Math.Sqrt(((double)vec.X * (double)vec.X) + ((double)vec.Y * (double)vec.Y));
        }

        /// <summary>
        /// Returns the squared length (squared magnitude) of vec vector.
        /// This method runs faster than <see cref="Length"/>, so prefer it if
        /// you need to compare vectors or need the squared length for some formula.
        /// </summary>
        /// <returns>The squared length of vec vector.</returns>
        public static double LengthSquaredD(this Vector2 vec)
        {
            return ((double)vec.X * (double)vec.X) + ((double)vec.Y * (double)vec.Y);
        }

        /// <summary>
        /// Returns the vector with a maximum length by limiting its length to <paramref name="length"/>.
        /// </summary>
        /// <param name="length">The length to limit to.</param>
        /// <returns>The vector with its length limited.</returns>
        public static Vector2D LimitLengthD(this Vector2 vec, double length = 1.0)
        {
            double l = vec.LengthD();

            if (l > 0 && length < l)
                return new Vector2D((double)vec.X * length / l, (double)vec.Y * length / l);
            return vec;
        }

        /// <summary>
        /// Moves vec vector toward <paramref name="to"/> by the fixed <paramref name="delta"/> amount.
        /// </summary>
        /// <param name="to">The vector to move towards.</param>
        /// <param name="delta">The amount to move towards by.</param>
        /// <returns>The resulting vector.</returns>
        public static Vector2D MoveTowardD(this Vector2 vec, Vector2D to, double delta)
        {
            double len = vec.DistanceToD(to);
            if (len <= delta || len < Epsilon)
                return to;

            return new Vector2D(
                (double)vec.X + (to.X - (double)vec.X) * delta / len,
                (double)vec.Y + (to.Y - (double)vec.Y) * delta / len);
        }

        /// <summary>
        /// Returns the vector scaled to unit length. Equivalent to <c>v / v.Length()</c>.
        /// </summary>
        /// <returns>A normalized version of the vector.</returns>
        public static Vector2D NormalizedD(this Vector2 vec)
        {
            double lengthsq = vec.LengthSquaredD();

            if (lengthsq == 0)
                return new Vector2D(0, 0);
            double length = Math.Sqrt(lengthsq);
            return new Vector2D((double)vec.X / length, (double)vec.Y / length);
        }

        /// <summary>
        /// Returns a vector composed of the <see cref="Mathf.PosMod(double, double)"/> of vec vector's components
        /// and <paramref name="mod"/>.
        /// </summary>
        /// <param name="mod">A value representing the divisor of the operation.</param>
        /// <returns>
        /// A vector with each component <see cref="Mathf.PosMod(double, double)"/> by <paramref name="mod"/>.
        /// </returns>
        public static Vector2D PosModD(this Vector2 vec, double mod)
        {
            return new Vector2D(Mathf.PosMod(vec.X, mod), Mathf.PosMod(vec.Y, mod));
        }

        /// <summary>
        /// Returns a vector composed of the <see cref="Mathf.PosMod(double, double)"/> of vec vector's components
        /// and <paramref name="modv"/>'s components.
        /// </summary>
        /// <param name="modv">A vector representing the divisors of the operation.</param>
        /// <returns>
        /// A vector with each component <see cref="Mathf.PosMod(double, double)"/> by <paramref name="modv"/>'s components.
        /// </returns>
        public static Vector2D PosModD(this Vector2 vec, Vector2D modv)
        {
            return new Vector2D(Mathf.PosMod(vec.X, modv.X), Mathf.PosMod(vec.Y, modv.Y));
        }

        /// <summary>
        /// Returns vec vector projected onto another vector <paramref name="onNormal"/>.
        /// </summary>
        /// <param name="onNormal">The vector to project onto.</param>
        /// <returns>The projected vector.</returns>
        public static Vector2D ProjectD(this Vector2 vec, Vector2D onNormal)
        {
            return onNormal * (vec.DotD(onNormal) / onNormal.LengthSquaredD());
        }

        /// <summary>
        /// Returns vec vector reflected from a plane defined by the given <paramref name="normal"/>.
        /// </summary>
        /// <param name="normal">The normal vector defining the plane to reflect from. Must be normalized.</param>
        /// <returns>The reflected vector.</returns>
        public static Vector2D ReflectD(this Vector2 vec, Vector2D normal)
        {
#if DEBUG
            if (!normal.IsNormalizedD())
            {
                throw new ArgumentException("Argument is not normalized.", nameof(normal));
            }
#endif
            return 2 * vec.DotD(normal) * normal - vec;
        }

        /// <summary>
        /// Rotates vec vector by <paramref name="angle"/> radians.
        /// </summary>
        /// <param name="angle">The angle to rotate by, in radians.</param>
        /// <returns>The rotated vector.</returns>
        public static Vector2D RotatedD(this Vector2 vec, double angle)
        {
            double sine = Math.Sin(angle);
            double cosi = Math.Cos(angle);
            return new Vector2D(
                (double)vec.X * cosi - (double)vec.Y * sine,
                (double)vec.X * sine + (double)vec.Y * cosi);
        }

        /// <summary>
        /// Returns vec vector with all components rounded to the nearest integer,
        /// with halfway cases rounded towards the nearest multiple of two.
        /// </summary>
        /// <returns>The rounded vector.</returns>
        public static Vector2D RoundD(this Vector2 vec)
        {
            return new Vector2D(Math.Round(vec.X), Math.Round(vec.Y));
        }

        /// <summary>
        /// Returns a vector with each component set to one or negative one, depending
        /// on the signs of vec vector's components, or zero if the component is zero,
        /// by calling <see cref="Math.Sign(double)"/> on each component.
        /// </summary>
        /// <returns>A vector with all components as either <c>1</c>, <c>-1</c>, or <c>0</c>.</returns>
        public static Vector2D SignD(this Vector2 vec)
        {
            return new Vector2D(Math.Sign((double)vec.X), Math.Sign((double)vec.Y));
        }

        /// <summary>
        /// Returns vec vector slid along a plane defined by the given <paramref name="normal"/>.
        /// </summary>
        /// <param name="normal">The normal vector defining the plane to slide on.</param>
        /// <returns>The slid vector.</returns>
        public static Vector2D SlideD(this Vector2 vec, Vector2D normal)
        {
            return vec - normal * vec.DotD(normal);
        }

        /// <summary>
        /// Returns vec vector with each component snapped to the nearest multiple of <paramref name="step"/>.
        /// This can also be used to round to an arbitrary number of decimals.
        /// </summary>
        /// <param name="step">A vector value representing the step size to snap to.</param>
        /// <returns>The snapped vector.</returns>
        public static Vector2D SnappedD(this Vector2 vec, Vector2D step)
        {
            return new Vector2D(Mathf.Snapped(vec.X, step.X), Mathf.Snapped(vec.Y, step.Y));
        }

        /// <summary>
        /// Returns a perpendicular vector rotated 90 degrees counter-clockwise
        /// compared to the original, with the same length.
        /// </summary>
        /// <returns>The perpendicular vector.</returns>
        public static Vector2D OrthogonalD(this Vector2 vec)
        {
            return new Vector2D(vec.Y, -vec.X);
        }
        public static bool IsEqualApprox(this Vector2 vec, Vector2D other, double epsilon)
        {
            return Mathf.IsEqualApprox(vec.X, other.X, epsilon) && Mathf.IsEqualApprox(vec.Y, other.Y, epsilon);
        }
    }
}