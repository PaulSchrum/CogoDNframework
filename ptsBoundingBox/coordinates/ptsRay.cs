using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ptsCogo.Angle;

namespace ptsCogo.coordinates
{
    public class ptsRay
    {
        public ptsRay() { }

        public ptsRay(ptsPoint startPt, Azimuth directionAZ, Slope slope=null)
        {
            this.StartPoint = startPt;
            this.HorizontalDirection = directionAZ;
            this.advanceDirection = 1;
            this.Slope = slope;
        }

        public ptsRay(double startX, double startY, double azimuthDegrees, Slope slope=null) :
            this(new ptsPoint(startX, startY), Azimuth.fromDegreesDouble(azimuthDegrees), slope)
        {

        }

        public ptsRay(double startX, double startY, double endX, double endY, Slope slope=null)
        {
            this.StartPoint = new ptsPoint(startX, startY);
            var endPoint = new ptsPoint(endX, endY);
            this.HorizontalDirection = new Azimuth(this.StartPoint, endPoint);
            this.advanceDirection = 1;
            this.Slope = slope;
        }

        public ptsRay(string x, string y, string z=null, string azimuth=null, string slope=null)
        {
            this.StartPoint = new ptsPoint(x, y, z);
            this.advanceDirection = 1;
            this.HorizontalDirection = new Azimuth(azimuth);
            this.Slope = new Slope(slope);
        }

        public ptsPoint StartPoint { get; set; }
        public Slope Slope { get; set; }
        private int advanceDirection_ = 1;
        public int advanceDirection
        {
            get { return advanceDirection_; }
            set
            {
                advanceDirection_ = Math.Sign(value);
                if(0 == value) advanceDirection_ = 1;
            }
        }
        public Azimuth HorizontalDirection { get; set; }

        public double? getElevationAlong(double X)
        {
            if(true == Slope.isVertical())
                return null;

            double horizDistance = X - StartPoint.x;

            if(Math.Sign(horizDistance) != Math.Sign(Slope.getAsSlope()))
                return null;

            return (double?)
               ((horizDistance * Slope.getAsSlope()) + this.StartPoint.z);

        }

        public double get_m() { return this.Slope.getAsSlope() * this.advanceDirection; }
        public double get_b()
        {
            if(true == Slope.isVertical())
                return Double.NaN;

            return this.StartPoint.z - (StartPoint.x * Slope.getAsSlope() * this.advanceDirection);
        }

        public ptsPoint IntersectWith_2D(ptsRay otherRay)
        {
            if(this.HorizontalDirection == otherRay.HorizontalDirection)
            {
                double offset = this.getOffset(otherRay.StartPoint);
                if (offset.tolerantEquals(0.0, 0.00001))
                    throw new Exception(
                        "Two rays are colinear. They intersect at all points.");
                else
                    throw new Exception(
                        "Two rays with identical horizontal direction never intersect.");
            }

            // Solution is from https://stackoverflow.com/a/2932601/1339950 .
            // But he refers to this: https://stackoverflow.com/a/2931703/1339950
            var ad = this.HorizontalDirection.AsUnitVector;
            var bd = otherRay.HorizontalDirection.AsUnitVector;
            var As = this.StartPoint;
            var bs = otherRay.StartPoint;

            // Transform points to small values.
            var transformDist = Math.Min(As.x, bs.x);
            As.x -= transformDist;  bs.x -= transformDist;
            transformDist = Math.Min(As.y, bs.y);
            As.y -= transformDist; bs.y -= transformDist;

            var dx = bs.x - As.x;
            var dy = bs.y - As.y;
            var determinate = bd.x * ad.y - bd.y * ad.x;
            var U = (dy * bd.x - dx * bd.y) / determinate;
            var V = (dy * ad.x - dx * ad.y) / determinate;

            var newX = As.x + ad.x * U;
            var newY = As.y + ad.y * U;

            // start here Sept 29 2019
            return new ptsPoint(newX, newY);
        }

        public bool isWithinDomain(double testX)
        {
            if(true == Slope.isVertical())
                return (testX == this.StartPoint.x);

            int sign = Math.Sign(testX - this.StartPoint.x);

            if(Math.Sign(testX - this.StartPoint.x) == this.advanceDirection)
                return true;

            return false;
        }

        public double getOffset(ptsPoint endPt)
        {
            ptsVector directVectr = endPt - this.StartPoint;
            ptsAngle alpha = this.HorizontalDirection - directVectr;
            Double offset = -1.0 * directVectr.Length * Math.Sin(alpha.getAsRadians());
            return offset;
        }

        public override bool Equals(object obj)
        {
            ptsRay other = obj as ptsRay;

            bool pointEqual = this.StartPoint.Equals(other.StartPoint);
            if(!pointEqual) return false;

            bool horDirectionEqual = this.HorizontalDirection.Equals(other.HorizontalDirection);
            if(!horDirectionEqual) return false;

            if(this.Slope is null && other.Slope is null)
                return true;

            if(null == this.Slope ^ null == other.Slope) return false;

            if(this.Slope != other.Slope) return false;

            return true;
        }

        public override string ToString()
        {
            return this.StartPoint.ToString() + " " + this.HorizontalDirection.ToString();
        }
    }
}
