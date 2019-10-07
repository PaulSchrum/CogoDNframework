using ptsCogo.Angle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ptsCogo.coordinates;

namespace ptsCogo
{
    [Serializable]
    public class ptsVector
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public ptsVector() { }
        public ptsVector(double x_, double y_, double z_=0.0)
        {
            x = x_; y = y_; z = z_;
        }

        public ptsVector(ptsPoint beginPt, ptsPoint endPoint)
        {
            x = endPoint.x - beginPt.x;
            y = endPoint.y - beginPt.y;
            z = endPoint.z - beginPt.z;
        }

        public ptsVector(Azimuth direction, Double length)
        {
            x = length * Math.Sin(direction.angle_);
            y = length * Math.Cos(direction.angle_);
            z = 0.0;
        }

        public ptsVector(ptsRay startRay, double length) : 
            this(startRay.HorizontalDirection, length)
        { }

        public void flattenThisZ()
        {
            this.z = 0.0;
        }

        public ptsVector flattenZnew()
        {
            return new ptsVector(this.x, this.y, 0.0);
        }

        public ptsPoint plus(ptsPoint aPoint)
        {
            if (aPoint.isEmpty)
            {
                return new ptsPoint();
            }

            return new ptsPoint(aPoint.x + this.x, aPoint.y + this.y, aPoint.z + this.z);
        }

        public Azimuth Azimuth
        {
            get
            {
                return new Azimuth(Math.Atan2(y, x));
            }
            private set { }
        }

        public Double Length
        {
            get { return Math.Sqrt(x * x + y * y + z * z); }
            private set { }
        }

        public Azimuth DirectionHorizontal
        {
            get { return new Azimuth(Math.Atan2(y, x)); }
            private set { }
        }

        /// <summary>
        /// The slope of the base of this vector if it is a normal vector.
        /// </summary>
        public Slope NormalSlope
        {
            get
            {
                var baseLength = Math.Sqrt(x * x + y * y);
                return new Slope(baseLength, z);
            }
        }

        public double dotProduct(ptsVector otherVec)
        {
            return (this.x * otherVec.x) + (this.y * otherVec.y) + (this.z * otherVec.z);
        }

        public ptsVector crossProduct(ptsVector otherVec)
        {
            ptsVector newVec = new ptsVector();
            newVec.x = this.y * otherVec.z - this.z * otherVec.y;
            newVec.y = this.z * otherVec.x - this.x * otherVec.z;
            newVec.z = this.x * otherVec.y - this.y * otherVec.x;
            return newVec;
        }

        public ptsVector right90degrees()
        {
            var az = this.Azimuth + ptsAngle.HALFCIRCLE / 2.0; ;
            return new ptsVector(az, this.Length);
        }

        public ptsVector left90degrees()
        {
            var az = this.Azimuth + ptsAngle.HALFCIRCLE * 1.5;
            return new ptsVector(az, this.Length);
        }

        public static ptsVector operator +(ptsVector vec1, Deflection defl)
        {
            Azimuth newAz = vec1.Azimuth + defl;
            ptsVector newVec = new ptsVector(newAz, vec1.Length);
            newVec.z = vec1.z;

            return newVec;
        }

        public static ptsVector operator +(ptsVector vec1, ptsVector vec2)
        {
            ptsVector newVec = new ptsVector();
            newVec.x = vec1.x + vec2.x;
            newVec.y = vec1.y + vec2.y;
            newVec.z = vec1.z + vec2.z;

            return newVec;
        }

        public override String ToString()
        {
            var retStr = new StringBuilder(String.Format("L: {0:#.0000}, Az: ", this.Length));
            retStr.Append(this.Azimuth.ToString());
            return  retStr.ToString();
        }


    }
}
