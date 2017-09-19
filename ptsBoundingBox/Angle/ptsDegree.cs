using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ptsCogo.Angle
{
    public struct ptsDegree
    {
        private readonly Double degrees_;
        public ptsDegree(Double newVal)
        {
            this.degrees_ = newVal;
        }

        public Double getAsRadians()
        {
            return degrees_ * Math.PI / 180.0;
        }

        public Double getAsDouble()
        {
            return degrees_;
        }

        public static ptsDegree newFromRadians(Double rad)
        {
            return new ptsDegree(180.0 * rad / Math.PI);
        }

        public static ptsDegree newFromDegrees(Double degreesDouble)
        {
            return new ptsDegree(degreesDouble);
        }

        public static ptsDegree newFromDegreesMinutesSeconds(int degrees, int minutes, double seconds)
        {
            return new ptsDegree(
                  Math.Abs((double)degrees) +
                  (double)minutes / 60.0 + seconds / 3600.0
                           );
        }

        public static double AsDblFromRadius(double radius)
        {
            return 18000.0 / (Math.PI * radius);
        }

        public static double asRadiusFromDegDouble(double degrees)
        {
            return 18000.0 / (Math.PI * degrees);
        }

        public static ptsDegree FromRadius(double radius)
        {
            double radians = 180.0 / radius;
            var deg = newFromRadians(radians);
            return deg;
        }

        /// <summary>
        /// Arc sine.  (Also known as inverse sine.)
        /// </summary>
        /// <param name="val">Value in distance units.</param>
        /// <returns>An angle in radians.</returns>
        public static ptsDegree Asin(Double val)
        {
            return newFromRadians(Math.Asin(val));
        }

        public static ptsDegree Acos(Double val)
        {
            return newFromRadians(Math.Acos(val));
        }

        public static ptsDegree Atan(Double val)
        {
            return newFromRadians(Math.Atan(val));
        }

        public static ptsDegree Atan2(Double y, Double x)
        {
            return newFromRadians(Math.Atan2(y, x));
        }

        public static Double Sin(ptsDegree deg)
        {
            return Math.Sin(deg.getAsRadians());
        }

        public static Double Cos(ptsDegree deg)
        {
            return Math.Cos(deg.getAsRadians());
        }

        public static Double Tan(ptsDegree deg)
        {
            return Math.Tan(deg.getAsRadians());
        }

        public static ptsDegree Abs(ptsDegree deg)
        {
            return Math.Abs(deg.degrees_);
        }

        public static implicit operator ptsDegree(double doubleVal)
        {
            return new ptsDegree(doubleVal);
        }

        public static bool operator >=(ptsDegree left, ptsDegree right)
        {
            return left.degrees_ >= right.degrees_;
        }

        public static bool operator <=(ptsDegree left, ptsDegree right)
        {
            return left.degrees_ <= right.degrees_;
        }

        public static bool operator >(ptsDegree left, ptsDegree right)
        {
            return left.degrees_ > right.degrees_;
        }

        public static bool operator <(ptsDegree left, ptsDegree right)
        {
            return left.degrees_ < right.degrees_;
        }

        public static bool operator !=(ptsDegree left, ptsDegree right)
        {
            return left.degrees_ != right.degrees_;
        }

        public static bool operator ==(ptsDegree left, ptsDegree right)
        {
            return left.degrees_ == right.degrees_;
        }

        public static ptsDegree operator +(ptsDegree left, Double right)
        {
            return left.degrees_ + right;
        }

        public static ptsDegree operator -(ptsDegree left, ptsDegree right)
        {
            return left.degrees_ - right.degrees_;
        }

        public static ptsDegree operator -(ptsDegree left, Double right)
        {
            return left.degrees_ - right;
        }

        public static ptsDegree operator -(ptsDegree left, Deflection right)
        {
            return left.degrees_ - right.getAsDegrees();
        }

        public static ptsDegree operator *(ptsDegree left, Double right)
        {
            return left.degrees_ * right;
        }

        public static ptsDegree operator /(ptsDegree left, Double right)
        {
            return left.degrees_ * (1.0 / right);
        }

        public override string ToString()
        {
            return degrees_.ToString() + "°";
        }
    }

    public static class extendDoubleForPtsDegree
    {
        public static ptsDegree AsPtsDegree(this Double val)
        {
            return new ptsDegree(val);
        }

        public static double AsPtsDegreeDouble(this Double val)
        {
            return AsPtsDegree(val).getAsDouble();
        }

        public static double dblDegreeFromRadius(this Double inRadius)
        {
            return ptsDegree.AsDblFromRadius(inRadius);
        }

        public static double RadiusFromDegreesDbl(this Double inDegree)
        {
            return ptsDegree.asRadiusFromDegDouble(inDegree);
        }
    }

}
