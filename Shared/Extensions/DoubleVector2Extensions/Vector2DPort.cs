using System;
using Godot;

namespace Shared.Extensions.DoubleVector2Extensions
{
    public partial struct Vector2D
    {
        private const double Epsilon = 1e-14;
        public static explicit operator Vector2(Vector2D vd) => new Vector2((float)vd.X, (float)vd.Y);
        public static implicit operator Vector2D(Vector2 vf) => new Vector2D(vf.X, vf.Y);
        public static explicit operator Vector2I(Vector2D vd) => new Vector2I((int)vd.X, (int)vd.Y);
        public static implicit operator Vector2D(Vector2I vi) => new Vector2D(vi.X, vi.Y);
        public static Vector2D operator +(Vector2D a)
        {
            return a;
        }
        public static Vector2D operator +(Vector2D a, Vector2D b)
        {
            a.X += b.X;
            a.Y += b.Y;
            return a;
        }
        public static Vector2D operator -(Vector2D a)
        {
            a.X = -a.X;
            a.Y = -a.Y;
            return a;
        }
        public static Vector2D operator -(Vector2D a, Vector2D b)
        {
            a.X -= b.X;
            a.Y -= b.Y;
            return a;
        }
        public static Vector2D operator *(Vector2D a, double b)
        {
            a.X *= b;
            a.Y *= b;
            return a;
        }
        public static Vector2D operator *(double a, Vector2D b)
        {
            b.X *= a;
            b.Y *= a;
            return b;
        }
        public static Vector2D operator /(Vector2D a, double b)
        {
            a.X /= b;
            a.Y /= b;
            return a;
        }
        /// <summary>
        /// Compares two <see cref="Vector2D"/> vectors by first checking if
        /// the X value of the <paramref name="left"/> vector is less than
        /// the X value of the <paramref name="right"/> vector.
        /// If the X values are exactly equal, then it repeats this check
        /// with the Y values of the two vectors.
        /// This operator is useful for sorting vectors.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <returns>Whether or not the left is less than the right.</returns>
        public static bool operator <(Vector2D left, Vector2D right)
        {
            return left.X == right.X ? left.Y < right.Y : left.X < right.X;
        }
        /// <summary>
        /// Compares two <see cref="Vector2D"/> vectors by first checking if
        /// the X value of the <paramref name="left"/> vector is greater than
        /// the X value of the <paramref name="right"/> vector.
        /// If the X values are exactly equal, then it repeats this check
        /// with the Y values of the two vectors.
        /// This operator is useful for sorting vectors.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <returns>Whether or not the left is greater than the right.</returns>
        public static bool operator >(Vector2D left, Vector2D right)
        {
            return left.X == right.X ? left.Y > right.Y : left.X > right.X;
        }
        /// <summary>
        /// Compares two <see cref="Vector2D"/> vectors by first checking if
        /// the X value of the <paramref name="left"/> vector is less than
        /// or equal to the X value of the <paramref name="right"/> vector.
        /// If the X values are exactly equal, then it repeats this check
        /// with the Y values of the two vectors.
        /// This operator is useful for sorting vectors.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <returns>Whether or not the left is less than or equal to the right.</returns>
        public static bool operator <=(Vector2D left, Vector2D right)
        {
            return left.X == right.X ? left.Y <= right.Y : left.X < right.X;
        }
        /// <summary>
        /// Compares two <see cref="Vector2D"/> vectors by first checking if
        /// the X value of the <paramref name="left"/> vector is greater than
        /// or equal to the X value of the <paramref name="right"/> vector.
        /// If the X values are exactly equal, then it repeats this check
        /// with the Y values of the two vectors.
        /// This operator is useful for sorting vectors.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <returns>Whether or not the left is greater than or equal to the right.</returns>
        public static bool operator >=(Vector2D left, Vector2D right)
        {
            return left.X == right.X ? left.Y >= right.Y : left.X > right.X;
        }
        public static bool operator ==(Vector2D left, Vector2D right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Vector2D left, Vector2D right)
        {
            return !left.Equals(right);
        }
        public override bool Equals(object? obj)
        {
            return obj is Vector2D other && Equals(other);
        }
        public bool Equals(Vector2D other)
        {
            return X == other.X && Y == other.Y;
        }
        public override int GetHashCode()
        {
            return Y.GetHashCode() ^ X.GetHashCode();
        }
        public override string ToString()
        {
            return $"({X}, {Y})";
        }
        public string ToString(string format)
        {
            return $"({X.ToString(format)}, {Y.ToString(format)})";
        }
        /// <summary>
        /// Returns this vector's angle with respect to the X axis, or (1, 0) vector, in radians.
        ///
        /// Equivalent to the result of <see cref="Math.Atan2(double, double)"/> when
        /// called with the vector's <see cref="Y"/> and <see cref="X"/> as parameters: <c>Math.Atan2(v.Y, v.X)</c>.
        /// </summary>
        /// <returns>The angle of this vector, in radians.</returns>
        public double AngleD()
        {
            return Math.Atan2(this.Y, this.X);
        }

        /// <summary>
        /// Returns the angle to the given vector, in radians.
        /// </summary>
        /// <param name="to">The other vector to compare this vector to.</param>
        /// <returns>The angle between the two vectors, in radians.</returns>
        public double AngleToD(Vector2D to)
        {
            return Math.Atan2(this.CrossD(to), this.DotD(to));
        }

        /// <summary>
        /// Returns the angle between the line connecting the two points and the X axis, in radians.
        /// </summary>
        /// <param name="to">The other vector to compare this vector to.</param>
        /// <returns>The angle between the two vectors, in radians.</returns>
        public double AngleToPointD(Vector2D to)
        {
            return Math.Atan2(to.Y - this.Y, to.X - this.X);
        }

        /// <summary>
        /// Returns the aspect ratio of this vector, the ratio of <see cref="X"/> to <see cref="Y"/>.
        /// </summary>
        /// <returns>The <see cref="X"/> component divided by the <see cref="Y"/> component.</returns>
        public double AspectD()
        {
            return this.X / this.Y;
        }

        /// <summary>
        /// Returns the vector "bounced off" from a plane defined by the given normal.
        /// </summary>
        /// <param name="normal">The normal vector defining the plane to bounce off. Must be normalized.</param>
        /// <returns>The bounced vector.</returns>
        public Vector2D BounceD(Vector2D normal)
        {
            return -this.ReflectD(normal);
        }

        /// <summary>
        /// Returns a new vector with all components rounded up (towards positive infinity).
        /// </summary>
        /// <returns>A vector with <see cref="Math.Ceiling"/> called on each component.</returns>
        public Vector2D CeilD()
        {
            return new Vector2D(Math.Ceiling(this.X), Math.Ceiling(this.Y));
        }

        /// <summary>
        /// Returns a new vector with all components clamped between the
        /// components of <paramref name="min"/> and <paramref name="max"/> using
        /// <see cref="Mathf.Clamp(double, double, double)"/>.
        /// </summary>
        /// <param name="min">The vector with minimum allowed values.</param>
        /// <param name="max">The vector with maximum allowed values.</param>
        /// <returns>The vector with all components clamped.</returns>
        public Vector2D ClampD(Vector2D min, Vector2D max)
        {
            return new Vector2D
            (
                Mathf.Clamp(this.X, min.X, max.X),
                Mathf.Clamp(this.Y, min.Y, max.Y)
            );
        }

        /// <summary>
        /// Returns the cross product of this vector and <paramref name="with"/>.
        /// </summary>
        /// <param name="with">The other vector.</param>
        /// <returns>The cross product value.</returns>
        public double CrossD(Vector2D with)
        {
            return (this.X * with.Y) - (this.Y * with.X);
        }

        /// <summary>
        /// Returns the normalized vector pointing from this vector to <paramref name="to"/>.
        /// </summary>
        /// <param name="to">The other vector to point towards.</param>
        /// <returns>The direction from this vector to <paramref name="to"/>.</returns>
        public Vector2D DirectionToD(Vector2D to)
        {
            double lengthsq = this.DistanceSquaredToD(to);
            if (lengthsq == 0)
                return new Vector2D(0, 0);
            return (to - this) / Math.Sqrt(lengthsq);
        }

        /// <summary>
        /// Returns the squared distance between this vector and <paramref name="to"/>.
        /// This method runs faster than <see cref="DistanceTo"/>, so prefer it if
        /// you need to compare vectors or need the squared distance for some formula.
        /// </summary>
        /// <param name="to">The other vector to use.</param>
        /// <returns>The squared distance between the two vectors.</returns>
        public double DistanceSquaredToD(Vector2D to)
        {
            return (this.X - to.X) * (this.X - to.X)
                    + (this.Y - to.Y) * (this.Y - to.Y);
        }

        /// <summary>
        /// Returns the distance between this vector and <paramref name="to"/>.
        /// </summary>
        /// <param name="to">The other vector to use.</param>
        /// <returns>The distance between the two vectors.</returns>
        public double DistanceToD(Vector2D to)
        {
            return Math.Sqrt(
                (this.X - to.X) * (this.X - to.X) +
                (this.Y - to.Y) * (this.Y - to.Y));
        }

        /// <summary>
        /// Returns the dot product of this vector and <paramref name="with"/>.
        /// </summary>
        /// <param name="with">The other vector to use.</param>
        /// <returns>The dot product of the two vectors.</returns>
        public double DotD(Vector2D with)
        {
            return (this.X * with.X) + (this.Y * with.Y);
        }

        /// <summary>
        /// Returns a new vector with all components rounded down (towards negative infinity).
        /// </summary>
        /// <returns>A vector with <see cref="Math.Floor"/> called on each component.</returns>
        public Vector2D FloorD()
        {
            return new Vector2D(Math.Floor(this.X), Math.Floor(this.Y));
        }

        /// <summary>
        /// Returns the inverse of this vector. This is the same as <c>new Vector2(1 / v.X, 1 / v.Y)</c>.
        /// </summary>
        /// <returns>The inverse of this vector.</returns>
        public Vector2D InverseD()
        {
            return new Vector2D(1 / this.X, 1 / this.Y);
        }

        /// <summary>
        /// Returns <see langword="true"/> if the vector is normalized, and <see langword="false"/> otherwise.
        /// </summary>
        /// <returns>A <see langword="bool"/> indicating whether or not the vector is normalized.</returns>
        public bool IsNormalizedD()
        {
            return Math.Abs(this.LengthSquaredD() - 1.0) < Epsilon;
        }

        /// <summary>
        /// Returns the length (magnitude) of this vector.
        /// </summary>
        /// <seealso cref="LengthSquared"/>
        /// <returns>The length of this vector.</returns>
        public double LengthD()
        {
            return Math.Sqrt((this.X * this.X) + (this.Y * this.Y));
        }

        /// <summary>
        /// Returns the squared length (squared magnitude) of this vector.
        /// This method runs faster than <see cref="Length"/>, so prefer it if
        /// you need to compare vectors or need the squared length for some formula.
        /// </summary>
        /// <returns>The squared length of this vector.</returns>
        public double LengthSquaredD()
        {
            return (this.X * this.X) + (this.Y * this.Y);
        }

        /// <summary>
        /// Returns the vector with a maximum length by limiting its length to <paramref name="length"/>.
        /// </summary>
        /// <param name="length">The length to limit to.</param>
        /// <returns>The vector with its length limited.</returns>
        public Vector2D LimitLengthD(double length = 1.0)
        {
            double l = this.LengthD();

            if (l > 0 && length < l)
                return new Vector2D(this.X * length / l, this.Y * length / l);
            return this;
        }

        /// <summary>
        /// Moves this vector toward <paramref name="to"/> by the fixed <paramref name="delta"/> amount.
        /// </summary>
        /// <param name="to">The vector to move towards.</param>
        /// <param name="delta">The amount to move towards by.</param>
        /// <returns>The resulting vector.</returns>
        public Vector2D MoveTowardD(Vector2D to, double delta)
        {
            double len = this.DistanceToD(to);
            if (len <= delta || len < Epsilon)
                return to;

            return new Vector2D(
                this.X + (to.X - this.X) * delta / len,
                this.Y + (to.Y - this.Y) * delta / len);
        }

        /// <summary>
        /// Returns the vector scaled to unit length. Equivalent to <c>v / v.Length()</c>.
        /// </summary>
        /// <returns>A normalized version of the vector.</returns>
        public Vector2D NormalizedD()
        {
            double lengthsq = this.LengthSquaredD();

            if (lengthsq == 0)
                return new Vector2D(0, 0);
            double length = Math.Sqrt(lengthsq);
            return new Vector2D(this.X / length, this.Y / length);
        }

        /// <summary>
        /// Returns a vector composed of the <see cref="Mathf.PosMod(double, double)"/> of this vector's components
        /// and <paramref name="mod"/>.
        /// </summary>
        /// <param name="mod">A value representing the divisor of the operation.</param>
        /// <returns>
        /// A vector with each component <see cref="Mathf.PosMod(double, double)"/> by <paramref name="mod"/>.
        /// </returns>
        public Vector2D PosModD(double mod)
        {
            return new Vector2D(Mathf.PosMod(this.X, mod), Mathf.PosMod(this.Y, mod));
        }

        /// <summary>
        /// Returns a vector composed of the <see cref="Mathf.PosMod(double, double)"/> of this vector's components
        /// and <paramref name="modv"/>'s components.
        /// </summary>
        /// <param name="modv">A vector representing the divisors of the operation.</param>
        /// <returns>
        /// A vector with each component <see cref="Mathf.PosMod(double, double)"/> by <paramref name="modv"/>'s components.
        /// </returns>
        public Vector2D PosModD(Vector2D modv)
        {
            return new Vector2D(Mathf.PosMod(this.X, modv.X), Mathf.PosMod(this.Y, modv.Y));
        }

        /// <summary>
        /// Returns this vector projected onto another vector <paramref name="onNormal"/>.
        /// </summary>
        /// <param name="onNormal">The vector to project onto.</param>
        /// <returns>The projected vector.</returns>
        public Vector2D ProjectD(Vector2D onNormal)
        {
            return onNormal * (this.DotD(onNormal) / onNormal.LengthSquaredD());
        }

        /// <summary>
        /// Returns this vector reflected from a plane defined by the given <paramref name="normal"/>.
        /// </summary>
        /// <param name="normal">The normal vector defining the plane to reflect from. Must be normalized.</param>
        /// <returns>The reflected vector.</returns>
        public Vector2D ReflectD(Vector2D normal)
        {
#if SECURITY
            if (!normal.IsNormalizedD())
            {
                throw new ArgumentException("Argument is not normalized.", nameof(normal));
            }
#endif
            return 2 * this.DotD(normal) * normal - this;
        }

        /// <summary>
        /// Rotates this vector by <paramref name="angle"/> radians.
        /// </summary>
        /// <param name="angle">The angle to rotate by, in radians.</param>
        /// <returns>The rotated vector.</returns>
        public Vector2D RotatedD(double angle)
        {
            double sine = Math.Sin(angle);
            double cosi = Math.Cos(angle);
            return new Vector2D(
                this.X * cosi - this.Y * sine,
                this.X * sine + this.Y * cosi);
        }

        /// <summary>
        /// Rotates this vector by <paramref name="angle"/> radians.
        /// </summary>
        /// <param name="angle">The angle to rotate by, in radians.</param>
        /// <returns>The rotated vector.</returns>
        public Vector2D RotatedToD(double angle)
        {
            double len = LengthD();
            return new Vector2D(
                len * Math.Cos(angle),
                len * Math.Sin(angle));
        }

        /// <summary>
        /// Returns this vector with all components rounded to the nearest integer,
        /// with halfway cases rounded towards the nearest multiple of two.
        /// </summary>
        /// <returns>The rounded vector.</returns>
        public Vector2D RoundD()
        {
            return new Vector2D(Math.Round(this.X), Math.Round(this.Y));
        }

        /// <summary>
        /// Returns a vector with each component set to one or negative one, depending
        /// on the signs of this vector's components, or zero if the component is zero,
        /// by calling <see cref="Math.Sign(double)"/> on each component.
        /// </summary>
        /// <returns>A vector with all components as either <c>1</c>, <c>-1</c>, or <c>0</c>.</returns>
        public Vector2D SignD()
        {
            return new Vector2D(Math.Sign(this.X), Math.Sign(this.Y));
        }

        /// <summary>
        /// Returns this vector slid along a plane defined by the given <paramref name="normal"/>.
        /// </summary>
        /// <param name="normal">The normal vector defining the plane to slide on.</param>
        /// <returns>The slid vector.</returns>
        public Vector2D SlideD(Vector2D normal)
        {
            return this - normal * this.DotD(normal);
        }

        /// <summary>
        /// Returns this vector with each component snapped to the nearest multiple of <paramref name="step"/>.
        /// This can also be used to round to an arbitrary number of decimals.
        /// </summary>
        /// <param name="step">A vector value representing the step size to snap to.</param>
        /// <returns>The snapped vector.</returns>
        public Vector2D SnappedD(Vector2D step)
        {
            return new Vector2D(Mathf.Snapped(this.X, step.X), Mathf.Snapped(this.Y, step.Y));
        }

        /// <summary>
        /// Returns a perpendicular vector rotated 90 degrees counter-clockwise
        /// compared to the original, with the same length.
        /// </summary>
        /// <returns>The perpendicular vector.</returns>
        public Vector2D OrthogonalD()
        {
            return new Vector2D(this.Y, -this.X);
        }
    }
}