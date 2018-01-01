using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ptsCogo.Angle
{
    sealed public class Azimuth : ptsAngle
    {
        public Azimuth() { }

        /// <summary>
        /// Ctor interprets double value as radians.
        /// </summary>
        /// <param name="anAngleDbl">Initialization value in radians.</param>
        public Azimuth(double anAngleDbl)
        {
            angle_ = anAngleDbl;
        }

        public Azimuth(ptsDegree deg)
        {
            this.angle__ = Math.Atan2(ptsDegree.Sin(deg), ptsDegree.Cos(deg));
        }

        public Azimuth(ptsPoint beginPt, ptsPoint endPt)
        {
            this.angle__ = Math.Atan2(endPt.y - beginPt.y, endPt.x - beginPt.x);
        }

        public Azimuth(string azStr, bool IsDegree=true)
        {
            double az = Convert.ToDouble(azStr);
            if(IsDegree)
                this.setFromDegreesDouble(az);
            else
                angle_ = az;
        }

        public new double angle_ { get { return getAsAzimuth(); } set { base.normalize(value); } }

        public Azimuth reverse()
        {
            return new Azimuth(this.angle__ + Math.PI);
        }

        //public override void setFromXY(double x, double y)
        //{
        //   double dbl = Math.Atan2(x, y);
        //   angle_ = dbl;
        //}

        private double getAsAzimuth()
        {
            double retVal;

            retVal = (-1.0 * base.angle_) + (Math.PI / 2.0);

            return retVal;
        }

        public override double getAsDegreesDouble()
        {
            double retValueDbl = getAsAzimuth() * 180 / Math.PI;
            return retValueDbl >= 0.0 ? retValueDbl : retValueDbl + 360.0;
        }

        public override ptsDegree getAsDegrees()
        {
            ptsDegree retValueDeg = getAsAzimuth() * 180 / Math.PI;
            return retValueDeg >= 0.0 ? retValueDeg : retValueDeg + 360.0;
        }

        public override void setFromDegreesDouble(double degrees)
        {
            //double adjustedDegrees = ((degrees / -180.0)+ 1) *180.0;
            double radians = degrees * Math.PI / 180.0;
            angle_ = Math.Atan2(Math.Cos(radians), Math.Sin(radians));  // This is flipped intentionally

        }

        public static Azimuth fromDegreesDouble(double degrees)
        {
            Azimuth retAz = new Azimuth();
            retAz.setFromDegreesDouble(degrees);
            return retAz;
        }

        public override void setFromDegreesMinutesSeconds(int degrees, int minutes, double seconds)
        {
            setFromDegreesDouble(
                  (double)degrees + (double)minutes / 60.0 + seconds / 3600.0
                           );
        }

        public static int getQuadrant(double angleDegrees)
        {
            return (int)Math.Round((angleDegrees / 90.0) + 0.5);
        }

        //to do:
        //setAsAzimuth
        //getAsDegreeMinuteSecond
        //setAsDegree
        //setAsDegreeMinuteSecond
        //yada

        /// <summary>
        /// Create a new Azimuth 90 degree perpandicular to the 
        /// </summary>
        /// <param name="multiplier">-1.0 for the left Normal</param>
        /// <returns></returns>
        public Azimuth RightNormal(double multiplier=1.0)
        {
            return newAzimuthFromAngle(this + ptsAngle.HALFCIRCLE * multiplier / 2.0);
        }

        public static Azimuth newAzimuthFromAngle(ptsAngle angle)
        {
            Azimuth retAz = new Azimuth();
            retAz.setFromDegreesDouble(angle.getAsDegreesDouble());
            return retAz;
        }

        // operator overloads
        public static implicit operator Azimuth(double angleAs_double)
        {
            Azimuth anAzimuth = new Azimuth();
            anAzimuth.setFromDegreesDouble(angleAs_double);
            return anAzimuth;
        }

        public static Azimuth operator +(Azimuth anAz, ptsAngle anAngle)
        {
            return new Azimuth(anAz.getAsRadians() - anAngle.getAsRadians());  // Note: Subtraction is intentional since azimuths are clockwise
        }

        public static double operator -(Azimuth Az1, Azimuth Az2)
        {
            Double returnDeflection = (Az1.angle_ - Az2.angle_);
            return ptsAngle.normalizeToPlusOrMinus2PiStatic(returnDeflection);
        }

        public static Azimuth operator +(Azimuth Az1, Deflection defl)
        {
            var newAzDeg = Az1.getAsDegreesDouble() + defl.getAsDegreesDouble();
            Double retDbl = ptsAngle.normalizeToPlusOrMinus360Static(newAzDeg);
            Azimuth retAz = new Azimuth();
            retAz.setFromDegreesDouble(retDbl);
            return retAz;
        }

        public Deflection minus(Azimuth Az2)
        {
            Double returnDeflection = (this.angle_ - Az2.angle_);
            return new Deflection(ptsAngle.normalizeToPlusOrMinus2PiStatic(returnDeflection));
        }

        public override String ToString()
        {
            return String.Format("{0:0.0000}°", this.getAsDegreesDouble());
        }
    }

    public static class extendDoubleForAzimuth
    {
        public static Azimuth AsAzimuth(this Double val)
        {
            return new Azimuth(val);
        }
    }

}
