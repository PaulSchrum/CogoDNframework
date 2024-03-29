﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ptsCogo.Angle
{
    sealed public class Deflection : ptsAngle
    {
        //protected double angle__;
        private int deflectionDirection_;
        public int deflectionDirection
        {
            get { return deflectionDirection_; }
            set
            {
                deflectionDirection_ = value >= 0 ? 1 : -1;
            }
        }

        public bool isLessThanEqual_180degrees { get; set; }

        public Deflection() { }

        public Deflection(Azimuth BeginAzimuth, Azimuth EndAzimuth, bool assumeDeflectionIsLessThan180Degrees)
        {
            isLessThanEqual_180degrees = assumeDeflectionIsLessThan180Degrees;

            if(false == isLessThanEqual_180degrees)
                this.angle_ = BeginAzimuth - EndAzimuth;
            else
                this.angle_ = EndAzimuth - BeginAzimuth;

            this.deflectionDirection = 1;
            if(this.angle_ < 0.0 || this.angle_ > Math.PI)
            {
                this.deflectionDirection = -1;
            }
        }

        public static Deflection ctorDeflectionFromAngle(double angleDegrees, int deflectionDirection)
        {
            return new Deflection(ptsAngle.radiansFromDegree(angleDegrees), deflectionDirection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anAngleDbl">unit: Radians</param>
        /// <param name="deflectionSign">+1 for Right, -1 for left</param>
        public Deflection(double anAngleDbl, int deflectionDirection)
        {
            base.angle_ = Math.Abs(anAngleDbl);
            this.deflectionDirection = deflectionDirection;
            //base.angle__ = ptsAngle.normalizeToPlusOrMinus2PiStatic(anAngleDbl);
            //angle_ = anAngleDbl;
        }

        public Deflection(ptsAngle anAngle)
        {
            this.deflectionDirection = Math.Sign(anAngle.angle_);
            angle__ = Math.Abs(anAngle.angle_);
        }

        public Deflection(double deflectionAngleDbl) : this(deflectionAngleDbl, Math.Sign(deflectionAngleDbl))
        { }

        internal override double angle_
        {
            get
            {
                Double retAngle = angle__;

                if(deflectionDirection >= 0)
                {
                    if(retAngle < 0.0)
                        retAngle += 2.0 * Math.PI;
                }
                else
                {
                    if(retAngle < 0.0)
                        retAngle += 2.0 * Math.PI;

                    retAngle *= -1.0;
                }

                return retAngle;
            }
            set { normalize(value); }
        }

        public override void setFromDegreesDouble(double deg)
        {
            base.setFromDegreesDouble(Math.Abs(deg));
            this.deflectionDirection = Math.Sign(deg);
        }

        public override double getAsRadians()
        {
            Double retVal = base.getAsRadians();
            if(this.deflectionDirection < 0)
            {
                if(this.isLessThanEqual_180degrees == true)
                {
                    retVal = retVal + 2.0 * Math.PI;
                    retVal *= -1.0;
                }
            }
            return retVal;
        }

        public override void setFromDegreesMinutesSeconds(int degrees, int minutes, double seconds)
        {
            setFromDegreesDouble(
                  Math.Abs((double)degrees) +
                  (double)minutes / 60.0 + seconds / 3600.0
                           );
            deflectionDirection = Math.Sign(degrees);
        }

        public override double getAsDegreesDouble()
        {
            return 180.0 * this.getAsRadians() / Math.PI;
        }

        public Deflection piCompliment
        {
            get
            {
                double angl = Math.PI - this.angle_;
                return new Deflection(angl * this.deflectionDirection);
            }
        }

        public static Deflection Perpandicular(int deflDirection)
        {
            return new Deflection(ptsAngle.HALFCIRCLE / 2.0 * deflDirection);
        }

        public static implicit operator Deflection(double radianDbl)
        {
            return new Deflection(radianDbl);
        }

        public static implicit operator Deflection(ptsDegree degrees)
        {
            return new Deflection(degrees.getAsRadians());
        }

        public static Deflection operator *(Deflection theDef, double multiplier)
        {
            double angl = theDef.deflectionDirection_ * theDef.angle__ * multiplier;
            return new Deflection(angl);
        }

        public static Deflection operator /(Deflection theDef, double divisor)
        {
            double angl = theDef.deflectionDirection_ * theDef.angle__ / divisor;
            return new Deflection(angl);
        }

        public override string ToString()
        {
            return this.getAsDegreesDouble().ToString();
        }
    }
}
